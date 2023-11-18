using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class ReportRepository : IReportRepository
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public ReportRepository(DataContext context, IMapper mapper)
        {
			_context = context;
			_mapper = mapper;
		}
        public async Task<UserReportDto> GetUserReportById(int id)
		{
			return await _context.UserReports
				.ProjectTo<UserReportDto>(_mapper.ConfigurationProvider)
				.SingleOrDefaultAsync(x =>x.Id == id);
		}

		public async Task<IEnumerable<UserReportDto>> GetUserReports()
		{
			return await _context.UserReports.ProjectTo<UserReportDto>(_mapper.ConfigurationProvider).ToListAsync();
		}

		public async Task<IEnumerable<UserReportDto>> GetUserReportedReports(int userId)
		{
			return await _context.UserReports
				.ProjectTo<UserReportDto>(_mapper.ConfigurationProvider)
				.Where(x => x.Reporter.Id == userId)
				.ToListAsync();
		}
	}
}
