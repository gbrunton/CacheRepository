using CacheRepository.Indexes;
using Tests.Entities;

namespace Tests.Indexes
{
    public class BlogByIdTitle : UniqueIndex<Blog, string>
    {
        protected override string GetKey(Blog entity)
        {
            return entity.Title;
        }

        protected override Blog FindDuplicateEntityBasedOnKey(string key, Blog entityAlreadyCached, Blog entity)
        {
            // return the blog with the most recent created date
            return entity.CreatedDate <= entityAlreadyCached.CreatedDate
                ? entity
                : entityAlreadyCached;
        }
    }
}