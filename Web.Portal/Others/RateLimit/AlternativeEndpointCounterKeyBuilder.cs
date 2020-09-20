using AspNetCoreRateLimit;

namespace WeebReader.Web.Portal.Others.RateLimit
{
    public class AlternativeEndpointCounterKeyBuilder : ICounterKeyBuilder
    {
        public string Build(ClientRequestIdentity requestIdentity, RateLimitRule rule) => $"_{rule.Endpoint}";
    }
}