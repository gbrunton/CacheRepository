using System;
using System.Collections.Generic;
using System.Linq;
using CacheRepository.ExecuteSqlStrategies;
using SqlKata;
using SqlKata.Compilers;

namespace CacheRepository.InsertStrategies
{
    public class SqlInsertWithDapper : IInsertStrategy
    {
        private readonly IExecuteSqlStrategy executeSqlStrategy;
        private readonly SqlServerCompiler sqlServerCompiler;
        private readonly Dictionary<Type, string> cachedSql;

        public SqlInsertWithDapper(IExecuteSqlStrategy executeSqlStrategy)
        {
            this.executeSqlStrategy = executeSqlStrategy ?? throw new ArgumentNullException(nameof(executeSqlStrategy));
            this.sqlServerCompiler = new SqlServerCompiler();
            this.cachedSql = new Dictionary<Type, string>();
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : class
        {
            var entityType = entity.GetType();

            if (!this.cachedSql.TryGetValue(entityType, out var sql))
            {
                var idProperty = entityType.GetProperties()
                    .SingleOrDefault(x => x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase));
                if (idProperty == null)
                    throw new Exception(
                        $"Entity '{entityType.Name}' does not have an Id property so an update cannot be made.");
                var query = new Query(entityType.Name).AsInsert(entity);
                sql = this.sqlServerCompiler.Compile(query).Sql;
                this.cachedSql.Add(entityType, sql);
            }

            this.executeSqlStrategy.Execute(sql, entity);
        }
    }
}