using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CacheRepository.Configuration.Configs;
using CacheRepository.Indexes;
using CacheRepository.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CacheRepository.Repositories
{
    public class RepositoryData : IDisposable
    {
        private readonly IRepositoryConfig repositoryConfig;

        public RepositoryData(IRepositoryConfig repositoryConfig)
        {
            if (repositoryConfig == null) throw new ArgumentNullException("repositoryConfig");
            this.repositoryConfig = repositoryConfig;
            if (repositoryConfig.PersistData)
            {
                var serializer = new JsonSerializer();

                this.IndexesCached = new Lazy<Cache<Type, IIndex>>(() =>
                {
                    Dictionary<Type, IIndex> deserializeData = null;
                    if (File.Exists("indexFile.dat"))
                    {
                        using (var indexFileStream = File.OpenText("indexFile.dat"))
                        using (var reader = new JsonTextReader(indexFileStream))
                        {
                            var deserialize = serializer.Deserialize<Dictionary<Type, dynamic>>(reader);
                            deserializeData = deserialize
                                .ToDictionary(
                                    pair => pair.Key,
                                    pair =>
                                    {
                                        var jobj = ((JObject) pair.Value);
                                        var o = jobj.ToObject(pair.Key);
                                        return (IIndex) o;
                                        //(IIndex) ((JObject) pair.Value).ToObject(pair.Key)
                                    }
                                );

                            //deserializeData = serializer.Deserialize<Dictionary<Type, IIndex>>(reader)
                            //        .ToDictionary(pair => pair.Key,
                            //            pair => (IIndex) ((JObject) pair.Value).ToObject(pair.Key));
                        }
                    }
                    return deserializeData == null
                        ? new Cache<Type, IIndex>(this.repositoryConfig.Indexes.ToDictionary(index => index.GetType(), index => index))
                        : new Cache<Type, IIndex>(deserializeData);
                });

                this.EntitiesCached = new Lazy<Cache<Type, List<dynamic>>>(() =>
                {
                    Dictionary<Type, List<dynamic>> deserializeData = null;
                    if (File.Exists("entityFile.dat"))
                    {
                        using (var entityFileStream = File.OpenText("entityFile.dat"))
                        using (var reader = new JsonTextReader(entityFileStream))
                        {
                            deserializeData =
                                serializer.Deserialize<Dictionary<Type, List<dynamic>>>(reader)
                                    .ToDictionary(pair => pair.Key,
                                        pair =>
                                            pair.Value.Select(x => (dynamic) ((JObject) x).ToObject(pair.Key)).ToList());
                        }                        
                    }
                    return deserializeData == null
                        ? new Cache<Type, List<dynamic>>()
                        : new Cache<Type, List<dynamic>>(deserializeData);
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
            if (!this.repositoryConfig.PersistData) return;

            var serializer = new JsonSerializer();
            var indexes = this.IndexesCached.Value.ToDictionary();
            using (var indexFileStream = File.CreateText("indexFile.dat"))
            {
                serializer.Serialize(indexFileStream, indexes);
            }

            var entities = this.EntitiesCached.Value.ToDictionary();
            using (var entityFileStream = File.CreateText("entityFile.dat"))
            {
                serializer.Serialize(entityFileStream, entities);
            }

        }
    }
}