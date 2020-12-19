using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WeebReader.Web.Services
{
    public partial class ReCaptchaValidator
    {
        private struct ReCaptchaRequest
        {
            public string Secret { get; set; }
            
            public string Response { get; set; }
            
            public string? RemoteIp { get; set; }

            public ReCaptchaRequest(string secret, string response, string? remoteIp)
            {
                Secret = secret;
                Response = response;
                RemoteIp = remoteIp;
            }
        }

        private struct ReCaptchaResponse
        {
            public bool Success { get; set; }
            
            [JsonPropertyName("challenge_ts")]
            public DateTime ChallengeTimestamp { get; set; }
            
            public string Hostname { get; set; }
            
            [JsonPropertyName("error-codes")]
            public IEnumerable<string>? ErrorCodes { get; set; }
        }
    }
}