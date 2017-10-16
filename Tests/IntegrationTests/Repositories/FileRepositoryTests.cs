using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CacheRepository.Configuration.Builders;
using CacheRepository.Configuration.Configs;
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
        private const string PersistedDatapath = @".\PersistedData";

	    [TestFixture]
	    public class When_persisting_data : FileRepositoryTests
	    {
	        [SetUp]
	        public void SetUp()
	        {
                Directory.CreateDirectory(PersistedDatapath);
                deletePersistedData();
	        }

	        [Test]
            public void the_data_can_be_persisted_and_retrieved()
	        {
	            // Arrange
                deleteTestFiles();

                var config = new FileRepositoryConfigBuilder(".")
                    .WithFileExtension(TestFileExtension)
                    .WithFileDelimitor(",")
                    .WithFieldQualifier("\"")
                    .WithPersistedDataPath(PersistedDatapath)
                    .WithIndexes(new BlogsByAuthorIndex())
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
	        public void the_data_can_be_persisted_and_an_Index_and_Entity_change_will_not_be_persisted_back_to_file()
	        {
	            // Arrange
	            deleteTestFiles();

	            FileRepositoryConfig getConfig() => new FileRepositoryConfigBuilder(".")
	                .WithFileExtension(TestFileExtension)
	                .WithFileDelimitor(",")
	                .WithFieldQualifier("\"")
	                .WithPersistedDataPath(PersistedDatapath, PersistedDataAccess.ReadOnly)
	                .WithIndexes(new BlogByIdIndex())
	                .Build();
	            var blog = new Blog
	            {
	                Title = "Future Time Saver Log",
	                Author = "Gary Brunton",
	                CreatedDate = DateTime.Now
	            };

	            // Act
	            using (var repo = getConfig().BuildRepository())
	            {
	                repo.Insert(blog);
	            }

	            deleteTestFiles();

	            Blog blogFromIndex;
	            Blog blogFromRepository;
	            using (var repo = getConfig().BuildRepository())
	            {
	                blogFromIndex = repo.GetIndex<BlogByIdIndex>().Get(blog.Id);
	                blogFromRepository = repo.GetAll<Blog>().Single();
	                blogFromIndex.Author = "Not Gary Brunton";
	                blogFromRepository.Author = "Still Not Gary Brunton";
	            }

	            deleteTestFiles();

	            using (var repo = getConfig().BuildRepository())
	            {
	                blogFromIndex = repo.GetIndex<BlogByIdIndex>().Get(blog.Id);
	                blogFromRepository = repo.GetAll<Blog>().Single();
	            }

                // Assert
                Assert.AreEqual(blog.Id, blogFromIndex.Id);
	            Assert.AreEqual(blog.Title, blogFromIndex.Title);
	            Assert.AreEqual(blog.Author, blogFromIndex.Author);
	            Assert.AreEqual(blog.CreatedDate.ToString("MM/dd/yyyy"), blogFromIndex.CreatedDate.ToString("MM/dd/yyyy"));
	            Assert.AreEqual(blog.ModifiedDate, blogFromIndex.ModifiedDate);

	            Assert.AreEqual(blog.Id, blogFromRepository.Id);
	            Assert.AreEqual(blog.Title, blogFromRepository.Title);
	            Assert.AreEqual(blog.Author, blogFromRepository.Author);
	            Assert.AreEqual(blog.CreatedDate.ToString("MM/dd/yyyy"), blogFromRepository.CreatedDate.ToString("MM/dd/yyyy"));
	            Assert.AreEqual(blog.ModifiedDate, blogFromRepository.ModifiedDate);
            }

            [Test]
            public void the_data_can_be_persisted_and_when_retrieved_the_indexes_reference_instances_within_the_entities()
            {
                // Arrange
                deleteTestFiles();

                FileRepositoryConfig getConfig() => 
                    new FileRepositoryConfigBuilder(".")
                    .WithFileExtension(TestFileExtension)
                    .WithFileDelimitor(",")
                    .WithFieldQualifier("\"")
                    .WithPersistedDataPath(PersistedDatapath, PersistedDataAccess.ReadOnly)
                    .WithIndexes(new BlogByIdIndex())
                    .Build();

                var blog = new Blog
                {
                    Title = "Future Time Saver Log",
                    Author = "Gary Brunton",
                    CreatedDate = DateTime.Now
                };

                // Act
                using (var repo = getConfig().BuildRepository())
                {
                    repo.Insert(blog);
                }

                deleteTestFiles();

                Blog blogFromIndex;
                Blog blogFromRepository;
                using (var repo = getConfig().BuildRepository())
                {
                    blogFromIndex = repo.GetIndex<BlogByIdIndex>().Get(blog.Id);
                    blogFromRepository = repo.GetAll<Blog>().Single();
                }

                blogFromRepository.Author = "Changed Name";

                // Assert
                Assert.AreEqual("Changed Name", blogFromRepository.Author);
                Assert.AreEqual("Changed Name", blogFromIndex.Author);
            }

            [Test]
            public void the_data_can_be_persisted_and_a_NonUniqueIndex_can_be_retrieved()
            {
                // Arrange
                deleteTestFiles();

                FileRepositoryConfig getConfig() =>
                    new FileRepositoryConfigBuilder(".")
                    .WithFileExtension(TestFileExtension)
                    .WithFileDelimitor(",")
                    .WithFieldQualifier("\"")
                    .WithPersistedDataPath(PersistedDatapath)
                    .WithIndexes(new BlogsByAuthorIndex())
                    .Build();
                var blog = new Blog
                {
                    Title = "Future Time Saver Log",
                    Author = "Gary Brunton",
                    CreatedDate = DateTime.Now
                };

                // Act
                using (var repo = getConfig().BuildRepository())
                {
                    repo.Insert(blog);
                }

                deleteTestFiles();

                IEnumerable<Blog> blogs;
                using (var repo = getConfig().BuildRepository())
                {
                    blogs = repo.GetIndex<BlogsByAuthorIndex>().Get(blog.Author);
                }

                // Assert
                var fromIndex = blogs.Single();
                Assert.AreEqual(blog.Id, fromIndex.Id);
                Assert.AreEqual(blog.Title, fromIndex.Title);
                Assert.AreEqual(blog.Author, fromIndex.Author);
                Assert.AreEqual(blog.CreatedDate.ToString("MM/dd/yyyy"), fromIndex.CreatedDate.ToString("MM/dd/yyyy"));
                Assert.AreEqual(blog.ModifiedDate, fromIndex.ModifiedDate);
            }

            [Test]
            public void the_data_can_be_persisted_and_a_UniqueIndex_can_be_retrieved()
            {
                // Arrange
                deleteTestFiles();

                var config = new FileRepositoryConfigBuilder(".")
                    .WithFileExtension(TestFileExtension)
                    .WithFileDelimitor(",")
                    .WithFieldQualifier("\"")
                    .WithPersistedDataPath(PersistedDatapath)
                    .WithIndexes(new BlogByIdIndex())
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

                deleteTestFiles();

                Blog blogFromIndex;
                using (var repo = config.BuildRepository())
                {
                    blogFromIndex = repo.GetIndex<BlogByIdIndex>().Get(blog.Id);
                }

                // Assert
                Assert.AreEqual(blog.Id, blogFromIndex.Id);
                Assert.AreEqual(blog.Title, blogFromIndex.Title);
                Assert.AreEqual(blog.Author, blogFromIndex.Author);
                Assert.AreEqual(blog.CreatedDate.ToString("MM/dd/yyyy"), blogFromIndex.CreatedDate.ToString("MM/dd/yyyy"));
                Assert.AreEqual(blog.ModifiedDate, blogFromIndex.ModifiedDate);
            }

	        [Test]
	        public void the_data_can_be_persisted_and_a_UniqueIndex_result_can_be_null()
	        {
	            // Arrange
	            deleteTestFiles();

	            var config = new FileRepositoryConfigBuilder(".")
	                .WithFileExtension(TestFileExtension)
	                .WithFileDelimitor(",")
	                .WithFieldQualifier("\"")
	                .WithPersistedDataPath(PersistedDatapath)
	                .WithIndexes(new BlogByIdIndex())
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

	            deleteTestFiles();

	            Blog blogFromIndex;
	            using (var repo = config.BuildRepository())
	            {
	                blogFromIndex = repo.GetIndex<BlogByIdIndex>().Get(blog.Id + 1);
	            }

	            // Assert
	            Assert.IsNull(blogFromIndex);
	        }

            [Test]
	        public void the_data_can_be_persisted_and_a_UniqueIndex_can_be_retrieved_even_if_there_are_multiple_keys_for_the_unique_index()
	        {
	            // Arrange
	            deleteTestFiles();

	            var config = new FileRepositoryConfigBuilder(".")
	                .WithFileExtension(TestFileExtension)
	                .WithFileDelimitor(",")
	                .WithFieldQualifier("\"")
	                .WithPersistedDataPath(PersistedDatapath)
	                .WithIndexes(new BlogByIdTitle())
	                .Build();
	            var blog1 = new Blog
	            {
	                Title = "Future Time Saver Log",
	                Author = "Gary Brunton 2",
	                CreatedDate = DateTime.Now.AddDays(-1)
	            };
	            var blog2 = new Blog
	            {
	                Title = "Future Time Saver Log",
	                Author = "Gary Brunton 2",
	                CreatedDate = DateTime.Now
	            };

                // Act
                using (var repo = config.BuildRepository())
	            {
	                repo.Insert(blog1);
	                repo.Insert(blog2);
	            }

                deleteTestFiles();

	            Blog blogFromIndex;
	            using (var repo = config.BuildRepository())
	            {
	                blogFromIndex = repo.GetIndex<BlogByIdTitle>().Get(blog1.Title);
	            }

	            // Assert
	            Assert.AreEqual(blog1.Id, blogFromIndex.Id);
	            Assert.AreEqual(blog1.Title, blogFromIndex.Title);
	            Assert.AreEqual(blog1.Author, blogFromIndex.Author);
	            Assert.AreEqual(blog1.CreatedDate.ToString("MM/dd/yyyy"), blogFromIndex.CreatedDate.ToString("MM/dd/yyyy"));
	            Assert.AreEqual(blog1.ModifiedDate, blogFromIndex.ModifiedDate);
	        }

            [Test]
            public void the_data_can_be_persisted_and_more_data_can_be_inserted_correctly()
	        {
                // Arrange
                deleteTestFiles();

                var config = new FileRepositoryConfigBuilder(".")
                    .WithFileExtension(TestFileExtension)
                    .WithFileDelimitor(",")
                    .WithFieldQualifier("\"")
                    .WithPersistedDataPath(PersistedDatapath)
                    .WithIndexes(new BlogsByAuthorIndex())
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
                    .WithPersistedDataPath(PersistedDatapath)
	                .WithIndexes(new BlogsByAuthorIndex())
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

            [Test]
            public void empty_strings_can_be_persisted_and_retrieved_as_empty_strings()
            {
                // Arrange
                deleteTestFiles();

                var config = new FileRepositoryConfigBuilder(".")
                    .WithFileExtension(TestFileExtension)
                    .WithFileDelimitor(",")
                    .WithFieldQualifier("\"")
                    .WithPersistedDataPath(PersistedDatapath)
                    .WithIndexes(new BlogsByAuthorIndex())
                    .Build();
                var blog = new Blog
                {
                    Title = null,
                    Author = "",
                    CreatedDate = DateTime.Now
                };

                using (var repo = config.BuildRepository())
                {
                    repo.Insert(blog);
                }

                deleteTestFiles();

                // Act
                Blog blogFromPersistantStore;
                using (var repo = config.BuildRepository())
                {
                    blogFromPersistantStore = repo.GetAll<Blog>().Single();
                }

                // Assert
                Assert.AreEqual(blog.Id, blogFromPersistantStore.Id);
                Assert.AreEqual(blog.Title, blogFromPersistantStore.Title);
                Assert.AreEqual(blog.Author, blogFromPersistantStore.Author);
                Assert.AreEqual(blog.CreatedDate.ToString("MM/dd/yyyy"), blogFromPersistantStore.CreatedDate.ToString("MM/dd/yyyy"));
                Assert.AreEqual(blog.ModifiedDate, blogFromPersistantStore.ModifiedDate);

                Assert.IsTrue(blogFromPersistantStore.Title == null);
                Assert.IsTrue(blogFromPersistantStore.Author == "");
            }

	        [Test]
	        public void the_data_can_be_persisted_and_a_NonUniqueIndex_complex_index_can_be_retrieved()
	        {
                // Arrange
                deleteTestFiles();

	            var config = new FileRepositoryConfigBuilder(".")
	                .WithFileExtension(TestFileExtension)
	                .WithFileDelimitor(",")
	                .WithFieldQualifier("\"")
	                .WithPersistedDataPath(PersistedDatapath)
	                .WithIndexes(new BlogsByIdAndAuthorIndex())
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

	            deleteTestFiles();

	            IEnumerable<Blog> blogs;
	            using (var repo = config.BuildRepository())
	            {
	                blogs = repo.GetIndex<BlogsByIdAndAuthorIndex>().Get(new IndexKey(blog.Id, blog.Author));
	            }

	            // Assert
	            var fromIndex = blogs.Single();
	            Assert.AreEqual(blog.Id, fromIndex.Id);
	            Assert.AreEqual(blog.Title, fromIndex.Title);
	            Assert.AreEqual(blog.Author, fromIndex.Author);
	            Assert.AreEqual(blog.CreatedDate.ToString("MM/dd/yyyy"), fromIndex.CreatedDate.ToString("MM/dd/yyyy"));
	            Assert.AreEqual(blog.ModifiedDate, fromIndex.ModifiedDate);
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

        private static void deletePersistedData()
        {
            Directory.GetFiles(PersistedDatapath).Each(File.Delete);
            Directory.GetDirectories(PersistedDatapath).Each(x => Directory.Delete(x, true));
        }
	}
}