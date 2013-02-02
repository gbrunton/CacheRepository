using System;
using System.Collections.Generic;
using CacheRepository.Indexes;
using CacheRepository.Utils;

namespace CacheRepository.Behaviours
{
	public interface ICanGet
	{
		IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class;
		TIndex GetIndex<TIndex>() where TIndex : IIndex;
		Cache<Type, dynamic> GetHashOfIdsByEntityType();
	}
}