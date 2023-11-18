namespace API.Entities
{
	public class Report
	{
        public int UserReportId { get; set; }
        public UserReport UserReport { get; set; }
		public int ReportTypeId { get; set; }
		public ReportType ReportType{ get; set; }
	}
}
