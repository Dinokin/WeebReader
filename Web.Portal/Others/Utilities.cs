﻿using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;

namespace WeebReader.Web.Portal.Others
{
    public static class Utilities
    {
        public static string EncodeToBase64(this string? source) => source == null ? string.Empty : WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(source));

        public static string DecodeFromBase64(this string? source) => source == null ? string.Empty : Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(source));

        public static string RemoveHtmlTags(this string? source) => source == null ? string.Empty : Regex.Replace(source, "<.*?>", string.Empty);
    }
}