using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.interfaces
{
	public interface IMessageRepository
	{
		void AddMessage(Message message);
		void DeleteMessage(Message message);
		Task<Message> GetMessage(int id);
		Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
		Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
		void AddGroup(Group group);
		void RemoveConnection(Connection connection);
		Task<Connection> GetConnectionByConnectionId(string id);
		Task<Group> GetMessageGroup(string groupName);
	}
}
