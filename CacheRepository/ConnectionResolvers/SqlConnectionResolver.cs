using System;
using System.Data;

namespace CacheRepository.ConnectionResolvers
{
	public class SqlConnectionResolver : ISqlConnectionResolver
	{
		private readonly IDbConnection connection;
		private IDbTransaction transaction;

		internal SqlConnectionResolver(IDbConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			this.connection = connection;
		}

		public IDbConnection GetConnection()
		{
			if (this.connection.State != ConnectionState.Open)
			{
				this.connection.Open();
				this.transaction = connection.BeginTransaction();
			}
			return connection;
		}

		public IDbTransaction GetTransaction()
		{
			return this.transaction;
		}

		public void Dispose()
		{
			if (this.transaction != null) this.transaction.Dispose();
			if (this.connection != null) this.connection.Dispose();
		}

		public void Commit()
		{
			this.transaction.Commit();
		}
	}
}