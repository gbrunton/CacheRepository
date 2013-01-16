using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using CacheRepository.FileEntityFactoryStrategies;

namespace CacheRepository.EntityRetrieverStrategies
{
	public class FileEntityRetrieverStrategy : IEntityRetrieverStrategy
	{
		private readonly IFileEntityFactoryStrategy fileEntityFactoryStrategy;
		private readonly FileConnectionResolver connectionResolver;

		public FileEntityRetrieverStrategy(IFileEntityFactoryStrategy fileEntityFactoryStrategy, FileConnectionResolver connectionResolver)
		{
			if (fileEntityFactoryStrategy == null) throw new ArgumentNullException("fileEntityFactoryStrategy");
			if (connectionResolver == null) throw new ArgumentNullException("connectionResolver");
			this.fileEntityFactoryStrategy = fileEntityFactoryStrategy;
			this.connectionResolver = connectionResolver;
		}

		public IEnumerable<dynamic> GetAll<TEntity>(string queryString) where TEntity : class
		{
			var entities = new List<dynamic>();
			var file = this.connectionResolver.GetFile(typeof (TEntity).Name);
			while (true)
			{
				var line = file.ReadLine();
				if (string.IsNullOrEmpty(line)) break;
				entities.Add(this.fileEntityFactoryStrategy.Create<TEntity>(line));
			}
			return entities;
		}
	}
}