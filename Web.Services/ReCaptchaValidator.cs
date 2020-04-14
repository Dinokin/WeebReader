using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeebReader.Data.Entities;
using WeebReader.Data.Services;

namespace WeebReader.Web.Services
{
    public class ReCaptchaValidator
    {
        private class ReCaptchaRequest
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

        private class ReCaptchaResponse
        {
            public bool Success { get; set; }
            
            [JsonProperty("challenge_ts")]
            public DateTime ChallengeTimestamp { get; set; }
            
            public string Hostname { get; set; }
            
            [JsonProperty("error-codes")]
            public IEnumerable<string>? ErrorCodes { get; set; }

            public ReCaptchaResponse(bool success, DateTime challengeTimestamp, string hostname, IEnumerable<string>? errorCodes)
            {
                Success = success;
                ChallengeTimestamp = challengeTimestamp;
                Hostname = hostname;
                ErrorCodes = errorCodes;
            }
        }

        private const string PostAddress = "https://www.google.com/recaptcha/api/siteverify";

        private readonly ParametersManager _parameterManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public ReCaptchaValidator(ParametersManager parameterManager, IHttpClientFactory httpClientFactory)
        {
            _parameterManager = parameterManager;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> Validate(string clientSecret, string? clientIp)
        {
            if (await _parameterManager.GetValue<bool>(Parameter.Types.ContactEmailRecaptchaEnabled))
            {
                var reCaptchaRequest = new ReCaptchaRequest(await _parameterManager.GetValue<string>(Parameter.Types.ContactEmailRecaptchaServerKey), clientSecret, clientIp);
                var content = new FormUrlEncodedContent(Encode(reCaptchaRequest));
                var httpResponse = await _httpClientFactory.CreateClient().PostAsync(PostAddress, content);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(await httpResponse.Content.ReadAsStringAsync());

                    return reCaptchaResponse.Success;
                }

                return false;
            }

            return true;
        }

        private static IEnumerable<KeyValuePair<string,string>> Encode(ReCaptchaRequest request)
        {
            var result = new List<KeyValuePair<string,string>>();
            result.Add(new KeyValuePair<string, string>(nameof(request.Secret).ToLower(), request.Secret));
            result.Add(new KeyValuePair<string, string>(nameof(request.Response).ToLower(), request.Response));

            if (request.RemoteIp != null) 
                result.Add(new KeyValuePair<string, string>(nameof(request.RemoteIp).ToLower(), request.RemoteIp));

            return result;
        }
    }
}