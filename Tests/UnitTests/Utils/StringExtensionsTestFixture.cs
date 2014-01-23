using CacheRepository.Utils;
using NUnit.Framework;

namespace Tests.UnitTests.Utils
{
	public class StringExtensionsTestFixture
	{
		[TestFixture]
		public class When_trimming_the_end : StringExtensionsTestFixture
		{
			[Test]
			public void and_the_field_qualifier_is_empty_then_the_source_should_not_be_changed()
			{
				// Arrange
				
				// Act
				var result = "this is a test".TrimEnd("");

				// Assert
				Assert.AreEqual("this is a test", result);
			}
		} 
	}
}