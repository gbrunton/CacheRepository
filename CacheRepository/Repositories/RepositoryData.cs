using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CacheRepository.Configuration.Builders;
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
                    var path = Path.Combine(this.repositoryConfig.PersistedDataPath, "Indexes");
                    if (!Directory.Exists(path)) return new Cache<Type, IIndex>(this.repositoryConfig.Indexes.ToDictionary(index => index.GetType(), index => index));
                    var cache = new Cache<Type, IIndex>();
                    Directory.EnumerateFiles(path)
                        .Each(fileName =>
                        {
                            using (var entityFileStream = File.OpenRead(fileName))
                            {
                                var indexContainer = JsonSerializer.DeserializeFromStream<IndexContainer>(entityFileStream);
                                cache.Fill(indexContainer.Type, indexContainer.Index);
                            }
                        });
                    return cache;
                });

                this.EntitiesCached = new Lazy<Cache<Type, List<dynamic>>>(() =>
                {
                    var path = Path.Combine(this.repositoryConfig.PersistedDataPath, "Entities");
                    if (!Directory.Exists(path)) return new Cache<Type, List<dynamic>>();
                    var cache = new Cache<Type, List<dynamic>>();
                    Directory.EnumerateFiles(path)
                        .Each(fileName =>
                        {
                            using (var entityFileStream = File.OpenRead(fileName))
                            {
                                var entityContainer = JsonSerializer.DeserializeFromStream<EntityContainer>(entityFileStream);
                                cache.Fill(entityContainer.Type, entityContainer.Entities);
                            }
                        });
                    return cache;
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

            var pathToIndexes = Path.Combine(this.repositoryConfig.PersistedDataPath, "Indexes");
            Directory.CreateDirectory(pathToIndexes);

            foreach (var item in this.IndexesCached.Value.ToDictionary())
            {
                var pathToFile = Path.Combine(pathToIndexes, string.Format("{0}.dat", item.Key));
                if (this.repositoryConfig.PersistedDataAccess == PersistedDataAccess.ReadOnly && File.Exists(pathToFile)) continue;
                using (var fileStream = File.Create(pathToFile))
                {
                    JsonSerializer.SerializeToStream(
                        new IndexContainer
                        {
                            Type = item.Key,
                            Index = item.Value
                        }, fileStream);
                }                

            }

            var pathToEntities = Path.Combine(this.repositoryConfig.PersistedDataPath, "Entities");
            Directory.CreateDirectory(pathToEntities);
            foreach (var item in this.EntitiesCached.Value.ToDictionary())
            {
                var pathToFile = Path.Combine(pathToEntities, string.Format("{0}.dat", item.Key));
                if (this.repositoryConfig.PersistedDataAccess == PersistedDataAccess.ReadOnly && File.Exists(pathToFile)) continue;
                using (var fileStream = File.Create(pathToFile))
                {
                    JsonSerializer.SerializeToStream(
                        new EntityContainer
                        {
                            Type = item.Key,
                            Entities = item.Value
                        }, fileStream);
                }                
            }
        }
    }

    public class IndexContainer
    {
        public Type Type { get; set; }
        public IIndex Index { get; set; }
    }

    public class EntityContainer
    {
        public List<dynamic> Entities { get; set; }
        public Type Type { get; set; }
    }
}