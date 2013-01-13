using System;
using System.Collections.Generic;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.ConnectionResolvers;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;

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
				ISetIdStrategy setIdStrategy,
				IBulkInsertStrategy bulkInsertStrategy,
				IInsertStrategy insertStrategy,
				IUpdateStrategy updateStrategy,
				IEntityRetrieverStrategy entityRetrieverStrategy
			)
		{
			ConnectionResolver = sqlConnectionResolver;
			Indexes = indexes;
			NextIdStrategy = nextIdStrategy;
			CustomEntitySql = customEntitySql;
			SetIdStrategy = setIdStrategy;
			BulkInsertStrategy = bulkInsertStrategy;
			InsertStrategy = insertStrategy;
			UpdateStrategy = updateStrategy;
			EntityRetrieverStrategy = entityRetrieverStrategy;
		}

		public ISqlConnectionResolver ConnectionResolver { get; private set; }
		public IEnumerable<IIndex> Indexes { get; private set; }
		public INextIdStrategy NextIdStrategy { get; private set; }
		public List<Tuple<Type, string>> CustomEntitySql { get; private set; }
		public ISetIdStrategy SetIdStrategy { get; private set; }
		public IBulkInsertStrategy BulkInsertStrategy { get; private set; }
		public IInsertStrategy InsertStrategy { get; private set; }
		public IUpdateStrategy UpdateStrategy { get; private set; }
		public IEntityRetrieverStrategy EntityRetrieverStrategy { get; private set; }
	}
}