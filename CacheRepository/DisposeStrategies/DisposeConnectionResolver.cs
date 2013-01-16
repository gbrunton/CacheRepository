using System;
using CacheRepository.ConnectionResolvers;

namespace CacheRepository.DisposeStrategies
{
	public class DisposeConnectionResolver : IDisposeStrategy
	{
		private readonly IConnectionResolver connectionResolver;

		public DisposeConnectionResolver(IConnectionResolver connectionResolver)
		{
			if (connectionResolver == null) throw new ArgumentNullException("connectionResolver");
			this.connectionResolver = connectionResolver;
		}

		public void Dispose()
		{
			this.connectionResolver.Dispose();
		}
	}
}