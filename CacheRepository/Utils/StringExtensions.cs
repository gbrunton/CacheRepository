using System;

namespace CacheRepository.Utils
{
	public static class StringExtensions
	{
		public static string Trim(this string source, string valueToTrim)
		{
			return source
				.TrimStart(valueToTrim)
				.TrimEnd(valueToTrim);
		}

		public static string TrimStart(this string source, string valueToTrim)
		{
			if (string.IsNullOrEmpty(source)) return source;
			return !source.StartsWith(valueToTrim)
				       ? source
				       : source.Remove(0, valueToTrim.Length);
		}
 
		public static string TrimEnd(this string source, string valueToTrim)
		{
			if (string.IsNullOrEmpty(source)) return source;
			return !source.EndsWith(valueToTrim) 
				? source 
				: source.Remove(source.LastIndexOf(valueToTrim));
		}

		public static string ToFormat(this string stringFormat, params object[] args)
		{
			return String.Format(stringFormat, args);
		}
	}
}