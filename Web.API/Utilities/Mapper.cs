using System;
using Microsoft.AspNetCore.Identity;
using WeebReader.Web.API.Models.Request.Installer;

namespace WeebReader.Web.API.Utilities
{
    public static class Mapper
    {
        public static IdentityUser<Guid> MapToEntity(InstallRequestModel model) => new()
        {
            UserName = model.Username,
            Email = model.Email
        };
    }
}