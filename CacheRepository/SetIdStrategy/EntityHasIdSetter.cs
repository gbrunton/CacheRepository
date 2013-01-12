namespace CacheRepository.SetIdStrategy
{
	public class EntityHasIdSetter : ISetIdStrategy
	{
		public void SetId(dynamic entityId, dynamic entity)
		{
			entity.Id = entityId;
		}
	}
}