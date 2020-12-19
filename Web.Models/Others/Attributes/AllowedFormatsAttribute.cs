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
        private readonly string _allowedExtensions;

        public AllowedFormatsAttribute(string allowedExtensions) => _allowedExtensions = allowedExtensions.Replace(",", "|").ToLowerInvariant();

        public override bool IsValid(object? value)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (value is IFormFile formFile && formFile != null)
                return Regex.IsMatch(Path.GetExtension(formFile.FileName.ToLowerInvariant()), $"({_allowedExtensions})");

            return true;
        }
    }
}