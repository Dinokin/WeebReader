using Services.Abstract;
using WeebReader.Data.Contexts.Abstract;

namespace Services
{
    public class SettingManager : BaseService
    {
        public SettingManager(BaseContext context) : base(context) { }
    }
}