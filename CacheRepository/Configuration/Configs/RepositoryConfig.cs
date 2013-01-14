using System;
using System.Collections.Generic;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.Repositories;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration.Configs
{
	public abstract class RepositoryConfig<TRepository> : IRepositoryConfig where TRepository : Repository
	{
		public IEnumerable<IIndex> Indexes { get; set; }
		public INextIdStrategy NextIdStrategy { get; set; }
		public List<Tuple<Type, string>> CustomEntitySql { get; set; }
		public ISetIdStrategy SetIdStrategy { get; set; }
		public IBulkInsertStrategy BulkInsertStrategy { get; set; }
		public IInsertStrategy InsertStrategy { get; set; }
		public IUpdateStrategy UpdateStrategy { get; set; }
		public IEntityRetrieverStrategy EntityRetrieverStrategy { get; set; }

		public abstract TRepository BuildRepository();
	}
}