namespace WeebReader.Web.API.Models.Response
{
    public abstract class PagedResponse
    {
        public ushort Page { get; init; }
        public ushort TotalPages { get; init; }
    }
}