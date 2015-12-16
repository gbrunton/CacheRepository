using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CacheRepository.Configuration.Configs;
using CacheRepository.Indexes;
using CacheRepository.Utils;
using ServiceStack.Text;

namespace CacheRepository.Repositories
{
    public class RepositoryData : IDisposable
    {
        private readonly IRepositoryConfig repositoryConfig;

        public RepositoryData(IRepositoryConfig repositoryConfig)
        {
            if (repositoryConfig == null) throw new ArgumentNullException("repositoryConfig");
            this.repositoryConfig = repositoryConfig;
            if (!string.IsNullOrWhiteSpace(repositoryConfig.PersistedDataPath))
            {
                this.IndexesCached = new Lazy<Cache<Type, IIndex>>(() =>
                {
                    var path = Path.Combine(this.repositoryConfig.PersistedDataPath, "indexFile.dat");
                    if (!File.Exists(path)) return new Cache<Type, IIndex>(this.repositoryConfig.Indexes.ToDictionary(index => index.GetType(), index => index));
                    using (var indexFileStream = File.OpenText(path))
                    {
                        return new Cache<Type, IIndex>(TypeSerializer.DeserializeFromReader<Dictionary<Type, IIndex>>(indexFileStream));
                    }
                });

                this.EntitiesCached = new Lazy<Cache<Type, List<dynamic>>>(() =>
                {
                    var path = Path.Combine(this.repositoryConfig.PersistedDataPath, "entityFile.dat");
                    if (!File.Exists(path)) return new Cache<Type, List<dynamic>>();
                    using (var entityFileStream = File.OpenText(path))
                    {
                        return new Cache<Type, List<dynamic>>(TypeSerializer.DeserializeFromReader<Dictionary<Type, List<dynamic>>>(entityFileStream));
                    }
                });
            }
            else
            {
                this.IndexesCached = new Lazy<Cache<Type, IIndex>>(() => new Cache<Type, IIndex>(repositoryConfig.Indexes.ToDictionary(index => index.GetType(), index => index)));
                this.EntitiesCached = new Lazy<Cache<Type, List<dynamic>>>(() => new Cache<Type, List<dynamic>>()); 
            }
        }

        public readonly Lazy<Cache<Type, IIndex>> IndexesCached;
        public readonly Lazy<Cache<Type, List<dynamic>>> EntitiesCached;

        public void Dispose()
        {
            if (string.IsNullOrWhiteSpace(this.repositoryConfig.PersistedDataPath)) return;

            if (this.IndexesCached.Value.Any())
            {
                using (var indexFileStream = File.CreateText(Path.Combine(this.repositoryConfig.PersistedDataPath, "indexFile.dat")))
                {
                    TypeSerializer.SerializeToWriter(this.IndexesCached.Value.ToDictionary(), indexFileStream);
                }                
            }

            if (this.EntitiesCached.Value.Any())
            {
                using (var entityFileStream = File.CreateText(Path.Combine(this.repositoryConfig.PersistedDataPath, "entityFile.dat")))
                {
                    TypeSerializer.SerializeToWriter(this.EntitiesCached.Value.ToDictionary(x => x[0].GetType()), entityFileStream);
                }                
            }
        }
    }
}