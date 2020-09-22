using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Attributes;
using WeebReader.Web.Models.Others.Extensions;

namespace WeebReader.Web.Models.Controllers.ParametersManager
{
    public class RateLimitParametersModel : IValidatableObject
    {
        [Parameter(ParameterTypes.RateLimitRealIpHeader)]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "IpHeaderRequired")]
        public string? RateLimitRealIpHeader { get; set; }
        
        [Parameter(ParameterTypes.RateLimitContentEnabled)]
        public bool RateLimitContentEnabled { get; set; }
        
        [Parameter(ParameterTypes.RateLimitApiEnabled)]
        public bool RateLimitApiEnabled { get; set; }

        [Parameter(ParameterTypes.RateLimitMaxContentRequests)]
        public double? RateLimitMaxContentRequests { get; set; }
        
        [Parameter(ParameterTypes.RateLimitMaxApiRequests)]
        public double? RateLimitMaxApiRequests { get; set; }

        [Parameter(ParameterTypes.RateLimitPeriodContent)]
        public byte? RateLimitPeriodContent { get; set; }
        
        [Parameter(ParameterTypes.RateLimitPeriodApi)]
        public byte? RateLimitPeriodApi { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (RateLimitContentEnabled)
            {
                if (Utilities.GetRateLimitTimePeriod(RateLimitPeriodContent) == null)
                    results.Add(new ValidationResult(ValidationMessages.ValidContentTimeIntervalIsRequired, new []{nameof(RateLimitPeriodContent)}));

                if (!RateLimitMaxContentRequests.HasValue)
                    results.Add(new ValidationResult(ValidationMessages.ValidContentAmountOfRequestsIsRequired, new []{nameof(RateLimitMaxContentRequests)}));

                if (!RateLimitMaxContentRequests.Between(1, 10000))
                    results.Add(new ValidationResult(ValidationMessages.AmountOfContentRequestMustBeBetween1And10000, new []{nameof(RateLimitMaxContentRequests)}));
            }

            if (RateLimitApiEnabled)
            {
                if (Utilities.GetRateLimitTimePeriod(RateLimitPeriodApi) == null)
                    results.Add(new ValidationResult(ValidationMessages.ValidApiTimeIntervalIsRequired, new []{nameof(RateLimitPeriodApi)}));

                if (!RateLimitMaxApiRequests.HasValue)
                    results.Add(new ValidationResult(ValidationMessages.ValidApiAmountOfRequestsIsRequired, new []{nameof(RateLimitMaxApiRequests)}));

                if (!RateLimitMaxContentRequests.Between(1, 10000))
                    results.Add(new ValidationResult(ValidationMessages.AmountOfApiRequestMustBeBetween1And10000, new []{nameof(RateLimitMaxApiRequests)}));
            }
            
            return results;
        }
    }
}