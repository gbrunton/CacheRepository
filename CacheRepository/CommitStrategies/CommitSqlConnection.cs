using System;
using CacheRepository.ConnectionResolvers;

namespace CacheRepository.CommitStrategies
{
	public class CommitSqlConnection : ICommitStrategy
	{
		private readonly SqlConnectionResolver sqlConnectionResolver;

		public CommitSqlConnection(SqlConnectionResolver sqlConnectionResolver)
		{
			if (sqlConnectionResolver == null) throw new ArgumentNullException("sqlConnectionResolver");
			this.sqlConnectionResolver = sqlConnectionResolver;
		}

		public void Commit()
		{
			this.sqlConnectionResolver.Commit();
		}
	}
}