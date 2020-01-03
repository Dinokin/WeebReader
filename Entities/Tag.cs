using System.Collections.Generic;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        
        public IEnumerable<TitleTag> TitleTag { get; set; }
    }
}