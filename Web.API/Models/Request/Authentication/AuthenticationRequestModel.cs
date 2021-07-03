namespace WeebReader.Web.API.Models.Request.Authentication
{
    public class AuthenticationRequestModel
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}