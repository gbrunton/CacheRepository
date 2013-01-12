namespace CacheRepository.NextIdStrategies
{
	public interface INextIdStrategy
	{
		dynamic GetNextId(dynamic currentMaxId);
	}
}