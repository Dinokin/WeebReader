using System;
using System.Collections.Generic;

namespace WeebReader.Web.API.Models.Response.Users
{
    public class UsersListResponse : PagedResponse
    {
        public IEnumerable<UserResponse> Users { get; set; } = Array.Empty<UserResponse>();
    }
}