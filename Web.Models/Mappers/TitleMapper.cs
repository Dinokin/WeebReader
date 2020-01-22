﻿using WeebReader.Data.Entities;
using WeebReader.Web.Models.Models.TitleManager;

namespace WeebReader.Web.Models.Mappers
{
    public static class TitleMapper
    {
        public static Comic MapComicToEntity(ComicModel comicModel)
        {
            return new Comic
            {
                Name = comicModel.Name,
                OriginalName = comicModel.OriginalName,
                Author = comicModel.Author,
                Artist = comicModel.Artist,
                Synopsis = comicModel.Synopsis,
                Status = comicModel.Status,
                Visible = comicModel.Visible,
                LongStrip = comicModel.LongStrip
            };
        }
    }
}