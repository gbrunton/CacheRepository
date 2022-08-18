using CacheRepository.InsertStrategies;
using NUnit.Framework;
using Tests.Entities;
using Tests.Fakes;

namespace Tests.UnitTests.InsertStrategies
{
    // These tests are very much dependent on the sql the SqlKata generates which is not ideal. It would be better
    // if the queries were executed against a real db but I'm not going to integrate that right now.
    public class SqlInsertWithDapperTestFixture
    {
        [TestFixture]
        public class When_inserting : SqlInsertWithDapperTestFixture
        {
            private ExecuteSqlStrategyFake executeSqlStrategy;

            private SqlInsertWithDapper getSut()
            {
                return new SqlInsertWithDapper(this.executeSqlStrategy);
            }

            [SetUp]
            public void Setup()
            {
                this.executeSqlStrategy = new ExecuteSqlStrategyFake();
            }

            [Test]
            public void the_sql_is_generated_correctly()
            {
                // Arrange
                var sut = this.getSut();
                var blog = new Blog
                {
                    Id = 1,
                    Author = "Gary Brunton",
                    Title = "Update Test"
                };

                // Act
                sut.Insert(blog);

                // Assert
                Assert.AreEqual("INSERT INTO [Blog] ([Id], [Title], [Author], [CreatedDate], [ModifiedDate]) VALUES (@p0, @p1, @p2, @p3, @p4)", this.executeSqlStrategy.Sql);
            }

            [Test]
            public void and_multiple_updates_are_called_the_sql_is_generated_correctly()
            {
                // Arrange
                var sut = this.getSut();
                var blog = new Blog
                {
                    Id = 1,
                    Author = "Gary Brunton",
                    Title = "Update Test"
                };
                sut.Insert(blog);

                // Act
                sut.Insert(blog);

                // Assert
                Assert.AreEqual("INSERT INTO [Blog] ([Id], [Title], [Author], [CreatedDate], [ModifiedDate]) VALUES (@p0, @p1, @p2, @p3, @p4)", this.executeSqlStrategy.Sql);
            }
        }
    }
}