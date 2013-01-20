using System;
using System.IO;
using FubuCore.Util;

namespace CacheRepository.ConnectionResolvers
{
	public class FilePathResolver
	{
		private readonly Cache<string, string> pathCache;

		public FilePathResolver(string rootPathFolder, string fileExtension)
		{
			if (rootPathFolder == null) throw new ArgumentNullException("rootPathFolder");
			this.pathCache = new Cache<string, string>
			{
				OnMissing = entityName => Path.Combine(rootPathFolder, entityName + fileExtension)
			};
		}

		public string GetPathToFile(string entityName)
		{
			return this.pathCache[entityName];
		}
	}
}