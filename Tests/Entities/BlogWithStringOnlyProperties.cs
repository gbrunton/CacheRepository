namespace Tests.Entities
{
	public class BlogWithStringOnlyProperties
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string CreatedDate { get; set; }
		public string ModifiedDate { get; set; }
	}
}