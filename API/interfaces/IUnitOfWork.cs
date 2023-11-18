namespace API.interfaces
{
	public interface IUnitOfWork
	{
		IUserRepository UserRepository { get; }
		IMessageRepository MessageRepository{ get; }
		ILikesRepository LikeRepository{ get; }
		IReportRepository ReportRepository { get; }
		Task<bool> Complete();
		bool HasChanges();

	}
}
