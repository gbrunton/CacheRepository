﻿using System;
using System.Linq;
using CacheRepository.Configuration.Builders;
using DapperExtensions;
using DapperExtensions.Mapper;

namespace CacheRepository.InsertStrategies
{
	public class SqlInsert : IInsertStrategy
	{
		private readonly SqlRepositoryConfigBuilder sqlRepositoryConfigBuilder;

		public SqlInsert(SqlRepositoryConfigBuilder sqlRepositoryConfigBuilder)
		{
			if (sqlRepositoryConfigBuilder == null) throw new ArgumentNullException("sqlRepositoryConfigBuilder");
			this.sqlRepositoryConfigBuilder = sqlRepositoryConfigBuilder;
		}

		public void Insert<TEntity>(TEntity entity) where TEntity : class
		{
			var defaultMapper = DapperExtensions.DapperExtensions.DefaultMapper;
			DapperExtensions.DapperExtensions.DefaultMapper = typeof(EntityIdIsAssigned<>);
			this.sqlRepositoryConfigBuilder.
				SqlConnectionResolver
				.GetConnection()
				.Insert(entity, this.sqlRepositoryConfigBuilder.SqlConnectionResolver.GetTransaction(), 0);
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