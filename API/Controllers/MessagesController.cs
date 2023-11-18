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
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public MessagesController(
			IUnitOfWork unitOfWork, 
			IMapper mapper)
        {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		[HttpPost]
		public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
		{
			var senderUsername = User.GetUserName();
			if (senderUsername == createMessageDto.RecipientUsername.ToLower()) return BadRequest("Bad request");

			var sender = await _unitOfWork.UserRepository.GetUserByUserNameAsync(senderUsername);
			var recipient = await _unitOfWork.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

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
			_unitOfWork.MessageRepository.AddMessage(message);

			if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

			return BadRequest("Failed to send message");
		}

		[HttpGet]
		public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
		{
			messageParams.Username = User.GetUserName();
			var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);
			Response.AddPaginationHeader(new PaginationHeader(
				messages.CurrentPage,
				messages.PageSize, 
				messages.TotalCount, 
				messages.TotalPage));
			return Ok(messages);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteMessage(int id)
		{
			var currentUsername = User.GetUserName();
			var message = await _unitOfWork.MessageRepository.GetMessage(id);
			if(message.SenderUsername != currentUsername && message.RecipientUsername != currentUsername) return Unauthorized();

			if (message.SenderUsername == currentUsername) message.SenderDeleted = true;
			if (message.RecipientUsername == currentUsername) message.RecipientDeleted = true;
			if (message.SenderDeleted && message.RecipientDeleted) _unitOfWork.MessageRepository.DeleteMessage(message);

			if(await _unitOfWork.Complete()) return Ok();

			return BadRequest("Problems at deleting the message");
		}

    }
}
