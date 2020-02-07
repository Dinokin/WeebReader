namespace WeebReader.Data.Contexts.Others
{
    public static class RoleMapper
    {
        public const string Administrator = "Administrator";
        public const string Moderator = "Moderator";
        public const string Uploader = "Uploader";

        /// <summary>
        ///     Convert role to translatable string.
        /// </summary>
        public static string Map(string role) => role switch
        {
            Administrator => ContextMessages.MSG001,
            Moderator => ContextMessages.MSG002,
            Uploader => ContextMessages.MSG003,
            _ => null
        };

        /// <summary>
        ///     Convert translatable string to role.
        /// </summary>
        public static string UnMap(string role)
        {
            if (role == ContextMessages.MSG001)
                return Administrator;
            
            if (role == ContextMessages.MSG002)
                return Administrator;
            
            if (role == ContextMessages.MSG003)
                return Uploader;
            
            return null;
        }
    }
}