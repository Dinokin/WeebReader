﻿using System;
using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Portal.Models.Shared;

namespace WeebReader.Web.Portal.Models.SignIn
{
    public class ChangeEmailModel : EmailModel
    {
        [Required(ErrorMessage = "An user ID is necessary to reset a password.")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "A token is necessary to reset a password.")]
        public string Token { get; set; }
    }
}