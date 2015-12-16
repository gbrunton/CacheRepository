using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CacheRepository.Configuration.Builders;
using CacheRepository.OutputConventions;
using CacheRepository.Utils;
using NUnit.Framework;
using Tests.Entities;
using Tests.Indexes;

namespace Tests.IntegrationTests.Repositories
{
	public class FileRepositoryTests
	{
		private const string TestFileExtension = ".test.txt";

	    [TestFixture]
	    public class When_persisting_data : FileRepositoryTests
	    {
	        [Test]
            public void the_data_can_be_persisted_and_retrieved()
	        {
	            // Arrange
                deleteTestFiles();

	            const string persistedDatapath = @".\PersistedData";
                Directory.CreateDirectory(persistedDatapath);
                Directory.GetFiles(persistedDatapath).Each(File.Delete);

                var config = new FileRepositoryConfigBuilder(".")
                    .WithFileExtension(TestFileExtension)
                    .WithFileDelimitor(",")
                    .WithFieldQualifier("\"")
                    .WithPersistedDataPath(persistedDatapath)
                    .WithIndexes(new BlogByAuthorIndex())
                    .Build();
                var blog = new Blog
                {
                    Title = "Future Time Saver Log",
                    Author = "Gary Brunton",
                    CreatedDate = DateTime.Now
                };

	            var blogWithStringOnlyProperties = new BlogWithStringOnlyProperties
	            {
	                Author = "Simon",
	                CreatedDate = "1/1/2010",
	                Id = 100,
	                ModifiedDate = "2/1/2010",
	                Title = "This is a test"
	            };

	            // Act
                using (var repo = config.BuildRepository())
                {
                    repo.Insert(blog);
                    repo.Insert(blogWithStringOnlyProperties);
                }

                deleteTestFiles();

                IEnumerable<Blog> blogs;
                IEnumerable<BlogWithStringOnlyProperties> blogsWithStringOnlyProperties;
                using (var repo = config.BuildRepository())
                {
                    blogs = repo.GetAll<Blog>();
                    blogsWithStringOnlyProperties = repo.GetAll<BlogWithStringOnlyProperties>();
                }

                // Assert
                var fromFile = blogs.Single();
                Assert.AreEqual(blog.Id, fromFile.Id);
                Assert.AreEqual(blog.Title, fromFile.Title);
                Assert.AreEqual(blog.Author, fromFile.Author);
                Assert.AreEqual(blog.CreatedDate.ToString("MM/dd/yyyy"), fromFile.CreatedDate.ToString("MM/dd/yyyy"));
                Assert.AreEqual(blog.ModifiedDate, fromFile.ModifiedDate);

                var fromFile2 = blogsWithStringOnlyProperties.Single();
                Assert.AreEqual(blogWithStringOnlyProperties.Id, fromFile2.Id);
                Assert.AreEqual(blogWithStringOnlyProperties.Title, fromFile2.Title);
                Assert.AreEqual(blogWithStringOnlyProperties.Author, fromFile2.Author);
                Assert.AreEqual(blogWithStringOnlyProperties.CreatedDate, fromFile2.CreatedDate);
                Assert.AreEqual(blogWithStringOnlyProperties.ModifiedDate, fromFile2.ModifiedDate);
            }

	        [Test]
            public void the_data_can_be_persisted_and_more_data_can_be_inserted_correctly()
	        {
                // Arrange
                deleteTestFiles();

                const string persistedDatapath = @".\PersistedData";
                Directory.CreateDirectory(persistedDatapath);
                Directory.GetFiles(persistedDatapath).Each(File.Delete);

                var config = new FileRepositoryConfigBuilder(".")
                    .WithFileExtension(TestFileExtension)
                    .WithFileDelimitor(",")
                    .WithFieldQualifier("\"")
                    .WithPersistedDataPath(persistedDatapath)
                    .WithIndexes(new BlogByAuthorIndex())
                    .Build();
                var blog = new Blog
                {
                    Title = "Future Time Saver Log",
                    Author = "Gary Brunton",
                    CreatedDate = DateTime.Now
                };

                using (var repo = config.BuildRepository())
                {
                    repo.Insert(blog);
                }

                // Act
	            config = new FileRepositoryConfigBuilder(".")
	                .WithFileExtension(TestFileExtension)
	                .WithFileDelimitor(",")
	                .WithFieldQualifier("\"")
	                .WithPersistedDataPath(persistedDatapath)
	                .WithIndexes(new BlogByAuthorIndex())
	                .Build();

                IEnumerable<Blog> blogs;
                using (var repo = config.BuildRepository())
                {
                    repo.Insert(blog);
                    blogs = repo.GetAll<Blog>();
                }

                // Assert
                Assert.AreEqual(2, blogs.Count());

                deleteTestFiles();

                using (var repo = config.BuildRepository())
                {
                    blogs = repo.GetAll<Blog>();
                }

                Assert.AreEqual(2, blogs.Count());
            }
	    }

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
		public class When_applying_a_string_format_attribute : FileRepositoryTests
		{
			[Test]
			public void the_formatting_should_be_applied()
			{
				// Arrange
				deleteTestFiles();

				var config = new FileRepositoryConfigBuilder(".")
					.WithFileExtension(TestFileExtension)
					.WithFileDelimitor(",")
					.WithFieldQualifier("\"")
					.Build();
				var blog = new BlogWithAttribute
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

				File.Copy("BlogWithAttribute" + TestFileExtension, "BlogWithStringOnlyProperties" + TestFileExtension);

				IEnumerable<BlogWithStringOnlyProperties> blogs;
				using (var repo = config.BuildRepository())
				{
					blogs = repo.GetAll<BlogWithStringOnlyProperties>();
				}

				// Assert
				var fromFile = blogs.Single();
				Assert.AreEqual(blog.Id, fromFile.Id);
				Assert.AreEqual(blog.Title.PadRight(50), fromFile.Title);
				Assert.AreEqual(blog.Author.PadLeft(25), fromFile.Author);
				Assert.AreEqual("        ", fromFile.ModifiedDate);
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
					.WithOutputConventions(new DateTimeToyyyyMMdd())
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

		[TestFixture]
		public class When_writing_to_a_file_that_already_exists : FileRepositoryTests
		{
			[Test]
			public void inserts_should_be_allowed()
			{
				// Arrange
				deleteTestFiles();
				var configBuilder = new FileRepositoryConfigBuilder(".")
					.WithFileExtension(TestFileExtension)
					.WithFileDelimitor(",")
					.WithFieldQualifier("\"");
				var config = configBuilder.Build();
				var blog = new Blog
				{
					Title = "Future Time Saver Log",
					Author = "Gary Brunton",
					CreatedDate = DateTime.Now
				};

				using (var repo = config.BuildRepository())
				{
					repo.Insert(blog);
				}

				// Act
				configBuilder = new FileRepositoryConfigBuilder(".")
					.WithFileExtension(TestFileExtension)
					.WithFileDelimitor(",")
					.WithFieldQualifier("\"");
				using (var repo = configBuilder.Build().BuildRepository())
				{
					repo.Insert(blog);
				}

				// Assert
				IEnumerable<Blog> blogsFromFile;
				configBuilder = new FileRepositoryConfigBuilder(".")
					.WithFileExtension(TestFileExtension)
					.WithFileDelimitor(",")
					.WithFieldQualifier("\"");
				using (var repo = configBuilder.Build().BuildRepository())
				{
					blogsFromFile = repo.GetAll<Blog>();
				}

				Assert.AreEqual(2, blog.Id);
				Assert.AreEqual(2, blogsFromFile.Count());
				Assert.IsTrue(blogsFromFile.Any(x => x.Id == 1));
				Assert.IsTrue(blogsFromFile.Any(x => x.Id == 2));
			}
		}

		private static void deleteTestFiles()
		{
			Directory
				.GetFiles(".", "*" + TestFileExtension)
                .Union(Directory.GetFiles(".", "*.dat"))
				.Each(File.Delete);
		}
	}
}