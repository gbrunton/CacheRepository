namespace CacheRepository.StringFormatterAttributes
{
	public interface IStringFormatterAttribute
	{
		string Format(object valueAsObj);
	}
}