using Services.Abstract;
using WeebReader.Data.Contexts.Abstract;

namespace Services
{
    public class ResourceManager : BaseService
    {
        public ResourceManager(BaseContext context) : base(context) { }
    }
}