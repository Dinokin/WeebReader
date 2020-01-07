using Services.Abstract;
using WeebReader.Data.Contexts.Abstract;

namespace Services
{
    public class PackageManager : BaseService
    {
        public PackageManager(BaseContext context) : base(context) { }
    }
}