namespace WeebReader.Web.Models.Others
{
    public static class Utilities
    {
        public static string? GetRateLimitTimePeriod(byte? rateLimitPeriod) => GetRateLimitTimePeriod((RateLimitPeriods?) rateLimitPeriod);

        private static string? GetRateLimitTimePeriod(RateLimitPeriods? rateLimitPeriod) => rateLimitPeriod switch
        {
            RateLimitPeriods.Second => "s",
            RateLimitPeriods.Minute => "m",
            RateLimitPeriods.Hour => "h",
            _ => null
        };
    }
}