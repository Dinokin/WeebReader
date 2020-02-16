﻿using System;
using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.Models.SignIn
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "UserIdRequired")]
        public Guid UserId { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TokenRequired")]
        public string Token { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "NewPasswordRequired")]
        [MinLength(8, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "MinimumPasswordLength")]
        public string NewPassword { get; set; }
    }
}