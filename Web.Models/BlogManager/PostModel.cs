using System;

namespace WeebReader.Web.Models.BlogManager
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
    }
}