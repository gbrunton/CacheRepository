using System;

namespace CacheRepository.NextIdStrategies
{
	public class IdIsInt : INextIdStrategy
	{
		public dynamic GetNextId(Type type, Func<dynamic> getCurrentMaxId)
		{
			return getCurrentMaxId() + 1 ?? 1;
		}
	}
}