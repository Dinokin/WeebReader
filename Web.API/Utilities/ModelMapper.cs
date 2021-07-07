using System.Collections.Generic;
using WeebReader.Web.API.Models.Response;
using WeebReader.Web.API.Models.Response.Authentication;

namespace WeebReader.Web.API.Utilities
{
    public static class ModelMapper
    {
        public static DefaultResponse MapToDefaultResponse(string message) => new() {Messages = new[] {message}};
        public static DefaultResponse MapToDefaultResponse(IEnumerable<string> messages) => new() {Messages = messages};
        public static AuthenticationResponse MapToAuthenticationResponse(string token) => new() {Token = token};
    }
}