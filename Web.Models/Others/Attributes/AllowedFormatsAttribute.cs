using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace WeebReader.Web.Models.Others.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AllowedFormatsAttribute : ValidationAttribute
    {
        /// <summary>
        ///     List of comma separated file extensions to be tested against an <see cref="IFormFile"/>.
        /// </summary>
        public string AllowedExtensions { get; }

        public AllowedFormatsAttribute(string allowedExtensions) => AllowedExtensions = allowedExtensions;

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            var allowedExtensions = AllowedExtensions.Replace(",", "|").ToLowerInvariant();
            
            if (value is IFormFile formFile) 
                if (Regex.IsMatch(Path.GetExtension(formFile.FileName.ToLowerInvariant()), $"({allowedExtensions})"))
                    return true;

            return false;
        }
    }
}