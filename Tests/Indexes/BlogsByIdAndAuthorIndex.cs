using CacheRepository.Indexes;
using Tests.Entities;

namespace Tests.Indexes
{
    public class BlogsByIdAndAuthorIndex : NonUniqueIndex<Blog, IndexKey>
    {
        protected override IndexKey GetKey(Blog entity)
        {
            return new IndexKey(entity.Id, entity.Author);
        }
    }

    public class IndexKey
    {
        protected bool Equals(IndexKey other)
        {
            return Id == other.Id && string.Equals(Author, other.Author);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IndexKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (Author != null ? Author.GetHashCode() : 0);
            }
        }

        public int Id { get; set; }
        public string Author { get; set; }

        public IndexKey(int id, string author)
        {
            Id = id;
            Author = author;
        }
    }
}