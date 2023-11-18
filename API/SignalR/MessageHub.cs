using API.Data;
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
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IHubContext<PresenceHub> _presenceHub;

		public MessageHub(IUnitOfWork unitOfWork,IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
			_unitOfWork = unitOfWork;
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

			var messages = await _unitOfWork.MessageRepository
				.GetMessageThread(Context.User.GetUserName(), otherUser);
			if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();
			await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
		}

		public async Task SendMessage(CreateMessageDto createMessageDto)
		{
			var senderUsername = Context.User.GetUserName();
			if (senderUsername == createMessageDto.RecipientUsername.ToLower()) throw new HubException("You cannot send messages to yourself");

			var sender = await _unitOfWork.UserRepository.GetUserByUserNameAsync(senderUsername);
			var recipient = await _unitOfWork.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

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
			var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
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

			_unitOfWork.MessageRepository.AddMessage(message);

			if (await _unitOfWork.Complete())
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
			var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
			var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

			if(group==null)
			{
				group = new Group(groupName);
				_unitOfWork.MessageRepository.AddGroup(group);
			}

			group.Connections.Add(connection);

			return await _unitOfWork.Complete();
		}

		private async Task RemoveFromMessageGroup()
		{
			var connection = await _unitOfWork.MessageRepository.GetConnectionByConnectionId(Context.ConnectionId);
			_unitOfWork.MessageRepository.RemoveConnection(connection);
			await _unitOfWork.Complete();
		}
    }
}
