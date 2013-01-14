using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CacheRepository.Configuration.Configs;
using CacheRepository.Indexes;
using FubuCore.Util;

namespace CacheRepository.Repositories
{
	public abstract class Repository : IDisposable
	{
		private readonly IRepositoryConfig repositoryConfig;
		protected readonly Cache<Type, IIndex> IndexesCached;
		private readonly Cache<Type, List<dynamic>> entitiesCached;
		protected readonly Cache<Type, List<dynamic>> EntitiesCachedForBulkInsert;
		protected readonly Cache<Type, dynamic> EntityIds;
		private readonly Cache<Type, string> entitySqlCached;

		protected Repository(IRepositoryConfig repositoryConfig)
		{
			if (repositoryConfig == null) throw new ArgumentNullException("repositoryConfig");
			this.repositoryConfig = repositoryConfig;
			this.IndexesCached = new Cache<Type, IIndex>(this.repositoryConfig.Indexes.ToDictionary(index => index.GetType(), index => index));
			this.entitiesCached = new Cache<Type, List<dynamic>>();
			this.entitySqlCached = new Cache<Type, string> {OnMissing = key => null};
			this.EntityIds = new Cache<Type, dynamic> { OnMissing = key => this.repositoryConfig.NextIdStrategy.GetNextId(this.getAll(key).Max(x => x.Id)) };
			this.EntitiesCachedForBulkInsert = new Cache<Type, List<dynamic>> { OnMissing = typeOfEntity => new List<dynamic>() };
			this.repositoryConfig.CustomEntitySql.Each(x => entitySqlCached[x.Item1] = x.Item2);
		}

		public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class 
		{
			return GetAllWithGeneric<TEntity>().Cast<TEntity>();
		}

		public TIndex GetIndex<TIndex>() where TIndex : IIndex
		{
			var index = (TIndex)this.IndexesCached[typeof(TIndex)];
			if (!index.HasBeenHydrated)
			{
				getAll(index.GetEntityType());
				index.HasBeenHydrated = true;
			}
			return index;
		}

		public Cache<Type, dynamic> GetHashOfIdsByEntityType()
		{
			return this.EntityIds;
		}

		protected List<dynamic> GetAllWithGeneric<TEntity>() where TEntity : class 
		{
			var type = typeof(TEntity);
			this.entitiesCached.OnMissing = key =>
			{
				var all = this.repositoryConfig
					.EntityRetrieverStrategy
					.GetAll<TEntity>(this.entitySqlCached[type]);
				var typeIndexes = this.IndexesCached.Where(x => type == x.GetEntityType());
				var returnList = new List<dynamic>();
				all.Each(entity =>
					{
						typeIndexes.Each(index => index.Add(entity));
						returnList.Add(entity);
					});
				return returnList;
			};
			return this.entitiesCached[type];
		}

		private List<dynamic> getAll(Type type)
		{
			var methodInfo = typeof(Repository).GetMethod("GetAllWithGeneric", BindingFlags.NonPublic | BindingFlags.Instance);
			var generic = methodInfo.MakeGenericMethod(type);
			return (List<dynamic>)generic.Invoke(this, null);
		}

		public void Dispose()
		{
			this.repositoryConfig.GetConnectionResolver().Dispose();
		}
	}
}