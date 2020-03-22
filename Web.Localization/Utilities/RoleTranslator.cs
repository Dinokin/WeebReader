namespace WeebReader.Web.Localization.Utilities
{
    public static class RoleTranslator
    {
        public const string Administrator = "Administrator";
        public const string Moderator = "Moderator";
        public const string Uploader = "Uploader";

        /// <summary>
        /// Convert role to translatable string
        /// </summary>
        /// <param name="role">Role string</param>
        /// <returns>Translatable string</returns>
        public static string FromRole(string? role) => role switch
        {
            Administrator => Labels.Administrator,
            Moderator => Labels.Moderator,
            Uploader => Labels.Uploader,
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
                return Administrator;
            
            if (message == Labels.Moderator)
                return Moderator;
            
            if (message == Labels.Uploader)
                return Uploader;

            return null;
        }
    }
}