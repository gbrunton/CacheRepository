using System.IO;
using System.Linq;
using CacheRepository.Configuration.Configs;
using CacheRepository.Repositories;
using CacheRepository.Utils;
using NUnit.Framework;
using Tests.Entities;
using Tests.Fakes;

namespace Tests.UnitTests.Repositories
{
    public class RepositoryTestFixture
    {
        private const string PersistedDatapath = @".\PersistedData";

        [TestFixture]
        public class When_performing_an_ad_hoc_query : RepositoryTestFixture
        {
            private RepositoryConfig reporitoryConfig;

            private Repository getSut()
            {
                return new Repository(this.reporitoryConfig);
            }

            [SetUp]
            public void SetUp()
            {
                this.reporitoryConfig = new RepositoryConfig();
                deletePersistedData();
            }

            [Test]
            public void and_persistent_caching_is_not_enabled_then_the_query_should_be_executed_on_a_subsequent_call()
            {
                // Arrange
                const string query = "Select * From Blog Where Author = 'Gary Brunton';";
                this.reporitoryConfig.QueryStrategy = new QueryStrategyFake
                {
                    Results = new[]
                    {
                        new Blog
                        {
                            Id = 1,
                            Author = "Gary Brunton",
                            Title = "first"
                        }
                    }
                };

                Blog blog;
                using (var repository = getSut())
                {
                    blog = repository.Query<Blog>(query).Single();
                }

                this.reporitoryConfig.QueryStrategy = new QueryStrategyFake
                {
                    Results = new[]
                    {
                        new Blog
                        {
                            Id = 1,
                            Author = "Gary Brunton",
                            Title = "second"
                        }
                    }
                };

                // Act
                Blog blogFromSubsequentCall;
                using (var repository = getSut())
                {
                    blogFromSubsequentCall = repository.Query<Blog>(query).Single();
                }

                // Assert
                Assert.AreEqual(blog.Id, blogFromSubsequentCall.Id);
                Assert.AreNotEqual(blog.Title, blogFromSubsequentCall.Title);
                Assert.AreEqual("first", blog.Title);
                Assert.AreEqual("second", blogFromSubsequentCall.Title);
            }

            [Test]
            public void and_persistent_caching_is_enabled_then_the_query_should_not_be_executed_on_a_subsequent_call()
            {
                // Arrange
                const string query = "Select * From Blog Where Author = 'Gary Brunton';";
                this.reporitoryConfig.PersistedDataPath = PersistedDatapath;
                this.reporitoryConfig.QueryStrategy = new QueryStrategyFake
                {
                    Results = new[]
                    {
                        new Blog
                        {
                            Id = 1,
                            Author = "Gary Brunton",
                            Title = "first"
                        }
                    }
                };

                Blog blog;
                using (var repository = getSut())
                {
                    blog = repository.Query<Blog>(query).Single();
                }

                // The point is that even though this is configured to return a different blog object, since persistent storage is configured
                // it will return the same blog object as before.
                this.reporitoryConfig.QueryStrategy = new QueryStrategyFake
                {
                    Results = new[]
                    {
                        new Blog
                        {
                            Id = 1,
                            Author = "Gary Brunton",
                            Title = "second"
                        }
                    }
                };

                // Act
                Blog blogFromSubsequentCall;
                using (var repository = getSut())
                {
                    blogFromSubsequentCall = repository.Query<Blog>(query).Single();
                }

                // Assert
                Assert.AreEqual(blog.Id, blogFromSubsequentCall.Id);
                Assert.AreEqual(blog.Title, blogFromSubsequentCall.Title);
            }

            [Test]
            public void and_persistent_caching_is_enabled_and_a_different_query_is_executed_for_the_same_object_type_then_the_query_should_be_executed_on_a_subsequent_call()
            {
                // Arrange
                const string query = "Select * From Blog Where Author = 'Gary Brunton';";
                this.reporitoryConfig.PersistedDataPath = PersistedDatapath;
                this.reporitoryConfig.QueryStrategy = new QueryStrategyFake
                {
                    Results = new[]
                    {
                        new Blog
                        {
                            Id = 1,
                            Author = "Gary Brunton",
                            Title = "first"
                        }
                    }
                };

                Blog blog;
                using (var repository = getSut())
                {
                    blog = repository.Query<Blog>(query).Single();
                }

                // The point is that even though this is configured to return a different blog object, since persistent storage is configured
                // it will return the same blog object as before.
                this.reporitoryConfig.QueryStrategy = new QueryStrategyFake
                {
                    Results = new[]
                    {
                        new Blog
                        {
                            Id = 1,
                            Author = "Gary Brunton",
                            Title = "second"
                        }
                    }
                };

                // Act
                Blog blogFromSubsequentCall;
                using (var repository = getSut())
                {
                    blogFromSubsequentCall = repository.Query<Blog>("a different query").Single();
                }

                // Assert
                Assert.AreEqual(blog.Id, blogFromSubsequentCall.Id);
                Assert.AreNotEqual(blog.Title, blogFromSubsequentCall.Title);
                Assert.AreEqual(blogFromSubsequentCall.Title, "second");
            }

            [Test]
            public void and_persistent_caching_is_enabled_and_a_different_query_parameter_is_used_for_the_same_object_type_and_query_text_then_the_query_should_be_executed_on_a_subsequent_call()
            {
                // Arrange
                const string query = "Select * From Blog Where Author = 'Gary Brunton';";
                var parametersForQuery1 = new {Param = "test1"};
                var parametersForQuery2 = new { Param = "test2" };
                this.reporitoryConfig.PersistedDataPath = PersistedDatapath;
                this.reporitoryConfig.QueryStrategy = new QueryStrategyFake
                {
                    Results = new[]
                    {
                        new Blog
                        {
                            Id = 1,
                            Author = "Gary Brunton",
                            Title = "first"
                        }
                    }
                };

                Blog blog;
                using (var repository = getSut())
                {
                    blog = repository.Query<Blog>(query, parametersForQuery1).Single();
                }

                // The point is that even though this is configured to return a different blog object, since persistent storage is configured
                // it will return the same blog object as before.
                this.reporitoryConfig.QueryStrategy = new QueryStrategyFake
                {
                    Results = new[]
                    {
                        new Blog
                        {
                            Id = 1,
                            Author = "Gary Brunton",
                            Title = "second"
                        }
                    }
                };

                // Act
                Blog blogFromSubsequentCall;
                using (var repository = getSut())
                {
                    blogFromSubsequentCall = repository.Query<Blog>(query, parametersForQuery2).Single();
                }

                // Assert
                Assert.AreEqual(blog.Id, blogFromSubsequentCall.Id);
                Assert.AreNotEqual(blog.Title, blogFromSubsequentCall.Title);
                Assert.AreEqual(blogFromSubsequentCall.Title, "second");
            }

            [Test]
            public void and_persistent_caching_is_enabled_multiple_data_files_should_not_be_created_for_the_same_key()
            {
                // Arrange
                const string query = "Select * From Blog Where Author = 'Gary Brunton';";
                this.reporitoryConfig.PersistedDataPath = PersistedDatapath;
                this.reporitoryConfig.QueryStrategy = new QueryStrategyFake
                {
                    Results = new[]
                    {
                        new Blog
                        {
                            Id = 1,
                            Author = "Gary Brunton",
                            Title = "first"
                        }
                    }
                };

                Blog blog;
                using (var repository = getSut())
                {
                    blog = repository.Query<Blog>(query).Single();
                }

                // Act
                Blog blogFromSubsequentCall;
                using (var repository = getSut())
                {
                    blogFromSubsequentCall = repository.Query<Blog>(query).Single();
                }

                // Assert
                Assert.AreEqual(blog.Id, blogFromSubsequentCall.Id);
                Assert.AreEqual(blog.Title, blogFromSubsequentCall.Title);
                Assert.AreEqual(1, Directory.GetFiles(Path.Combine(PersistedDatapath, "Queries")).Length);
            }

            private static void deletePersistedData()
            {
                Directory.GetFiles(PersistedDatapath).Each(File.Delete);
                Directory.GetDirectories(PersistedDatapath).Each(x => Directory.Delete(x, true));
            }
        }
    }
}