using API.DTOs;
using API.Entities;
using API.Extensions;
using API.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
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

		[Authorize(Policy = "ModeratorPhotoRole")]
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

		[HttpGet("report-types")]
		public async Task<ActionResult<IEnumerable<ReportType>>> GetReportTypes()
		{
			return Ok(await _unitOfWork.ReportRepository.GetReportTypes());
		}

		[HttpPost]
		public async Task<ActionResult> AddReport(CreateReportDto createReportDto)
		{
			var reportedUserId = _unitOfWork.UserRepository.GetUserId(createReportDto.ReportedUsername).Result;
			var reporterUserId = User.GetUserId();
			var userReport = new UserReport
			{
				Id = await _unitOfWork.ReportRepository.GetTotalReports() + 1,
				Content = createReportDto.Content,
				ReporterUserId = reporterUserId,
				ReportedUserId = reportedUserId
			};
			foreach(var type in createReportDto.ReportTypes)
			{
				var report = new Report
				{
					UserReportId = userReport.Id,
					ReportTypeId = type.Id
				};
				_unitOfWork.ReportRepository.AddReport(userReport, report);
			}
			if (await _unitOfWork.Complete()) return NoContent();

			return BadRequest("Problems in submit report");
		}

	}
}
