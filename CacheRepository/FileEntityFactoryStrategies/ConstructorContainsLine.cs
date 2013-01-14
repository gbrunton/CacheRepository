using System;

namespace CacheRepository.FileEntityFactoryStrategies
{
	public class ConstructorContainsLine : IFileEntityFactoryStrategy
	{
		public dynamic Create<TEntity>(string line) where TEntity : class
		{
			return Activator.CreateInstance(typeof (TEntity), line);
		}
	}
}