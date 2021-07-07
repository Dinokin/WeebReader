using System;
using Microsoft.AspNetCore.Identity;
using WeebReader.Web.API.Models.Request.Installer;

namespace WeebReader.Web.API.Utilities
{
    public static class EntityMapper
    {
        public static IdentityUser<Guid> MapToUser(InstallRequest model) => new()
        {
            UserName = model.Username,
            Email = model.Email
        };
    }
}