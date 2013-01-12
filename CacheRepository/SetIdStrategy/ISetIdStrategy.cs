namespace CacheRepository.SetIdStrategy
{
	public interface ISetIdStrategy
	{
		void SetId(dynamic entityId, dynamic entity);
	}
}