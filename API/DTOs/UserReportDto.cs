using API.Entities;

namespace API.DTOs
{
	public class UserReportDto
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime CreatedDate { get; set; }
        public MemberDto Reporter { get; set; }
		public MemberDto ReportedUser { get; set; }
        public List<ReportDto> Reports { get; set; }
    }
}
