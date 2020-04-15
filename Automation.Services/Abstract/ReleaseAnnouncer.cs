using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;

namespace WeebReader.Automation.Services.Abstract
{
    public abstract class ReleaseAnnouncer
    {
        protected readonly IConfiguration Configuration;
        protected readonly ParametersManager ParametersManager;

        protected ReleaseAnnouncer(IConfiguration configuration, ParametersManager parametersManager)
        {
            Configuration = configuration;
            ParametersManager = parametersManager;
        }

        public abstract Task<bool> Announce(Title title, Chapter chapter);
    }
}