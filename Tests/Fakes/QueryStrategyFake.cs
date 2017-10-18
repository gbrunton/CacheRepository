using System.Collections.Generic;
using System.Linq;
using CacheRepository.QueryStrategies;

namespace Tests.Fakes
{
    public class QueryStrategyFake : IQueryStrategy
    {
        public IEnumerable<dynamic> Results { private get; set; }

        public IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null) where TEntity : class
        {
            return this.Results.Cast<TEntity>();
        }
    }
}