using CacheRepository.Configuration;
using CacheRepository.ConnectionResolvers;
using DapperExtensions;

namespace CacheRepository.InsertStrategies
{
	public class SqlInsert : IInsertStrategy
	{
		public void Insert<TEntity>(TEntity entity) where TEntity : class
		{
			var defaultMapper = DapperExtensions.DapperExtensions.DefaultMapper;
			DapperExtensions.DapperExtensions.DefaultMapper = typeof(EntityIdIsAssigned<>);
			ConnectionResolver.GetConnection().Insert(entity, ConnectionResolver.GetTransaction(), 0);
			DapperExtensions.DapperExtensions.DefaultMapper = defaultMapper;
		}

		public ISqlConnectionResolver ConnectionResolver { set; private get; }
	}
}