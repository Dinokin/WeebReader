using System;

namespace WeebReader.Web.API.Models.Response.Users
{
    public class UserResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public bool IsBlocked { get; set; }
    }
}