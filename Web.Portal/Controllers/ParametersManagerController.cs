﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Localization.Others;
using WeebReader.Web.Models.Controllers.ParametersManager;
using WeebReader.Web.Models.Others.Attributes;
using WeebReader.Web.Models.Others.Extensions;

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

        public ParametersManagerController(ParametersManager parametersManager) => _parametersManager = parametersManager;

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

        private async Task<JsonResult> ProcessPatchRequest(object model)
        {
            if (ModelState.IsValid)
            {
                var result = await SaveParameters(model);

                switch (result)
                {
                    case SaveResult.Success:
                        TempData["SuccessMessage"] = new[] {OtherMessages.ParametersUpdatedSuccessfully};
                    
                        return new JsonResult(new
                        {
                            success = true
                        });
                    case SaveResult.Partial:
                        TempData["ErrorMessage"] = new[] {OtherMessages.SomeParametersFailedToUpdate};

                        return new JsonResult(new
                        {
                            success = true
                        });
                    default:
                        ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
                        break;
                }
            }
            
            return new JsonResult(new
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