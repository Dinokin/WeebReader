using System.Collections.Generic;

namespace WeebReader.Web.API.Data.Entities.Abstract
{
    public abstract class Title : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? OriginalName { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string? Synopsis { get; set; }
        public ScanlationStatuses Status { get; set; }
        public bool Visible { get; set; }
        
        public IEnumerable<TitleTag>? TitleTags { get; set; }
    }
}