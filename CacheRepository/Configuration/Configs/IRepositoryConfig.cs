using System;
using System.Collections.Generic;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.CommitStrategies;
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
	public interface IRepositoryConfig
	{
		IEnumerable<IIndex> Indexes { get; }
		INextIdStrategy NextIdStrategy { get; }
		IEnumerable<Tuple<Type, string>> CustomEntitySql { get; }
		ISetIdStrategy SetIdStrategy { get; }
		IBulkInsertStrategy BulkInsertStrategy { get; }
		IInsertStrategy InsertStrategy { get; }
		IUpdateStrategy UpdateStrategy { get; }
		IEntityRetrieverStrategy EntityRetrieverStrategy { get; }
		IExecuteSqlStrategy ExecuteSqlStrategy { get; }
		IQueryStrategy QueryStrategy { get; }
		ICommitStrategy CommitStrategy { get; }
		IDisposeStrategy DisposeStrategy { get; }
	}
}