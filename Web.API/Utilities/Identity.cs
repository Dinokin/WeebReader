using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WeebReader.Web.API.Utilities
{
    public static class Identity
    {
        public static class Roles
        {
            public const string Administrator = "Administrator";
            public const string Moderator = "Moderator";
            public const string Uploader = "Uploader";
            public const string None = "None";

            public static IEnumerable<string> GetRoles() => new[] {Administrator, Moderator, Uploader};
        }

        public static bool IsUserBlocked(IdentityUser<Guid> user) => user.LockoutEnd > DateTimeOffset.Now || !user.EmailConfirmed;
    }
}