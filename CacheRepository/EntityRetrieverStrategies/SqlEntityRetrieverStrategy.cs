using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using CacheRepository.EntityRetrieverStrategies.GetAllQueryStrategies;
using CacheRepository.SqlQualifierStrategies;
using CacheRepository.Utils;

namespace CacheRepository.EntityRetrieverStrategies
{
	public class SqlEntityRetrieverStrategy : IEntityRetrieverStrategy
	{
		private readonly ISqlConnectionResolver connectionResolver;
	    private readonly ISqlQualifiers sqlQualifiers;
        private readonly Dictionary<Type, IGetAllQuery> overriddenDefaultGetAllQueryStrategy;

        public SqlEntityRetrieverStrategy(ISqlConnectionResolver connectionResolver, ISqlQualifiers sqlQualifiers, Dictionary<Type, IGetAllQuery> overriddenDefaultGetAllQueryStrategy)
		{
			if (connectionResolver == null) throw new ArgumentNullException("connectionResolver");
	        if (sqlQualifiers == null) throw new ArgumentNullException("sqlQualifiers");
	        if (overriddenDefaultGetAllQueryStrategy == null) throw new ArgumentNullException("overriddenDefaultGetAllQueryStrategy");
	        this.connectionResolver = connectionResolver;
	        this.sqlQualifiers = sqlQualifiers;
	        this.overriddenDefaultGetAllQueryStrategy = overriddenDefaultGetAllQueryStrategy;
		}

		public IEnumerable<dynamic> GetAll<TEntity>(string queryString) where TEntity : class
		{
		    var entityType = typeof (TEntity);
			queryString = string.IsNullOrEmpty(queryString)
                              ? "Select * From {0}{1}{2}".ToFormat(this.sqlQualifiers.BeginDelimitedIdentifier, entityType.Name, this.sqlQualifiers.EndDelimitedIdentifier)
				              : queryString;

            IGetAllQuery query;
		    if (!this.overriddenDefaultGetAllQueryStrategy.TryGetValue(entityType, out query))
		    {
		        query = new GetAllQueryUsingDapper();
		    }

            return query.Execute<TEntity>(this.connectionResolver.GetConnection(), queryString, this.connectionResolver.GetTransaction());
		}
	}
}