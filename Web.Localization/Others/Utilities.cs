using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Web.Localization.Others
{
    public static class Utilities
    {
        public static class Roles
        {
            public const string Administrator = "Administrator";
            public const string Moderator = "Moderator";
            public const string Uploader = "Uploader";
        }

        /// <summary>
        /// Convert role to translatable string
        /// </summary>
        /// <param name="role">Role string</param>
        /// <returns>Translatable string</returns>
        public static string FromRole(string? role) => role switch
        {
            Roles.Administrator => Labels.Administrator,
            Roles.Moderator => Labels.Moderator,
            Roles.Uploader => Labels.Uploader,
            _ => Labels.None
        };

        /// <summary>
        /// Convert translatable string to role string
        /// </summary>
        /// <param name="message">Translatable string</param>
        /// <returns>Role string, null if not found</returns>
        public static string? ToRole(string? message)
        {
            if (message == Labels.Administrator)
                return Roles.Administrator;
            
            if (message == Labels.Moderator)
                return Roles.Moderator;
            
            if (message == Labels.Uploader)
                return Roles.Uploader;

            return null;
        }
        
        public static string FromStatus(Title.Statuses status) => status switch
        {
            Title.Statuses.Ongoing => Labels.Ongoing,
            Title.Statuses.Hiatus => Labels.Hiatus,
            Title.Statuses.Completed => Labels.Completed,
            Title.Statuses.Dropped => Labels.Dropped,
            _ => string.Empty
        };
    }
}