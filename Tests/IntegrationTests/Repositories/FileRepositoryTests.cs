using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CacheRepository.Configuration.Builders;
using NUnit.Framework;
using Tests.Entities;

namespace Tests.IntegrationTests.Repositories
{
	public class FileRepositoryTests
	{
		[TestFixture]
		public class When_writing_a_reading_an_entity_to_file : FileRepositoryTests
		{
			[Test]
			public void the_entity_can_be_saved_and_retreived_from_the_repository()
			{
				// Arrange
				File.Delete("Blog.txt");

				var config = new FileRepositoryConfigBuilder(".")
					.WithFileExtension(".txt")
					.WithFileDelimitor(",")
					.WithFieldQualifier("\"")
					.Build();
				var blog = new Blog
					{
						Title = "Future Time Saver Log",
						Author = "Gary Brunton",
						CreatedDate = DateTime.Now
					};

				// Act
				using (var repo = config.BuildRepository())
				{
					repo.Insert(blog);
				}

				IEnumerable<Blog> blogs;
				using (var repo = config.BuildRepository())
				{
					blogs = repo.GetAll<Blog>();
				}

				// Assert
				var fromFile = blogs.Single();
				Assert.AreEqual(blog.Id, fromFile.Id);
				Assert.AreEqual(blog.Title, fromFile.Title);
				Assert.AreEqual(blog.Author, fromFile.Author);
				Assert.AreEqual(blog.CreatedDate.ToString("MM/dd/yyyy"), fromFile.CreatedDate.ToString("MM/dd/yyyy"));
				Assert.AreEqual(blog.ModifiedDate, fromFile.ModifiedDate);
			}
		}
	}
}