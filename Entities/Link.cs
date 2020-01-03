using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Link : BaseEntity
    {
        public string Name { get; set; }
        public string Destination { get; set; }
        public bool Active { get; set; }
    }
}