namespace WeebReader.Web.API.Settings
{
    public class MariaDbConfiguration
    {
        public string Address { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string ConnectionString => $"Server={Address};Database={Database};Uid={User};Pwd={Password};";
    }
}