using System;

namespace CacheRepository.NextIdStrategies
{
	public class IdDoesNotExist : INextIdStrategy
	{
		public dynamic GetNextId(Type type, Func<dynamic> getCurrentMaxId)
		{
			return null;
		}
	}
}