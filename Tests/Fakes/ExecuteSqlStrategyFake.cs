using CacheRepository.ExecuteSqlStrategies;

namespace Tests.Fakes
{
    public class ExecuteSqlStrategyFake : IExecuteSqlStrategy
    {
        public void Execute(string sql, object parameters)
        {
            this.Sql = sql;
            this.Parameters = parameters;
        }

        public string Sql { get; private set; }
        public object Parameters { get; private set; }
    }
}