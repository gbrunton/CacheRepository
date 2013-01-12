namespace CacheRepository.NextIdStrategies
{
	public class IdIsInt : INextIdStrategy
	{
		public dynamic GetNextId(dynamic currentMaxId)
		{
			return currentMaxId + 1 ?? 1;
		}
	}
}