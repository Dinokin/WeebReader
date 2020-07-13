using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;

namespace WeebReader.Web.Portal.Others
{
    public static class Utilities
    {
        public static string EncodeToBase64(this string? source) => source == null ? string.Empty : WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(source));

        public static string DecodeFromBase64(this string? source) => source == null ? string.Empty : Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(source));

        public static string RemoveHtmlTags(this string? source) => source == null ? string.Empty : Regex.Replace(source, "<.*?>", string.Empty);

        public static string AddImgLazyLoading(this string? source, string placeholderLocation)
        {
            if (source == null)
                return string.Empty;
            
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(source);
            var imageNodes = htmlDocument.DocumentNode.SelectNodes("//img")?.ToArray() ?? new HtmlNode[0];

            Parallel.ForEach(imageNodes, node =>
            {
                var src = node.GetAttributeValue("src", string.Empty);

                node.SetAttributeValue("data-src", src);
                node.SetAttributeValue("src", placeholderLocation);
            });

            return htmlDocument.DocumentNode.OuterHtml;
        }
    }
}