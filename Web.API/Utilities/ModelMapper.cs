using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using WeebReader.Web.API.Models.Response;
using WeebReader.Web.API.Models.Response.Authentication;
using WeebReader.Web.API.Models.Response.Users;

namespace WeebReader.Web.API.Utilities
{
    public static class ModelMapper
    {
        public static DefaultResponse MapToDefaultResponse(string message) => new() {Messages = new[] {message}};
        
        public static DefaultResponse MapToDefaultResponse(IEnumerable<string> messages) => new() {Messages = messages};
        
        public static AuthenticationResponse MapToAuthenticationResponse(string token) => new() {Token = token};

        public static UsersListResponse MapToUsersListResponse(ushort page, ushort totalPages, IEnumerable<(IdentityUser<Guid> User, IdentityRole<Guid>? Role)> users) => new()
        {
            Page = page,
            TotalPages = totalPages,
            Users = users.Select(tuple => MapToUserResponse(tuple.User, tuple.Role))
        };

        public static UserResponse MapToUserResponse(IdentityUser<Guid> user, IdentityRole<Guid>? role) => new()
        {
            UserId = user.Id,
            Username = user.Email,
            Email = user.Email,
            Role = role?.Name ?? Identity.Roles.None,
            IsBlocked = Identity.IsUserBlocked(user)
        };
    }
}