using System;
using System.IO;
using CacheRepository.Configuration;
using CacheRepository.Configuration.Builders;
using FubuCore.Util;

namespace CacheRepository.ConnectionResolvers
{
	public class FilePathResolver
	{
		private readonly Cache<Type, string> pathCache;

		public FilePathResolver(Cache<Type,EntityPropertiesForFile> entityPropertiesForFilesCache, string rootPathFolder, string fileExtension)
		{
			if (entityPropertiesForFilesCache == null) throw new ArgumentNullException("entityPropertiesForFilesCache");
			if (rootPathFolder == null) throw new ArgumentNullException("rootPathFolder");
			this.pathCache = new Cache<Type, string>
			{
				OnMissing = entityType => Path.Combine(rootPathFolder, entityPropertiesForFilesCache[entityType].GetFileName() + fileExtension)
			};
		}

		public string GetPathToFile(Type entityType)
		{
			return this.pathCache[entityType];
		}
	}
}