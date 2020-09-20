using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace WeebReader.Web.Portal.Others.RateLimit
{
    public class AlternativeRateLimitConfiguration : RateLimitConfiguration
    {
        public override ICounterKeyBuilder EndpointCounterKeyBuilder { get; } = new AlternativeEndpointCounterKeyBuilder();

        public AlternativeRateLimitConfiguration(IHttpContextAccessor httpContextAccessor, IOptions<IpRateLimitOptions> ipOptions, IOptions<ClientRateLimitOptions> clientOptions) : base(httpContextAccessor, ipOptions, clientOptions) { }
    }
}