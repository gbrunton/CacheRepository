using System;
using System.Collections.Generic;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.CommitStrategies;
using CacheRepository.Configuration.Builders;
using CacheRepository.DisposeStrategies;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.ExecuteSqlStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.QueryStrategies;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration.Configs
{
	public class RepositoryConfig : IRepositoryConfig
	{
		public IEnumerable<IIndex> Indexes { get; set; }
		public INextIdStrategy NextIdStrategy { get; set; }
		public IEnumerable<Tuple<Type, string>> CustomEntitySql { get; set; }
		public ISetIdStrategy SetIdStrategy { get; set; }
		public IBulkInsertStrategy BulkInsertStrategy { get; set; }
		public IInsertStrategy InsertStrategy { get; set; }
		public IUpdateStrategy UpdateStrategy { get; set; }
		public IEntityRetrieverStrategy EntityRetrieverStrategy { get; set; }
		public IExecuteSqlStrategy ExecuteSqlStrategy { get; set; }
		public IQueryStrategy QueryStrategy { get; set; }
		public ICommitStrategy CommitStrategy { get; set; }
		public IDisposeStrategy DisposeStrategy { get; set; }
	    public string PersistedDataPath { get; set; }
	    public PersistedDataAccess PersistedDataAccess { get; set; }
	}
}