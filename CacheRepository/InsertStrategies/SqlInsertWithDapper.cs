using System;
using System.Linq;
using CacheRepository.ConnectionResolvers;
using DapperExtensions;
using DapperExtensions.Mapper;

namespace CacheRepository.InsertStrategies
{
	public class SqlInsertWithDapper : IInsertStrategy
	{
		private readonly ISqlConnectionResolver connectionResolver;

		public SqlInsertWithDapper(ISqlConnectionResolver connectionResolver)
		{
			if (connectionResolver == null) throw new ArgumentNullException("connectionResolver");
			this.connectionResolver = connectionResolver;
		}

		public void Insert<TEntity>(TEntity entity) where TEntity : class
		{
			var defaultMapper = DapperExtensions.DapperExtensions.DefaultMapper;
			DapperExtensions.DapperExtensions.DefaultMapper = typeof(EntityIdIsAssigned<>);
			this.connectionResolver
				.GetConnection()
				.Insert(entity, this.connectionResolver.GetTransaction(), 0);
			DapperExtensions.DapperExtensions.DefaultMapper = defaultMapper;
		}
	}

	public class EntityIdIsAssigned<TEntityWithId> : AutoClassMapper<TEntityWithId> where TEntityWithId : class
	{
		protected override void AutoMap()
		{
			var type = typeof(TEntityWithId);
			var keyFound = Properties.Any(p => p.KeyType != KeyType.NotAKey);
			foreach (var propertyInfo in type.GetProperties())
			{
				if (Properties.Any(p => p.Name.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
				{
					continue;
				}

				var map = Map(propertyInfo);

				if (!keyFound && string.Equals(map.PropertyInfo.Name, "Id", StringComparison.InvariantCultureIgnoreCase))
				{
					map.Key(KeyType.Assigned);
					keyFound = true;
				}
			}
		}
	}
}