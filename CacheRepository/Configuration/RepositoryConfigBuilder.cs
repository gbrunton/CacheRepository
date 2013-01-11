using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using CacheRepository.Indexes;
using CacheRepository.NextIdCommands;

namespace CacheRepository.Configuration
{
	public class RepositoryConfigBuilder
	{
		private readonly ISqlConnectionResolver connectionResolver;
		private IEnumerable<IIndex> indexes;
		private INextIdCommand nextIdCommand;
		private readonly List<Tuple<Type, string>> customEntitySql;

		public RepositoryConfigBuilder(ISqlConnectionResolver connectionResolver)
		{
			this.connectionResolver = connectionResolver;
			this.indexes = new List<IIndex>();
			this.nextIdCommand = new IdIsInt();
			this.customEntitySql = new List<Tuple<Type, string>>();
		}

		internal RepositoryConfig Build()
		{
			return new RepositoryConfig(this.connectionResolver, this.indexes, this.nextIdCommand, this.customEntitySql);
		}

		public RepositoryConfigBuilder WithIndexes(IEnumerable<IIndex> newValue)
		{
			this.indexes = newValue;
			return this;
		}

		public RepositoryConfigBuilder WithNextIdCommand(INextIdCommand newValue)
		{
			this.nextIdCommand = newValue;
			return this;
		}

		public RepositoryConfigBuilder AddCustomEntitySql<TEntity>(string sql)
		{
			this.customEntitySql.Add(new Tuple<Type, string>(typeof(TEntity), sql));
			return this;
		}
	}
}