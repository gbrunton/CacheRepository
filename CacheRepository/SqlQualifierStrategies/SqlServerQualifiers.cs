namespace CacheRepository.SqlQualifierStrategies
{
    public class SqlServerQualifiers : ISqlQualifiers
    {
        public string BeginDelimitedIdentifier
        {
            get { return "["; }
        }

        public string EndDelimitedIdentifier
        {
            get { return "]"; }
        }
    }
}