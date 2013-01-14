using System;
using System.IO;
using CacheRepository.Configuration.Configs;
using FubuCore.Util;

namespace CacheRepository.ConnectionResolvers
{
	public class FileConnectionResolver : IDisposable
	{
		private readonly Cache<string, StreamReader> streamReaderCache;

		public FileConnectionResolver(string rootPathFolder)
		{
			if (rootPathFolder == null) throw new ArgumentNullException("rootPathFolder");
			this.streamReaderCache = new Cache<string, StreamReader>
				{
					OnMissing = name =>
						File.OpenText(Path.Combine(rootPathFolder, name, this.FileRepositoryConfig.FileExtension))
				};
		}

		// TODO: When I make the repository implement IDisposible and not make consumers of this lib to new up
		// ConnectionResolvers, I'll be able to just inject the following into the constructor
		public FileRepositoryConfig FileRepositoryConfig { get; set; }

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