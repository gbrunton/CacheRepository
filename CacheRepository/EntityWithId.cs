namespace CacheRepository
{
	public abstract class EntityWithId : IEntityWithId
	{
		public int Id { get; set; }
	}
}