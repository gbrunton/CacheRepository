using System;

namespace CacheRepository.NextIdStrategies
{
	public interface INextIdStrategy
	{
		dynamic GetNextId(Type type, Func<dynamic> getCurrentMaxId);
	}
}