using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using CacheRepository.DisposeStrategies;
using CacheRepository.FileEntityFactoryStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.Repositories;
using CacheRepository.SetIdStrategy;

namespace CacheRepository.Configuration.Configs
{
	public class FileRepositoryConfig
	{
		public FileConnectionResolver ConnectionResolver { get; set; }
		public IEnumerable<IIndex> Indexes { get; set; }
		public IFileEntityFactoryStrategy EntityFactoryStrategy { get; set; }
		public IDisposeStrategy DisposeStrategy { get; set; }

		public INextIdStrategy NextIdStrategy { get; set; }
		public ISetIdStrategy SetIdStrategy { get; set; }
		public IInsertStrategy InsertStrategy { get; set; }
	    public bool PersistData { get; set; }

	    public FileRepository BuildRepository()
		{
			return new FileRepository(this);
		}
	}
}