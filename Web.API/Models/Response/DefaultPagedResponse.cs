namespace WeebReader.Web.API.Models.Response
{
    public abstract class PagedResponse
    {
        public ushort Page { get; set; }
        public ushort TotalPages { get; set; }
    }
}