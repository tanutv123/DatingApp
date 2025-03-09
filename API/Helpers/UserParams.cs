namespace API.Helpers
{
	public class UserParams : PaginationParams
	{
		public string CurrentUsername { get; set; }
        public string Gender { get; set; }
		public int minAge { get; set; } = 18;
		public int maxAge { get; set; } = 50;
        public string OrderBy { get; set; }

    }
}
