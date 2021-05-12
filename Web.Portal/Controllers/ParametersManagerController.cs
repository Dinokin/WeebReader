using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Controllers.ParametersManager;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Attributes;
using WeebReader.Web.Models.Others.Extensions;
using WeebReader.Web.Portal.Others;
using Utilities = WeebReader.Web.Localization.Others.Utilities;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize(Roles = Utilities.Roles.Administrator)]
    [Route("Admin/Parameters/")]
    public class ParametersManagerController : Controller
    {
        private enum SaveResult
        {
            Error,
            Partial,
            Success,
        }
        
        private readonly ParametersManager _parametersManager;
        private readonly IpRateLimitOptions _rateLimitOptions;
        private readonly IIpPolicyStore _ipPolicyStore;

        public ParametersManagerController(ParametersManager parametersManager, IOptions<IpRateLimitOptions> rateLimitOptions, IIpPolicyStore ipPolicyStore)
        {
            _parametersManager = parametersManager;
            _rateLimitOptions = rateLimitOptions.Value;
            _ipPolicyStore = ipPolicyStore;
        }

        [HttpGet("{action}")]
        public async Task<IActionResult> General() => View(await _parametersManager.GetModel<GeneralParametersModel>());
        
        [HttpPatch("{action}")]
        public async Task<IActionResult> General(GeneralParametersModel generalParametersModel) => await ProcessPatchRequest(generalParametersModel);

        [HttpGet("{action}")]
        public async Task<IActionResult> Email() => View(await _parametersManager.GetModel<EmailParametersModel>());

        [HttpPatch("{action}")]
        public async Task<IActionResult> Email(EmailParametersModel emailParametersModel) => await ProcessPatchRequest(emailParametersModel);

        [HttpGet("{action}")]
        public async Task<IActionResult> Contact() => View(await _parametersManager.GetModel<ContactParametersModel>());

        [HttpPatch("{action}")]
        public async Task<IActionResult> Contact(ContactParametersModel contactParametersModel) => await ProcessPatchRequest(contactParametersModel);

        [HttpGet("{action}")]
        public async Task<IActionResult> Pages() => View(await _parametersManager.GetModel<PagesParametersModel>());

        [HttpPatch("{action}")]
        public async Task<IActionResult> Pages(PagesParametersModel pagesParametersModel) => await ProcessPatchRequest(pagesParametersModel);

        [HttpGet("{action}")]
        public async Task<IActionResult> RateLimit()
        {
            var model = await _parametersManager.GetModel<RateLimitParametersModel>();

            model.RateLimitRealIpHeader ??= _rateLimitOptions.RealIpHeader;
            model.RateLimitPeriodContent ??= (byte) RateLimitPeriods.Second;
            model.RateLimitPeriodApi ??= (byte) RateLimitPeriods.Second;
            model.RateLimitMaxContentRequests ??= Constants.RateLimitDefaultRequestLimit;
            model.RateLimitMaxApiRequests ??= Constants.RateLimitDefaultRequestLimit;

            return View(model);
        }

        [HttpPatch("{action}")]
        public async Task<IActionResult> RateLimit(RateLimitParametersModel rateLimitParametersModel)
        {
            if (ModelState.IsValid)
            {
                _rateLimitOptions.GeneralRules.Clear();
                _rateLimitOptions.RealIpHeader = rateLimitParametersModel.RateLimitRealIpHeader;
                
                if (rateLimitParametersModel.RateLimitContentEnabled)
                    _rateLimitOptions.GeneralRules.AddRange(new [] {
                        new RateLimitRule
                        {
                            Endpoint = Constants.RateLimitEndpointContent,
                            Limit = rateLimitParametersModel.RateLimitMaxContentRequests ?? Constants.RateLimitDefaultRequestLimit,
                            Period = $"1{Models.Others.Utilities.GetRateLimitTimePeriod(rateLimitParametersModel.RateLimitPeriodContent) ?? Constants.RateLimitDefaultTimeInterval}"
                        },
                        new RateLimitRule
                        {
                            Endpoint = Constants.RateLimitEndpointContentJson,
                            Limit = rateLimitParametersModel.RateLimitMaxContentRequests ?? Constants.RateLimitDefaultRequestLimit,
                            Period = $"1{Models.Others.Utilities.GetRateLimitTimePeriod(rateLimitParametersModel.RateLimitPeriodContent) ?? Constants.RateLimitDefaultTimeInterval}"
                        }
                    });
                
                if (rateLimitParametersModel.RateLimitApiEnabled)
                    _rateLimitOptions.GeneralRules.Add(new RateLimitRule
                    {
                        Endpoint = Constants.RateLimitEndpointApi,
                        Limit = rateLimitParametersModel.RateLimitMaxApiRequests ?? Constants.RateLimitDefaultRequestLimit,
                        Period = $"1{Models.Others.Utilities.GetRateLimitTimePeriod(rateLimitParametersModel.RateLimitPeriodApi) ?? Constants.RateLimitDefaultTimeInterval}"
                    });

                await _ipPolicyStore.SeedAsync();
            }

            return await ProcessPatchRequest(rateLimitParametersModel);
        }

        private async Task<JsonResult> ProcessPatchRequest(object model)
        {
            if (ModelState.IsValid)
            {
                var result = await SaveParameters(model);

                switch (result)
                {
                    case SaveResult.Success:
                        TempData["SuccessMessage"] = new[] {OtherMessages.ParametersUpdatedSuccessfully};
                    
                        return Json(new
                        {
                            success = true
                        });
                    case SaveResult.Partial:
                        TempData["ErrorMessage"] = new[] {OtherMessages.SomeParametersFailedToUpdate};

                        return Json(new
                        {
                            success = true
                        });
                    default:
                        ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
                        break;
                }
            }
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        private async Task<SaveResult> SaveParameters(object model)
        {
            var resultList = new List<bool>();
            
            foreach (var property in model.GetType().GetProperties().Where(property => Attribute.IsDefined(property, typeof(ParameterAttribute))))
            {
                var attribute = (ParameterAttribute) property.GetCustomAttribute(typeof(ParameterAttribute))!;
                
                resultList.Add(await _parametersManager.Save(attribute.ParameterType, property.GetValue(model)?.ToString()));
            }

            return resultList.All(value => value) ? SaveResult.Success : resultList.All(value => !value) ? SaveResult.Error : SaveResult.Partial;
        }
    }
}