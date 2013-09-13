using System;
using System.Data;

namespace CacheRepository.ConnectionResolvers
{
	public class SqlWithPassThruConnectionAndTransactionConnectionResolver : ISqlConnectionResolver
	{
		private readonly IDbConnection connection;
		private readonly IDbTransaction transaction;

		public SqlWithPassThruConnectionAndTransactionConnectionResolver(IDbConnection connection, IDbTransaction transaction)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (transaction == null) throw new ArgumentNullException("transaction");
			this.connection = connection;
			this.transaction = transaction;
		}

		public IDbConnection GetConnection()
		{
			return connection;
		}

		public IDbTransaction GetTransaction()
		{
			return this.transaction;
		}

		public void Dispose()
		{
			// doing nothing here because this should be handled the they user of this type
		}

		public void Commit()
		{
			// doing nothing here because this should be handled the they user of this type
		}
	}
}