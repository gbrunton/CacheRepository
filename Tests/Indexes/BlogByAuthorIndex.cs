using CacheRepository.Indexes;
using Tests.Entities;

namespace Tests.Indexes
{
    public class BlogByAuthorIndex : NonUniqueIndex<Blog, string>
    {
        protected override string GetKey(Blog entity)
        {
            return entity.Author;
        }
    }
}