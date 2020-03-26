using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Web.Models.TitlesManager
{
    public class TitleModel
    {
        public Guid? TitleId { get; set; }

        public string Name { get; set; } = string.Empty;
        
        public string? OriginalName { get; set; }

        public string Author { get; set; } = string.Empty;

        public string Artist { get; set; } = string.Empty;
        
        public string? Synopsis { get; set; }

        public Title.Statuses Status { get; set; }
        
        public bool Visible { get; set; }
        
        public string[] Tags { get; set; } = new string[0];
    }
}