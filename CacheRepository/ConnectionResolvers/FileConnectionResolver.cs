using System;
using System.IO;
using FubuCore.Util;

namespace CacheRepository.ConnectionResolvers
{
	public class FileConnectionResolver : IConnectionResolver, IFileConnectionResolver
	{
		private readonly Cache<string, StreamReader> streamReaderCache;
		private readonly Cache<string, StreamWriter> streamWriterCache;

		internal FileConnectionResolver(string rootPathFolder, string fileExtension)
		{
			if (rootPathFolder == null) throw new ArgumentNullException("rootPathFolder");
			this.streamReaderCache = new Cache<string, StreamReader>
				{
					OnMissing = name =>
						File.OpenText(Path.Combine(rootPathFolder, name + fileExtension))
				};

			this.streamWriterCache = new Cache<string, StreamWriter>
			{
				OnMissing = name =>
					File.CreateText(Path.Combine(rootPathFolder, name + fileExtension))
			};
		}

		public string GetLine(string entityName)
		{
			return this.streamReaderCache[entityName].ReadLine();
		}

		public void WriteLine<TEntity>(string line)
		{
			this.streamWriterCache[typeof (TEntity).Name].WriteLine(line);
		}

		public void Dispose()
		{
			this.streamReaderCache.Each(x => x.Dispose());
		}
	}
}