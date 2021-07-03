using System.Collections.Generic;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; init; } = string.Empty;
        
        public IEnumerable<TitleTag>? TitleTag { get; set; }
    }
}