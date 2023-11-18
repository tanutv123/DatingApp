using API.interfaces;
using AutoMapper;

namespace API.Data
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public UnitOfWork(DataContext _context, IMapper _mapper)
        {
			this._context = _context;
			this._mapper = _mapper;
		}
        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

		public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

		public ILikesRepository LikeRepository => new LikesRepository(_context);

		public IReportRepository ReportRepository => new ReportRepository(_context, _mapper);

		public async Task<bool> Complete()
		{
			return await _context.SaveChangesAsync() > 0;
		}

		public bool HasChanges()
		{
			return _context.ChangeTracker.HasChanges();
		}
	}
}
