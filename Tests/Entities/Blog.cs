using System;

namespace Tests.Entities
{
	public class Blog
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
	}
}