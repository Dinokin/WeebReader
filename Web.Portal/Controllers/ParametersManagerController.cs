using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Localization.Utilities;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Attributes;
using WeebReader.Web.Models.ParametersManager;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize(Roles = RoleTranslator.Administrator)]
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
        public IActionResult General() => View(Mapper.Map<GeneralParametersModel>(_parametersManager.Entities));
        
        [HttpPatch("{action}")]
        public async Task<IActionResult> General(GeneralParametersModel generalParametersModel)
        {
            if (TryValidateModel(generalParametersModel))
            {
                var result = await SaveParameters(generalParametersModel);

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

        [HttpGet("{action}")]
        public IActionResult Email() => View(Mapper.Map<EmailParametersModel>(_parametersManager.Entities));
        
        [HttpPatch("{action}")]
        public async Task<IActionResult> Email(EmailParametersModel emailParametersModel)
        {
            if (TryValidateModel(emailParametersModel))
            {
                if (emailParametersModel.EmailEnabled)
                    if (string.IsNullOrWhiteSpace(emailParametersModel.SmtpServer) || emailParametersModel.SmtpServerPort == null || string.IsNullOrWhiteSpace(emailParametersModel.SmtpServerUser) || string.IsNullOrWhiteSpace(emailParametersModel.SmtpServerPassword))
                        return new JsonResult(new
                        {
                            success = false,
                            messages = new[] {ValidationMessages.SmtpServerInformationRequired}
                        });
                
                var result = await SaveParameters(emailParametersModel);

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

        [HttpGet("{action}")]
        public IActionResult Contact() => View(Mapper.Map<ContactParametersModel>(_parametersManager.Entities));
        
        [HttpPatch("{action}")]
        public async Task<IActionResult> Contact(ContactParametersModel contactParametersModel)
        {
            if (TryValidateModel(contactParametersModel))
            {
                var result = await SaveParameters(contactParametersModel);

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

        [HttpGet("{action}")]
        public IActionResult Pages() => View(Mapper.Map<PagesParametersModel>(_parametersManager.Entities));
        
        [HttpPatch("{action}")]
        public async Task<IActionResult> Pages(PagesParametersModel pagesParametersModel)
        {
            if (TryValidateModel(pagesParametersModel))
            {
                var result = await SaveParameters(pagesParametersModel);

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