using API.Entities;

namespace API.DTOs
{
	public class CreateReportDto
	{
        public string Content { get; set; }
        public string ReportedUsername { get; set; }
        public List<ReportType> ReportTypes { get; set; }
    }
}
