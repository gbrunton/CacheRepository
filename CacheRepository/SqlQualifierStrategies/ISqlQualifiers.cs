namespace CacheRepository.SqlQualifierStrategies
{
    public interface ISqlQualifiers
    {
        string BeginDelimitedIdentifier { get; }
        string EndDelimitedIdentifier { get; }
    }
}