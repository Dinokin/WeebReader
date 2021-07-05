using System;
using System.Collections.Generic;

namespace WeebReader.Web.API.Models.Response
{
    public class DefaultResponseMessage
    {
        public IEnumerable<string> Messages { get; set; } = Array.Empty<string>();
    }
}