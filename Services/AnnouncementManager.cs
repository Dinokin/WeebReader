using Services.Abstract;
using WeebReader.Data.Contexts.Abstract;

namespace Services
{
    public class AnnouncementManager : BaseService
    {
        public AnnouncementManager(BaseContext context) : base(context) { }
    }
}