using System;
using System.Collections.Generic;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration.Configs
{
	public interface IRepositoryConfig
	{
		IEnumerable<IIndex> Indexes { get; set; }
		INextIdStrategy NextIdStrategy { get; set; }
		List<Tuple<Type, string>> CustomEntitySql { get; set; }
		ISetIdStrategy SetIdStrategy { get; set; }
		IBulkInsertStrategy BulkInsertStrategy { get; set; }
		IInsertStrategy InsertStrategy { get; set; }
		IUpdateStrategy UpdateStrategy { get; set; }
		IEntityRetrieverStrategy EntityRetrieverStrategy { get; set; }
	}
}