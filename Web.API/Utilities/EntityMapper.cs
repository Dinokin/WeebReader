using System;
using Microsoft.AspNetCore.Identity;
using WeebReader.Web.API.Models.Request.Installer;
using WeebReader.Web.API.Models.Request.Users;

namespace WeebReader.Web.API.Utilities
{
    public static class EntityMapper
    {
        public static IdentityUser<Guid> MapToUser(InstallRequest model) => new()
        {
            UserName = model.Username,
            Email = model.Email
        };
        
        public static IdentityUser<Guid> MapToUser(AddUserRequest model) => new()
        {
            UserName = model.Username,
            Email = model.Email
        };
    }
}