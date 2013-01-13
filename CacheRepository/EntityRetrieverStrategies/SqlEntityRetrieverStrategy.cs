using System;
using System.Collections.Generic;
using CacheRepository.Configuration.Builders;
using Dapper;
using FubuCore;

namespace CacheRepository.EntityRetrieverStrategies
{
	public class SqlEntityRetrieverStrategy : IEntityRetrieverStrategy
	{
		private readonly SqlRepositoryConfigBuilder sqlRepositoryConfigBuilder;

		public SqlEntityRetrieverStrategy(SqlRepositoryConfigBuilder sqlRepositoryConfigBuilder)
		{
			if (sqlRepositoryConfigBuilder == null) throw new ArgumentNullException("sqlRepositoryConfigBuilder");
			this.sqlRepositoryConfigBuilder = sqlRepositoryConfigBuilder;
		}

		public IEnumerable<dynamic> GetAll<TEntity>(string queryString) where TEntity : class
		{
			queryString = string.IsNullOrEmpty(queryString)
							  ? "Select * From [{0}]".ToFormat(typeof(TEntity).Name)
				              : queryString;
			return this.sqlRepositoryConfigBuilder
				.SqlConnectionResolver
				.GetConnection()
				.Query<TEntity>(queryString, null, this.sqlRepositoryConfigBuilder.SqlConnectionResolver.GetTransaction(), true, 0);
		}
	}
}