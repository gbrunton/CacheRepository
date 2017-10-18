using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CacheRepository.Behaviours;
using CacheRepository.Configuration.Builders;
using CacheRepository.Configuration.Configs;
using CacheRepository.Indexes;
using CacheRepository.Utils;
using ServiceStack.Text;

namespace CacheRepository.Repositories
{
	public class Repository 
		: IDisposable, ICanInsert, ICanBuilkInsert, ICanSqlQuery, ICanExecuteSql, ICanUpdate, ICanCommit, ICanGet
	{
		private readonly IRepositoryConfig repositoryConfig;
		private readonly Cache<Type, List<dynamic>> entitiesCachedForBulkInsert;
		private readonly Cache<Type, dynamic> entityIds;
	    private readonly Lazy<Cache<Type, string>> entitySqlCachedLazy;
	    private readonly RepositoryData repositoryData;
	    private readonly Lazy<Cache<string, IEnumerable<dynamic>>> queryCached;

        public Repository(IRepositoryConfig repositoryConfig)
		{
			if (repositoryConfig == null) throw new ArgumentNullException("repositoryConfig");
			this.repositoryConfig = repositoryConfig;
			this.entityIds = new Cache<Type, dynamic> { OnMissing = key => this.repositoryConfig.NextIdStrategy.GetNextId(key, () => this.GetAll(key).Max(x => x.Id)) };
			this.entitiesCachedForBulkInsert = new Cache<Type, List<dynamic>> { OnMissing = typeOfEntity => new List<dynamic>() };
            this.entitySqlCachedLazy = new Lazy<Cache<Type, string>>(() =>
            {
                var entitySqlCached = new Cache<Type, string> { OnMissing = key => null };
                this.repositoryConfig.CustomEntitySql.Each(x => entitySqlCached[x.Item1] = x.Item2);
                return entitySqlCached;
            });
		    this.repositoryData = new RepositoryData(repositoryConfig);
		    this.queryCached = new Lazy<Cache<string, IEnumerable<dynamic>>>(() =>
		    {
		        var cache = new Cache<string, IEnumerable<dynamic>>(type => null);
		        if (!string.IsNullOrWhiteSpace(repositoryConfig.PersistedDataPath))
		        {
		            var pathToQueries = Path.Combine(this.repositoryConfig.PersistedDataPath, "Queries");
		            if (!Directory.Exists(pathToQueries)) return cache;
		            Directory.EnumerateFiles(pathToQueries)
		                .Each(fileName =>
		                {
		                    using (var entityFileStream = File.OpenRead(fileName))
		                    {
		                        var entityContainer = JsonSerializer.DeserializeFromStream<QueryContainer>(entityFileStream);
		                        cache.Fill(entityContainer.Key, entityContainer.QueryResults);
		                    }
		                });
                }
		        return cache;
            });
		}

        public void BuilkInsertNonGeneric(object entity)
		{
			var typeOfEntity = entity.GetType();
			var methodInfo = typeof(Repository).GetMethods().First(x => x.Name == "BulkInsert");
			var generic = methodInfo.MakeGenericMethod(typeOfEntity);
			var parameter = Array.CreateInstance(typeOfEntity, 1);
			parameter.SetValue(entity, 0);
			generic.Invoke(this, new object[] { parameter });
		}

		public void BulkInsert<TEntity>(params TEntity[] entities) where TEntity : class
		{
			this.BulkInsert((IEnumerable<TEntity>)entities);
		}

		public void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			var type = typeof(TEntity);
			this.insert(entities, entity => this.entitiesCachedForBulkInsert[type].Add(entity));
		}

		public void BulkInsertFlush(IEnumerable<Type> flushOrder = null)
		{
			if (flushOrder != null)
			{
				flushOrder.Each(type =>
				{
					this.repositoryConfig.BulkInsertStrategy.Insert(type, this.entitiesCachedForBulkInsert[type]);
					this.entitiesCachedForBulkInsert.Remove(type);
				});
			}

			this.entitiesCachedForBulkInsert
				.Each((type, entityList) =>
					  this.repositoryConfig.BulkInsertStrategy.Insert(type, this.entitiesCachedForBulkInsert[type]));
			this.entitiesCachedForBulkInsert.ClearAll();
		}

		public void Insert<TEntity>(params TEntity[] entities) where TEntity : class
		{
			this.Insert((IEnumerable<TEntity>)entities);
		}

		public void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			insert(entities, entity => this.repositoryConfig.InsertStrategy.Insert(entity));
		}

		private void insert<TEntity>(IEnumerable<TEntity> entities, Action<TEntity> insertFunction) where TEntity : class
		{
			var type = typeof(TEntity);
			var existingEntities = this.getAllWithGeneric<TEntity>();
			var typeIndexes = this.repositoryData.IndexesCached.Value.Where(x => type == x.GetEntityType());
			entities.Each(entity =>
			{
				var id = this.entityIds[type];
				this.repositoryConfig.SetIdStrategy.SetId(type, id, entity);
				this.entityIds[type] = this.repositoryConfig.NextIdStrategy.GetNextId(type, () => id);
				existingEntities.Add(entity);
				insertFunction(entity);
				typeIndexes.Each(index => index.Add(entity));
			});
		}

		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			this.repositoryConfig.UpdateStrategy.Update(entity);
		}

		public void ExecuteSql(string sql, object parameters = null)
		{
			this.repositoryConfig.ExecuteSqlStrategy.Execute(sql, parameters);
		}

		public IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null) where TEntity : class
		{
		    var key = $"{typeof(TEntity)}--{query}--{JsonSerializer.SerializeToString(parameters)}";
		    var cachedValue = this.queryCached.Value;
		    var queryResults = cachedValue[key];

		    if (queryResults != null) return queryResults.Cast<TEntity>();

		    queryResults = this.repositoryConfig.QueryStrategy.Query<TEntity>(query, parameters);

		    cachedValue[key] = queryResults;
		    return queryResults.Cast<TEntity>();
		}

		public void Commit()
		{
			this.repositoryConfig.CommitStrategy.Commit();
		}

		public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class 
		{
			return getAllWithGeneric<TEntity>().Cast<TEntity>();
		}

		public TIndex GetIndex<TIndex>() where TIndex : IIndex
		{
            var index = (TIndex)this.repositoryData.IndexesCached.Value[typeof(TIndex)];
			if (!index.HasBeenHydrated)
			{
				GetAll(index.GetEntityType());
				index.HasBeenHydrated = true;
			}
			return index;
		}

		public Cache<Type, dynamic> GetHashOfIdsByEntityType()
		{
			return this.entityIds;
		}

		public void Dispose()
		{
			this.repositoryConfig.DisposeStrategy?.Dispose();
		    this.repositoryData.Dispose();

		    if (string.IsNullOrWhiteSpace(this.repositoryConfig.PersistedDataPath)) return;

		    var pathToQueries = Path.Combine(this.repositoryConfig.PersistedDataPath, "Queries");
		    Directory.CreateDirectory(pathToQueries);
		    foreach (var item in this.queryCached.Value.ToDictionary())
		    {
		        var pathToFile = Path.Combine(pathToQueries, $"{item.Key.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries)[0]}-{Guid.NewGuid()}.dat");
		        if (this.repositoryConfig.PersistedDataAccess == PersistedDataAccess.ReadOnly && File.Exists(pathToFile)) continue;
		        using (var fileStream = File.Create(pathToFile))
		        {
		            JsonSerializer.SerializeToStream(
		                new QueryContainer
                        {
		                    Key = item.Key,
		                    QueryResults = item.Value
		                }, fileStream);
		        }
		    }
        }

        private List<dynamic> getAllWithGeneric<TEntity>() where TEntity : class 
		{
			var type = typeof(TEntity);
            this.repositoryData.EntitiesCached.Value.OnMissing = key =>
			{
				var all = this.repositoryConfig
					.EntityRetrieverStrategy
				    .GetAll<TEntity>(this.entitySqlCachedLazy.Value[type]);
                var typeIndexes = this.repositoryData.IndexesCached.Value.Where(x => type == x.GetEntityType());
				var returnList = new List<dynamic>();
				all.Each(entity =>
					{
						typeIndexes.Each(index => index.Add(entity));
						returnList.Add(entity);
					});
				return returnList;
			};
            return this.repositoryData.EntitiesCached.Value[type];
		}

		public List<dynamic> GetAll(Type type)
		{
			var methodInfo = typeof(Repository)
				.GetMethod("getAllWithGeneric", BindingFlags.NonPublic | BindingFlags.Instance);
			var generic = methodInfo.MakeGenericMethod(type);
			return (List<dynamic>)generic.Invoke(this, null);
		}
	}

    public class QueryContainer
    {
        public IEnumerable<dynamic> QueryResults { get; set; }
        public string Key { get; set; }
    }
}