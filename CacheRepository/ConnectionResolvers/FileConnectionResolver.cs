using System;
using System.IO;
using CacheRepository.Configuration.Configs;
using FubuCore.Util;

namespace CacheRepository.ConnectionResolvers
{
	public class FileConnectionResolver : IConnectionResolver
	{
		private readonly Cache<string, StreamReader> streamReaderCache;

		internal FileConnectionResolver(string rootPathFolder, FileRepositoryConfig fileRepositoryConfig)
		{
			if (rootPathFolder == null) throw new ArgumentNullException("rootPathFolder");
			this.streamReaderCache = new Cache<string, StreamReader>
				{
					OnMissing = name =>
						File.OpenText(Path.Combine(rootPathFolder, name, fileRepositoryConfig.FileExtension))
				};
		}

		public StreamReader GetFile(string name)
		{
			return this.streamReaderCache[name];
		}

		public void Dispose()
		{
			this.streamReaderCache.Each(x => x.Dispose());
		}
	}
}