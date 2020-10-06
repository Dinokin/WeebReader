using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace WeebReader.Web.Services
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
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

            public ReCaptchaResponse(bool success, DateTime challengeTimestamp, string hostname, IEnumerable<string>? errorCodes)
            {
                Success = success;
                ChallengeTimestamp = challengeTimestamp;
                Hostname = hostname;
                ErrorCodes = errorCodes;
            }
        }
    }
}