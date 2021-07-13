using System;
using System.Collections.Generic;

namespace WeebReader.Web.API.Models.Response
{
    public class DefaultResponse
    {
        public IEnumerable<string> Messages { get; init; } = Array.Empty<string>();
    }
}