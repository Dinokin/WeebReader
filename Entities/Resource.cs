using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Resource : BaseEntity
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string Type { get; set; }
    }
}