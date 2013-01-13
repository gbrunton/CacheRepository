using CacheRepository.ConnectionResolvers;
using DapperExtensions;

namespace CacheRepository.UpdateStrategies
{
	public class SqlUpdate : IUpdateStrategy
	{
		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			// what happens if an indexed property gets updated???
			// See Issue #2
			ConnectionResolver.GetConnection().Update(entity, ConnectionResolver.GetTransaction());
		}

		public ISqlConnectionResolver ConnectionResolver { set; private get; }
	}
}