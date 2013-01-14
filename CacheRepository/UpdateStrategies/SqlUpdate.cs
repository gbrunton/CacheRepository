using System;
using CacheRepository.Configuration.Configs;
using DapperExtensions;

namespace CacheRepository.UpdateStrategies
{
	public class SqlUpdate : IUpdateStrategy
	{
		private readonly SqlRepositoryConfig repositoryConfig;

		public SqlUpdate(SqlRepositoryConfig repositoryConfig)
		{
			if (repositoryConfig == null) throw new ArgumentNullException("repositoryConfig");
			this.repositoryConfig = repositoryConfig;
		}

		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			// what happens if an indexed property gets updated???
			// See Issue #2
			this.repositoryConfig.SqlConnectionResolver
				.GetConnection()
				.Update(entity, this.repositoryConfig.SqlConnectionResolver.GetTransaction());
		}
	}
}