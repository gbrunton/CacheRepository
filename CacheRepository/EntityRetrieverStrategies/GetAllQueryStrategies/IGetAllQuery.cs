using System.Collections.Generic;
using System.Data;

namespace CacheRepository.EntityRetrieverStrategies.GetAllQueryStrategies
{
    public interface IGetAllQuery
    {
        IEnumerable<dynamic> Execute<TEntity>(IDbConnection connection, string queryString, IDbTransaction transaction) where TEntity : class;
    }
}