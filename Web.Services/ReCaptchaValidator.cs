using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(await httpResponse.Content.ReadAsStringAsync());

            return httpResponse.IsSuccessStatusCode && reCaptchaResponse.Success;
        }

        private static IEnumerable<KeyValuePair<string,string>> Encode(ReCaptchaRequest request)
        {
            var result = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(nameof(request.Secret).ToLower(), request.Secret),
                new KeyValuePair<string, string>(nameof(request.Response).ToLower(), request.Response)
            };

            if (request.RemoteIp != null) 
                result.Add(new KeyValuePair<string, string>(nameof(request.RemoteIp).ToLower(), request.RemoteIp));

            return result;
        }
    }
}