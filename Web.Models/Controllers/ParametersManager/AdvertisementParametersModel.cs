using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Controllers.ParametersManager
{
    public class AdvertisementParametersModel
    {
        [Parameter(ParameterTypes.AdsEnabled)]
        public bool AdsEnabled { get; set; }
        
        [Parameter(ParameterTypes.AdsHeaderCode)]
        public string? AdsHeaderCode { get; set; }
        
        [Parameter(ParameterTypes.AdsTopSlot)]
        public string? AdsTopSlot { get; set; }
        
        [Parameter(ParameterTypes.AdsBottomSlot)]
        public string? AdsBottomSlot { get; set; }
    }
}