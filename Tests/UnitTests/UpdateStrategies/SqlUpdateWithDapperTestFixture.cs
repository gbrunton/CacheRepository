using CacheRepository.UpdateStrategies;
using NUnit.Framework;
using Tests.Entities;
using Tests.Fakes;

namespace Tests.UnitTests.UpdateStrategies
{
    // These tests are very much dependent on the sql the SqlKata generates which is not ideal. It would be better
    // if the queries were executed against a real db but I'm not going to integrate that right now.
    public class SqlUpdateWithDapperTestFixture
    {
        [TestFixture]
        public class When_updating : SqlUpdateWithDapperTestFixture
        {
            private ExecuteSqlStrategyFake executeSqlStrategy;

            private SqlUpdateWithDapper getSut()
            {
                return new SqlUpdateWithDapper(this.executeSqlStrategy);
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
                sut.Update(blog);

                // Assert
                Assert.AreEqual("UPDATE [Blog] SET [Id] = @p0, [Title] = @p1, [Author] = @p2, [CreatedDate] = @p3, [ModifiedDate] = @p4 WHERE [Id] = @p5", this.executeSqlStrategy.Sql);
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
                sut.Update(blog);

                // Act
                sut.Update(blog);

                // Assert
                Assert.AreEqual("UPDATE [Blog] SET [Id] = @p0, [Title] = @p1, [Author] = @p2, [CreatedDate] = @p3, [ModifiedDate] = @p4 WHERE [Id] = @p5", this.executeSqlStrategy.Sql);
            }
        }
    }
}