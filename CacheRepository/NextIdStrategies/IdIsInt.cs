using System;

namespace CacheRepository.NextIdStrategies
{
	public class IdIsInt : INextIdStrategy
	{
		public dynamic GetNextId(Type type, dynamic currentMaxId)
		{
			return currentMaxId + 1 ?? 1;
		}
	}
}