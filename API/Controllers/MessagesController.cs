using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class MessagesController : BaseApiController
	{
		private readonly IUserRepository _userRepository;
		private readonly IMessageRepository _messageRepository;
		private readonly IMapper _mapper;

		public MessagesController(
			IUserRepository userRepository, 
			IMessageRepository messageRepository, 
			IMapper mapper)
        {
			_userRepository = userRepository;
			_messageRepository = messageRepository;
			_mapper = mapper;
		}

		[HttpPost]
		public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
		{
			var senderUsername = User.GetUserName();
			if (senderUsername == createMessageDto.RecipientUsername.ToLower()) return BadRequest("Bad request");

			var sender = await _userRepository.GetUserByUserNameAsync(senderUsername);
			var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

			if (sender == null || recipient == null) return NotFound();

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
			_messageRepository.AddMessage(message);

			if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

			return BadRequest("Failed to send message");
		}

		[HttpGet]
		public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
		{
			messageParams.Username = User.GetUserName();
			var messages = await _messageRepository.GetMessagesForUser(messageParams);
			Response.AddPaginationHeader(new PaginationHeader(
				messages.CurrentPage,
				messages.PageSize, 
				messages.TotalCount, 
				messages.TotalPage));
			return Ok(messages);
		}

		[HttpGet("thread/{username}")]
		public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
		{
			var currentUsername = User.GetUserName();
			return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteMessage(int id)
		{
			var currentUsername = User.GetUserName();
			var message = await _messageRepository.GetMessage(id);
			if(message.SenderUsername != currentUsername && message.RecipientUsername != currentUsername) return Unauthorized();

			if (message.SenderUsername == currentUsername) message.SenderDeleted = true;
			if (message.RecipientUsername == currentUsername) message.RecipientDeleted = true;
			if (message.SenderDeleted && message.RecipientDeleted) _messageRepository.DeleteMessage(message);

			if(await _messageRepository.SaveAllAsync()) return Ok();

			return BadRequest("Problems at deleting the message");
		}

    }
}
