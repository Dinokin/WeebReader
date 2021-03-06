﻿using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeebReader.Data.Services;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Extensions;

namespace WeebReader.Web.Services
{
    public partial class ReCaptchaValidator
    {
        private const string PostAddress = "https://www.google.com/recaptcha/api/siteverify";

        private readonly ParametersManager _parameterManager;
        private readonly HttpClient _httpClient;

        public ReCaptchaValidator(ParametersManager parameterManager, IHttpClientFactory httpClientFactory)
        {
            _parameterManager = parameterManager;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<bool> Validate(string clientSecret, string? clientIp)
        {
            if (!await _parameterManager.GetValue<bool>(ParameterTypes.ContactEmailRecaptchaEnabled))
                return false;
            
            var reCaptchaRequest = new ReCaptchaRequest(await _parameterManager.GetValue<string>(ParameterTypes.ContactEmailRecaptchaServerKey), clientSecret, clientIp);
            var httpResponse = await _httpClient.PostAsync(PostAddress, new FormUrlEncodedContent(Encode(reCaptchaRequest)));
            var reCaptchaResponse = JsonSerializer.Deserialize<ReCaptchaResponse>(await httpResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});

            return httpResponse.IsSuccessStatusCode && reCaptchaResponse.Success;
        }

        private static IEnumerable<KeyValuePair<string?,string?>> Encode(ReCaptchaRequest request)
        {
            var result = new List<KeyValuePair<string?, string?>>
            {
                new(nameof(request.Secret).ToLower(), request.Secret),
                new(nameof(request.Response).ToLower(), request.Response)
            };

            if (request.RemoteIp != null) 
                result.Add(new KeyValuePair<string?, string?>(nameof(request.RemoteIp).ToLower(), request.RemoteIp));

            return result;
        }
    }
}