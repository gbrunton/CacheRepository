using System;
using System.Collections.Generic;
using CacheRepository.Configuration.Configs;

namespace CacheRepository.EntityRetrieverStrategies
{
	public class FileEntityRetrieverStrategy : IEntityRetrieverStrategy
	{
		private readonly FileRepositoryConfig repositoryConfig;

		public FileEntityRetrieverStrategy(FileRepositoryConfig repositoryConfig)
		{
			if (repositoryConfig == null) throw new ArgumentNullException("repositoryConfig");
			this.repositoryConfig = repositoryConfig;
		}

		public IEnumerable<dynamic> GetAll<TEntity>(string queryString) where TEntity : class
		{
			var entities = new List<dynamic>();
			var factory = this.repositoryConfig.FileEntityFactoryStrategy;
			var file = this.repositoryConfig.ConnectionResolver.GetFile(typeof (TEntity).Name);
			while (true)
			{
				var line = file.ReadLine();
				if (string.IsNullOrEmpty(line)) break;
				entities.Add(factory.Create<TEntity>(line));
			}
			return entities;
		}
	}
}