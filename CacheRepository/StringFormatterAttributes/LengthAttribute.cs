using System;

namespace CacheRepository.StringFormatterAttributes
{
	public enum Justify
	{
		Left,
		Right
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class LengthAttribute : Attribute, IStringFormatterAttribute
	{
		private readonly int length;
		private readonly Justify justify;

		public LengthAttribute(int length)
		{
			this.length = length;
			this.justify = Justify.Left;
		}

		public LengthAttribute(int length, Justify justify)
		{
			this.length = length;
			this.justify = justify;
		}

		public string Format(object valueAsObj)
		{
			var value = valueAsObj == null
				            ? ""
				            : valueAsObj.ToString();
			return this.justify == Justify.Left 
				? value.PadRight(this.length)
				: value.PadLeft(this.length);
		}
	}
}