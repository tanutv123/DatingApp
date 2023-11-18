namespace API.Entities
{
	public class ReportType
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Report> Reports { get; set; }
    }
}
