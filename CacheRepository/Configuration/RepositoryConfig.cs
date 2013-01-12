using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using CacheRepository.Indexes;
using CacheRepository.NextIdStrategies;
using CacheRepository.SetIdStrategy;

namespace CacheRepository.Configuration
{
	internal class RepositoryConfig
	{
		internal RepositoryConfig
			(
				ISqlConnectionResolver sqlConnectionResolver, 
				IEnumerable<IIndex> indexes, 
				INextIdStrategy nextIdStrategy, 
				List<Tuple<Type, string>> customEntitySql, 
				ISetIdStrategy setIdStrategy
			)
		{
			ConnectionResolver = sqlConnectionResolver;
			Indexes = indexes;
			NextIdStrategy = nextIdStrategy;
			CustomEntitySql = customEntitySql;
			SetIdStrategy = setIdStrategy;
		}

		public ISqlConnectionResolver ConnectionResolver { get; private set; }
		public IEnumerable<IIndex> Indexes { get; private set; }
		public INextIdStrategy NextIdStrategy { get; private set; }
		public List<Tuple<Type, string>> CustomEntitySql { get; private set; }
		public ISetIdStrategy SetIdStrategy { get; private set; }
	}
}