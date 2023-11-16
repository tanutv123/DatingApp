﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
	public class MessageHub : Hub
	{
		private readonly IMessageRepository _messageRepository;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		private readonly IHubContext<PresenceHub> _presenceHub;

		public MessageHub(IMessageRepository messageRepository, 
							IUserRepository userRepository, 
							IMapper mapper ,
							IHubContext<PresenceHub> presenceHub)
        {
			_messageRepository = messageRepository;
			_userRepository = userRepository;
			_mapper = mapper;
			_presenceHub = presenceHub;
		}

		public override async Task OnConnectedAsync()
		{
			var httpContext = Context.GetHttpContext();
			var otherUser = httpContext.Request.Query["user"];
			var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			await AddToGroup(groupName);

			var messages = await _messageRepository
				.GetMessageThread(Context.User.GetUserName(), otherUser);
			await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
		}

		public async Task SendMessage(CreateMessageDto createMessageDto)
		{
			var senderUsername = Context.User.GetUserName();
			if (senderUsername == createMessageDto.RecipientUsername.ToLower()) throw new HubException("You cannot send messages to yourself");

			var sender = await _userRepository.GetUserByUserNameAsync(senderUsername);
			var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

			if (sender == null || recipient == null) throw new HubException("User not found");

			var message = new Message
			{
				//EF Core will automatically populate the SenderId and RecipientId fields 
				//But not for the SenderUsername and RecipientUsername
				Sender = sender,
				Recipient = recipient,
				SenderUsername = senderUsername,
				RecipientUsername = recipient.UserName,
				Content = createMessageDto.Content
			};

			var groupName = GetGroupName(senderUsername, recipient.UserName);
			var group = await _messageRepository.GetMessageGroup(groupName);
			if(group.Connections.Any(x => x.Username == recipient.UserName))
			{
				message.DateRead = DateTime.UtcNow;
			} else
			{
				var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
				if(connections != null)
				{
					await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
						new 
						{
							username = sender.UserName,
							knownAs = sender.KnownAs
						});
				}
			}

			_messageRepository.AddMessage(message);

			if (await _messageRepository.SaveAllAsync())
			{
				await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
			}
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			await RemoveFromMessageGroup();
			await base.OnDisconnectedAsync(exception);
		}

		private string GetGroupName(string caller, string other)
		{
			var stringCompare = string.CompareOrdinal(caller, other) < 0;
			return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
		}

		private async Task<bool> AddToGroup(string groupName)
		{
			var group = await _messageRepository.GetMessageGroup(groupName);
			var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

			if(group==null)
			{
				group = new Group(groupName);
				_messageRepository.AddGroup(group);
			}

			group.Connections.Add(connection);

			return await _messageRepository.SaveAllAsync();
		}

		private async Task RemoveFromMessageGroup()
		{
			var connection = await _messageRepository.GetConnectionByConnectionId(Context.ConnectionId);
			_messageRepository.RemoveConnection(connection);
			await _messageRepository.SaveAllAsync();
		}
    }
}
