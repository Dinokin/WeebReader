using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace WeebReader.Web.Models.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class AllowedFormats : ValidationAttribute
    {
        public string AllowedExtensions { get; set; }

        public AllowedFormats(string allowedExtensions) => AllowedExtensions = allowedExtensions;

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