using System;

namespace CacheRepository.NextIdStrategies
{
	public class IdDoesNotExist : INextIdStrategy
	{
		public dynamic GetNextId(Type type, dynamic currentMaxId)
		{
			return null;
		}
	}
}