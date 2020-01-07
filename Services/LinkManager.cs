using Services.Abstract;
using WeebReader.Data.Contexts.Abstract;

namespace Services
{
    public class LinkManager : BaseService
    {
        public LinkManager(BaseContext context) : base(context) { }
    }
}