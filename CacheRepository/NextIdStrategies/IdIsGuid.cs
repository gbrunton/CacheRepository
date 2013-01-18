using System;

namespace CacheRepository.NextIdStrategies
{
	public class IdIsGuid : INextIdStrategy
	{
		public dynamic GetNextId(Type type, dynamic currentMaxId)
		{
			return Guid.NewGuid();
		}
	}
}