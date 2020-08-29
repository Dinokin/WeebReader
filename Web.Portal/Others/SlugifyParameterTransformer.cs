using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace WeebReader.Web.Portal.Others
{
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value) =>  Regex.Replace(value.ToString() ?? string.Empty, "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}