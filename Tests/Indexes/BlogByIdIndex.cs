using CacheRepository.Indexes;
using Tests.Entities;

namespace Tests.Indexes
{
    public class BlogByIdIndex : UniqueIndex<Blog, int>
    {
        protected override int GetKey(Blog entity)
        {
            return entity.Id;
        }
    }
}