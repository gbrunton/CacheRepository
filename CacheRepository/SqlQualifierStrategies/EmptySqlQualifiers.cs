namespace CacheRepository.SqlQualifierStrategies
{
    public class EmptySqlQualifiers : ISqlQualifiers
    {
        public string BeginDelimitedIdentifier
        {
            get { return ""; }
        }

        public string EndDelimitedIdentifier
        {
            get { return ""; }
        }
    }
}