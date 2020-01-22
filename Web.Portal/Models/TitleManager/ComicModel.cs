﻿using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Portal.Models.TitleManager
{
    public class ComicModel : TitleModel
    {
        [Required(ErrorMessage = "The type of comic must be set.")]
        public bool LongStrip { get; set; }
    }
}