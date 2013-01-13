using System;
using CacheRepository.Configuration.Builders;
using DapperExtensions;

namespace CacheRepository.UpdateStrategies
{
	public class SqlUpdate : IUpdateStrategy
	{
		private readonly SqlRepositoryConfigBuilder sqlRepositoryConfigBuilder;

		public SqlUpdate(SqlRepositoryConfigBuilder sqlRepositoryConfigBuilder)
		{
			if (sqlRepositoryConfigBuilder == null) throw new ArgumentNullException("sqlRepositoryConfigBuilder");
			this.sqlRepositoryConfigBuilder = sqlRepositoryConfigBuilder;
		}

		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			// what happens if an indexed property gets updated???
			// See Issue #2
			this.sqlRepositoryConfigBuilder
				.SqlConnectionResolver
				.GetConnection()
				.Update(entity, this.sqlRepositoryConfigBuilder.SqlConnectionResolver.GetTransaction());
		}
	}
}