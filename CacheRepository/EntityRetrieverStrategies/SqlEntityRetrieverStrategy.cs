using System;
using System.Collections.Generic;
using CacheRepository.Configuration.Configs;
using Dapper;
using FubuCore;

namespace CacheRepository.EntityRetrieverStrategies
{
	public class SqlEntityRetrieverStrategy : IEntityRetrieverStrategy
	{
		private readonly SqlRepositoryConfig repositoryConfig;

		public SqlEntityRetrieverStrategy(SqlRepositoryConfig repositoryConfig)
		{
			if (repositoryConfig == null) throw new ArgumentNullException("repositoryConfig");
			this.repositoryConfig = repositoryConfig;
		}

		public IEnumerable<dynamic> GetAll<TEntity>(string queryString) where TEntity : class
		{
			queryString = string.IsNullOrEmpty(queryString)
							  ? "Select * From [{0}]".ToFormat(typeof(TEntity).Name)
				              : queryString;
			return this.repositoryConfig.SqlConnectionResolver
				.GetConnection()
				.Query<TEntity>(queryString, null, this.repositoryConfig.SqlConnectionResolver.GetTransaction(), true, 0);
		}
	}
}