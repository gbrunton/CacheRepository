using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using CacheRepository.InsertStrategies;
using CacheRepository.OutputConventions;
using CacheRepository.Utils;
using NUnit.Framework;
using Tests.Entities;

namespace Tests.UnitTests.InsertStrategies
{
	public class FileInsertTestFixture
	{
		[TestFixture]
		public class When_Inserting : FileInsertTestFixture
		{
			private FakeConnectionResolver connectionResolver;
			private IEnumerable<IOutputConvention> outputConventions;
			private string delimitor;
			private string fieldQualifier;

			private FileInsert createSut()
			{
				return new FileInsert(this.connectionResolver, this.outputConventions, this.delimitor, this.fieldQualifier);
			}

			[SetUp]
			public void SetUp()
			{
				this.connectionResolver = new FakeConnectionResolver();
				this.outputConventions = new List<IOutputConvention>();
				this.delimitor = string.Empty;
				this.fieldQualifier = string.Empty;
			}

			[Test]
			public void default_property_values_should_make_up_line()
			{
				// Arrange

				// Act
				createSut().Insert(new Blog());

				// Assert
				Assert.AreEqual("0{0}".ToFormat(default(DateTime)), this.connectionResolver.GetLine(), "Should be a 0 because that's the default value of an int");
			}

			[Test]
			public void non_default_property_values_should_make_up_line()
			{
				// Arrange
				var createdDate = DateTime.Now;
				var modifiedDate = createdDate.AddDays(1);

				// Act
				createSut().Insert(new Blog
				{
					Id = 1,
					Title = "Future Time Saver Log",
					Author = "Gary Brunton",
					CreatedDate = createdDate,
					ModifiedDate = modifiedDate
				});

				// Assert
				Assert.AreEqual("1Future Time Saver LogGary Brunton{0}{1}".ToFormat(createdDate, modifiedDate), 
					this.connectionResolver.GetLine());
			}

			[Test]
			public void and_a_delimitor_is_set_it_should_be_used_in_the_line()
			{
				// Arrange
				this.delimitor = ",";

				// Act
				createSut().Insert(new Blog
				{
					Id = 1,
					Title = "Future Time Saver Log",
					Author = "Gary Brunton"
				});

				// Assert
				Assert.AreEqual("1,Future Time Saver Log,Gary Brunton,{0},".ToFormat(default(DateTime)), this.connectionResolver.GetLine());
			}

			[Test]
			public void and_a_delimitor_and_a_field_qualifier_are_set_they_should_be_used_in_the_line()
			{
				// Arrange
				this.delimitor = ",";
				this.fieldQualifier = "\"";

				// Act
				createSut().Insert(new Blog
				{
					Id = 1,
					Title = "Future Time Saver Log",
					Author = "Gary Brunton"
				});

				// Assert
				Assert.AreEqual("\"1\",\"Future Time Saver Log\",\"Gary Brunton\",\"{0}\",\"\"".ToFormat(default(DateTime)), this.connectionResolver.GetLine());
			}

			private class FakeConnectionResolver : IFileConnectionResolver
			{
				private string line;

				public void WriteLine<TEntity>(string newValue)
				{
					this.line = newValue;
				}

				public string GetLine()
				{
					return this.line;
				}
			}
		} 
	}
}