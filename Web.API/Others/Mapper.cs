using System;
using Microsoft.AspNetCore.Identity;
using WeebReader.Web.API.Models.Request.Authentication;

namespace WeebReader.Web.API.Others
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