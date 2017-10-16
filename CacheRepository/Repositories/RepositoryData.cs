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

                this.IndexesCached = new Lazy<Cache<Type, IIndex>>(() =>
                {
                    var indexCache = new Cache<Type, IIndex>(repositoryConfig.Indexes.ToDictionary(index => index.GetType(), index =>
                    {
                        index.Clear(); // Clear out any existing data in the index because we are going to build it back up here.
                        return index;
                    }));
                    var cachedEntities = this.EntitiesCached.Value;

                    foreach (var entry in cachedEntities.ToDictionary())
                    {
                        var entityType = entry.Key;
                        var indexes = indexCache.Where(x => entityType == x.GetEntityType()).ToArray();
                        foreach (var entity in entry.Value)
                        {
                            indexes.Each(index => index.Add(entity));
                        }
                    }

                    return indexCache;
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

    public class EntityContainer
    {
        public List<dynamic> Entities { get; set; }
        public Type Type { get; set; }
    }
}