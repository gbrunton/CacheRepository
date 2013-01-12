namespace CacheRepository.SetIdStrategy
{
	public class EntityHasNoIdSetter : ISetIdStrategy
	{
		public void SetId(dynamic entityId, dynamic entity)
		{
		}
	}
}