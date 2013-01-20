using System;
using CacheRepository.StringFormatterAttributes;

namespace Tests.Entities
{
	public class BlogWithAttribute
	{
		public int Id { get; set; }

		[Length(50, Justify.Left)]
		public string Title { get; set; }

		[Length(25, Justify.Right)]
		public string Author { get; set; }

		public DateTime CreatedDate { get; set; }

		[Length(8)]
		public DateTime? ModifiedDate { get; set; }		 
	}
}