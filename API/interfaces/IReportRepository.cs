using API.DTOs;
using API.Entities;

namespace API.interfaces
{
	public interface IReportRepository
	{
		Task<UserReportDto> GetUserReportById(int id);
		Task<IEnumerable<UserReportDto>> GetUserReports();
		Task<IEnumerable<UserReportDto>> GetUserReportedReports(int userId);
	}
}
