using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeebReader.Data.Entities;
using WeebReader.Data.Services;

namespace WeebReader.Web.Services
{
    public partial class ReCaptchaValidator
    {
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