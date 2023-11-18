using API.DTOs;
using API.Entities;
using API.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize(Policy = "ModeratorPhotoRole")]
	public class ReportController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;

		public ReportController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<IEnumerable<UserReportDto>>> GetReportForUser(int id)
		{
			return Ok(await _unitOfWork.ReportRepository.GetUserReportedReports(id));
		}
		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserReportDto>>> GetReports()
		{
			return Ok(await _unitOfWork.ReportRepository.GetUserReports());
		}
		[HttpGet("get-report/{id}")]
		public async Task<ActionResult<UserReportDto>> GetReportById(int id)
		{
			return await _unitOfWork.ReportRepository.GetUserReportById(id);
		}
	}
}
