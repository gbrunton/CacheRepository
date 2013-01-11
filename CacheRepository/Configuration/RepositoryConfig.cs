using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using CacheRepository.Indexes;
using CacheRepository.NextIdCommands;

namespace CacheRepository.Configuration
{
	internal class RepositoryConfig
	{
		internal RepositoryConfig(ISqlConnectionResolver sqlConnectionResolver, IEnumerable<IIndex> indexes, INextIdCommand nextIdCommand, List<Tuple<Type, string>> customEntitySql)
		{
			ConnectionResolver = sqlConnectionResolver;
			Indexes = indexes;
			NextIdCommand = nextIdCommand;
			CustomEntitySql = customEntitySql;
		}

		public ISqlConnectionResolver ConnectionResolver { get; private set; }
		public IEnumerable<IIndex> Indexes { get; private set; }
		public INextIdCommand NextIdCommand { get; private set; }
		public List<Tuple<Type, string>> CustomEntitySql { get; private set; }
	}
}