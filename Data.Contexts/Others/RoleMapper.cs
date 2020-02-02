namespace WeebReader.Data.Contexts.Others
{
    public static class RoleMapper
    {
        public const string Administrator = "Administrator";
        public const string Moderator = "Moderator";
        public const string Uploader = "Uploader";

        public static string Map(string role) => role switch
        {
            Administrator => ContextMessages.MSG001,
            Moderator => ContextMessages.MSG002,
            Uploader => ContextMessages.MSG003,
            _ => null
        };
    }
}