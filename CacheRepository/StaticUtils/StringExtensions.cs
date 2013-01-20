namespace CacheRepository.StaticUtils
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
			return !source.StartsWith(valueToTrim)
				       ? source
				       : source.Remove(0, valueToTrim.Length);
		}
 
		public static string TrimEnd(this string source, string valueToTrim)
		{
			return !source.EndsWith(valueToTrim) 
				? source 
				: source.Remove(source.LastIndexOf(valueToTrim));
		}
	}
}