using System.Collections.Generic;
using System.Data;
using Dapper;

namespace CacheRepository.EntityRetrieverStrategies.GetAllQueryStrategies
{
    public class GetAllQueryUsingDapper : IGetAllQuery
    {
        public IEnumerable<dynamic> Execute<TEntity>(IDbConnection connection, string queryString, IDbTransaction transaction) where TEntity : class
        {
            return connection.Query<TEntity>(queryString, null, transaction, true, 0);
        }
    }
}