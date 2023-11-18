namespace API.Entities
{
	public class UserReport
	{
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int ReporterUserId { get; set; }
        public AppUser Reporter { get; set; }
        public int ReportedUserId { get; set; }
        public AppUser ReportedUser { get; set; }
        public List<Report> Reports{ get; set; }

    }
}
