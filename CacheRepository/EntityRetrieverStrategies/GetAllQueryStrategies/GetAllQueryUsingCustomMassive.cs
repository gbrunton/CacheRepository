using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace CacheRepository.EntityRetrieverStrategies.GetAllQueryStrategies
{
    public class GetAllQueryUsingCustomMassive : IGetAllQuery
    {
        public IEnumerable<dynamic> Execute<TEntity>(IDbConnection connection, string queryString, IDbTransaction transaction) where TEntity : class
        {
            // http://stackoverflow.com/a/29972767/384853
            var func = Expression.Lambda<Func<TEntity>>(
                Expression.New(typeof (TEntity).GetConstructor(Type.EmptyTypes))
                ).Compile();

            return new DynamicModel(connection).Query(queryString, connection).Select(x =>
            {
                var entity = func();
                return Mapper<TEntity>.Map(x, entity);
            });
        }
    }
}