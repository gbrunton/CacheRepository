using System;
using CacheRepository.NextIdStrategies;
using NUnit.Framework;

namespace Tests.UnitTests
{
	public class SmartNextIdRetreiverTestFixture
	{
		[TestFixture]
		public class When_getting_the_next_Id : SmartNextIdRetreiverTestFixture
		{
			[Test]
			public void and_an_id_property_cannot_be_found_then_a_null_should_be_returned()
			{
				// Arrange
				
				// Act
				var nextId = new SmartNextIdRetreiver().GetNextId(typeof(BlogWithNoPropertyId), 1);

				// Assert
				Assert.IsNull(nextId);
			}

			private class BlogWithNoPropertyId
			{
			}

			[Test]
			public void and_an_id_property_is_found_but_no_action_has_been_defined_for_the_type_then_an_exception_should_be_thrown()
			{
				// Arrange

				// Act, Assert
				Assert.Throws<Exception>(() => new SmartNextIdRetreiver().GetNextId(typeof(BlogWithIdWithUnknownType), null));
			}

			private class BlogWithIdWithUnknownType
			{
				public bool Id { get; set; }
			}

			[TestCase(null, 1)]
			[TestCase(1, 2)]
			public void and_an_id_property_is_found_of_type_int_then_the_next_increment_int_value_should_be_returned(int? currentMaxId, int expectedId)
			{
				// Arrange

				// Act
				var nextId = new SmartNextIdRetreiver().GetNextId(typeof(BlogWithIdWithIntType), currentMaxId);

				//Assert
				Assert.AreEqual(expectedId, nextId);
			}

			private class BlogWithIdWithIntType
			{
				public int Id { get; set; }
			}

			[TestCase(null, 1L)]
			[TestCase(1L, 2L)]
			public void and_an_id_property_is_found_of_type_long_then_the_next_increment_long_value_should_be_returned(long? currentMaxId, long expectedId)
			{
				// Arrange

				// Act
				var nextId = new SmartNextIdRetreiver().GetNextId(typeof(BlogWithIdWithLongType), currentMaxId);

				//Assert
				Assert.AreEqual(expectedId, nextId);
			}

			private class BlogWithIdWithLongType
			{
				public long Id { get; set; }
			}

			[TestCase(null, "BlogWithIdWithStringType-1")]
			[TestCase("BlogWithIdWithStringType-1", "BlogWithIdWithStringType-2")]
			public void and_an_id_property_is_found_of_type_string_then_the_next_increment_string_value_should_be_returned(string currentMaxId, string expectedId)
			{
				// Arrange

				// Act
				var nextId = new SmartNextIdRetreiver().GetNextId(typeof(BlogWithIdWithStringType), currentMaxId);

				//Assert
				Assert.AreEqual(expectedId, nextId);
			}

			private class BlogWithIdWithStringType
			{
				public string Id { get; set; }
			}

			[TestCase(true)]
			[TestCase(false)]
			public void and_an_id_property_is_found_of_type_guid_then_the_next_guid_value_should_be_returned(bool isCurrentIdNull)
			{
				// Arrange
				var currentMaxId = isCurrentIdNull ? null as Guid? : Guid.NewGuid();

				// Act
				var nextId = new SmartNextIdRetreiver().GetNextId(typeof(BlogWithIdWithGuidType), currentMaxId);

				//Assert
				Assert.IsNotNull(nextId);
				Assert.AreNotEqual(currentMaxId, nextId);
			}

			private class BlogWithIdWithGuidType
			{
				public Guid Id { get; set; }
			}
		} 
	}
}