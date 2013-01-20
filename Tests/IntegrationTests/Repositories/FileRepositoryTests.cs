using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CacheRepository.Configuration.Builders;
using CacheRepository.OutputConventions;
using NUnit.Framework;
using Tests.Entities;

namespace Tests.IntegrationTests.Repositories
{
	public class FileRepositoryTests
	{
		private const string TestFileExtension = ".test.txt";
		[TestFixture]
		public class When_writing_and_reading_an_entity_to_file : FileRepositoryTests
		{
			[Test]
			public void the_entity_can_be_saved_and_retreived_from_the_repository()
			{
				// Arrange
				deleteTestFiles();

				var config = new FileRepositoryConfigBuilder(".")
					.WithFileExtension(TestFileExtension)
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

		[TestFixture]
		public class When_declaring_output_conventions : FileRepositoryTests
		{
			[Test]
			public void the_matching_property_values_should_be_formated_per_the_convention()
			{
				// Arrange
				deleteTestFiles();

				var config = new FileRepositoryConfigBuilder(".")
					.WithFileExtension(TestFileExtension)
					.WithFileDelimitor(",")
					.WithFieldQualifier("\"")
					.WithOutputConventsions(new DateTimeToyyyyMMdd())
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

				File.Copy("Blog" + TestFileExtension, "BlogWithStringOnlyProperties" + TestFileExtension);

				IEnumerable<BlogWithStringOnlyProperties> blogs;
				using (var repo = config.BuildRepository())
				{
					blogs = repo.GetAll<BlogWithStringOnlyProperties>();
				}

				// Assert
				var fromFile = blogs.Single();
				Assert.AreEqual(blog.Id, fromFile.Id);
				Assert.AreEqual(blog.Title, fromFile.Title);
				Assert.AreEqual(blog.Author, fromFile.Author);
				Assert.AreEqual(blog.CreatedDate.ToString("yyyyMMdd"), fromFile.CreatedDate);
				Assert.AreEqual(blog.ModifiedDate, fromFile.ModifiedDate);
			}
		}

		private static void deleteTestFiles()
		{
			Directory
				.GetFiles(".", "*" + TestFileExtension)
				.Each(File.Delete);
		}
	}
}