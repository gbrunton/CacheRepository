using System;
using CacheRepository.OutputConventions;
using NUnit.Framework;
using Tests.Entities;

namespace Tests.UnitTests.OutputConventions
{
	public class DateTimeToyyyyMMddTestFixture
	{
		[TestFixture]
		public class When_determining_if_the_convention_should_be_used : DateTimeToyyyyMMddTestFixture
		{
			[Test]
			public void and_the_type_being_evaluated_is_a_DateTime_then_it_should_be_applied()
			{
				// Arrange
				var blogType = typeof (Blog);
				var sut = new DateTimeToyyyyMMdd();

				// Act
				var shouldBeApplied = sut.ShouldBeApplied(blogType, blogType.GetProperty("CreatedDate"), new Blog());

				// Assert
				Assert.IsTrue(shouldBeApplied);
			}

			[Test]
			public void and_the_type_being_evaluated_is_a_nullable_DateTime_then_it_should_be_applied()
			{
				// Arrange
				var blogType = typeof(Blog);
				var sut = new DateTimeToyyyyMMdd();

				// Act
				var shouldBeApplied = sut.ShouldBeApplied(blogType, blogType.GetProperty("ModifiedDate"), new Blog());

				// Assert
				Assert.IsTrue(shouldBeApplied);
			}

			[Test]
			public void and_the_type_being_evaluated_is_not_a_DateTime_type_then_it_should_not_be_applied()
			{
				// Arrange
				var blogType = typeof(Blog);
				var sut = new DateTimeToyyyyMMdd();

				// Act
				var shouldBeApplied = sut.ShouldBeApplied(blogType, blogType.GetProperty("Title"), new Blog());

				// Assert
				Assert.IsFalse(shouldBeApplied);
			}
		}

		[TestFixture]
		public class When_applying_the_convention : DateTimeToyyyyMMddTestFixture
		{
			[Test]
			public void and_the_value_is_a_DateTime_then_the_value_should_be_formatted()
			{
				// Arrange
				var sut = new DateTimeToyyyyMMdd();
				
				// Act
				var applied = sut.Apply(new DateTime(2013, 1, 1));

				// Assert
				Assert.AreEqual("20130101", applied);
			}

			[Test]
			public void and_the_value_is_a_nullable_DateTime_then_the_value_should_be_formatted()
			{
				// Arrange
				var sut = new DateTimeToyyyyMMdd();
				DateTime? value = new DateTime(2013, 1, 1);

				// Act
				var applied = sut.Apply(value);

				// Assert
				Assert.AreEqual("20130101", applied);
			}

			[Test]
			public void and_the_value_is_a_null_then_the_value_should_be_formatted_as_an_empty_string()
			{
				// Arrange
				var sut = new DateTimeToyyyyMMdd();

				// Act
				var applied = sut.Apply(null);

				// Assert
				Assert.AreEqual("", applied);
			}
		}
	}
}