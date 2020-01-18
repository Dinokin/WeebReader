using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Setting : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}