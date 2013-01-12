namespace CacheRepository.NextIdStrategies
{
	public class IdDoesNotExist : INextIdStrategy
	{
		public dynamic GetNextId(dynamic currentMaxId)
		{
			return null;
		}
	}
}