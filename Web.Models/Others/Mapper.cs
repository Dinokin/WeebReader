using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Web.Models.Controllers.BlogManager;
using WeebReader.Web.Models.Controllers.ChaptersManager;
using WeebReader.Web.Models.Controllers.Others;
using WeebReader.Web.Models.Controllers.TitlesManager;
using WeebReader.Web.Models.Controllers.UsersManager;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Others
{
    public static class Mapper
    {
        public static PostModel MapToModel(Post post) => new PostModel
        {
            PostId = post.Id,
            Title = post.Title,
            Content = post.Content,
            ReleaseDate = post.ReleaseDate,
            Visible = post.Visible
        };

        public static Post MapToEntity(PostModel postModel) => postModel.PostId.HasValue ? 
            new Post(postModel.PostId.Value, postModel.Title, postModel.Content, postModel.ReleaseDate ?? DateTime.Now, postModel.Visible) : 
            new Post(postModel.Title, postModel.Content, postModel.ReleaseDate ?? DateTime.Now, postModel.Visible);

        public static void MapEditModelToEntity(PostModel postModel, ref Post post)
        {
            post.Title = postModel.Title;
            post.Content = postModel.Content;
            post.Visible = postModel.Visible;
            post.ReleaseDate = postModel.ReleaseDate ?? post.ReleaseDate;
        }

        public static IdentityUser<Guid> MapToEntity(InstallerModel installerModel) => new IdentityUser<Guid>
        {
            UserName = installerModel.Username,
            Email = installerModel.Email
        };

        public static UserModel MapToModel(IdentityUser<Guid> user) => new UserModel
        {
            UserId = user.Id,
            Username = user.UserName,
            Email = user.Email
        };

        public static IdentityUser<Guid> MapToEntity(UserModel userModel) => new IdentityUser<Guid>
        {
            Id = userModel.UserId ?? Guid.Empty,
            UserName = userModel.Username,
            Email = userModel.Email
        };

        public static void MapEditModelToEntity(UserModel userModel, ref IdentityUser<Guid> user)
        {
            user.UserName = userModel.Username;
            user.Email = userModel.Email;
        }

        public static TitleModel MapToModel(Title title, IEnumerable<Tag>? tags = null) => title switch
        {
            Comic comic => new ComicModel
            {
                TitleId = comic.Id,
                Name = comic.Name,
                OriginalName = comic.OriginalName,
                Author = comic.Author,
                Artist = comic.Artist,
                Synopsis = comic.Synopsis,
                Status = comic.Status,
                Nsfw = comic.Nsfw,
                Visible = comic.Visible,
                LongStrip = comic.LongStrip,
                Tags = tags == null ? null : string.Join(",", tags.Select(tag => tag.Name)),
                PreviousChaptersLink = comic.PreviousChaptersLink
            },
            _ => new TitleModel
            {
                TitleId = title.Id,
                Name = title.Name,
                OriginalName = title.OriginalName,
                Author = title.Author,
                Artist = title.Artist,
                Synopsis = title.Synopsis,
                Status = title.Status,
                Nsfw = title.Nsfw,
                Visible = title.Visible,
                Tags = tags == null ? null : string.Join(",", tags.Select(tag => tag.Name)),
                PreviousChaptersLink = title.PreviousChaptersLink
            }
        };

        public static Title MapToEntity(TitleModel titleModel, string titleType)
        {
            Title result;

            switch (titleType.ToLowerInvariant())
            {
                case "comic":
                    var comicModel = (ComicModel) titleModel;
                    
                    result = comicModel.TitleId.HasValue ? 
                        new Comic(comicModel.TitleId.Value, comicModel.Name, comicModel.OriginalName, comicModel.Author, comicModel.Artist, comicModel.Synopsis, comicModel.Status, comicModel.Nsfw, comicModel.Visible, comicModel.LongStrip, comicModel.PreviousChaptersLink) : 
                        new Comic(comicModel.Name, comicModel.OriginalName, comicModel.Author, comicModel.Artist, comicModel.Synopsis, comicModel.Status, comicModel.Nsfw, comicModel.Visible, comicModel.LongStrip, comicModel.PreviousChaptersLink);
                    break;
                case "novel":
                    result = titleModel.TitleId.HasValue ? 
                        new Novel(titleModel.TitleId.Value, titleModel.Name, titleModel.OriginalName, titleModel.Author, titleModel.Artist, titleModel.Synopsis, titleModel.Status, titleModel.Nsfw, titleModel.Visible, titleModel.PreviousChaptersLink) : 
                        new Novel(titleModel.Name, titleModel.OriginalName, titleModel.Author, titleModel.Artist, titleModel.Synopsis, titleModel.Status, titleModel.Nsfw, titleModel.Visible, titleModel.PreviousChaptersLink);
                    break;
                default:
                    throw new ArgumentException();
            }

            return result;
        }

        public static void MapEditModelToEntity(TitleModel titleModel, ref Title title)
        {
            title.Name = titleModel.Name;
            title.OriginalName = titleModel.OriginalName;
            title.Author = titleModel.Author;
            title.Artist = titleModel.Artist;
            title.Synopsis = titleModel.Synopsis;
            title.Status = titleModel.Status;
            title.Nsfw = titleModel.Nsfw;
            title.Visible = titleModel.Visible;
            title.PreviousChaptersLink = titleModel.PreviousChaptersLink;

            if (title is Comic comic && titleModel is ComicModel comicModel)
                comic.LongStrip = comicModel.LongStrip;
        }

        public static ChapterModel MapToModel(Chapter chapter) => chapter switch
        {
            ComicChapter comicChapter => new ComicChapterModel
            {
                ChapterId = comicChapter.Id,
                Volume = comicChapter.Volume,
                Number = comicChapter.Number,
                Name = comicChapter.Name,
                ReleaseDate = comicChapter.ReleaseDate,
                Visible = comicChapter.Visible,
                TitleId = comicChapter.TitleId
            },
            NovelChapter novelChapter => new NovelChapterModel
            {
                ChapterId = novelChapter.Id,
                Volume = novelChapter.Volume,
                Number = novelChapter.Number,
                Name = novelChapter.Name,
                ReleaseDate = novelChapter.ReleaseDate,
                Visible = novelChapter.Visible,
                TitleId = novelChapter.TitleId,
                Content = novelChapter.NovelChapterContent?.Content ?? string.Empty
            },
            _ => throw new ArgumentException()
        };

        public static Chapter MapToEntity(ChapterModel chapterModel) => chapterModel switch
        {
            ComicChapterModel comicChapterModel => comicChapterModel.ChapterId.HasValue ? 
                new ComicChapter(comicChapterModel.ChapterId.Value, comicChapterModel.Volume, comicChapterModel.Number, comicChapterModel.Name, comicChapterModel.ReleaseDate ?? DateTime.Now, comicChapterModel.Visible, comicChapterModel.TitleId) : 
                new ComicChapter(comicChapterModel.Volume, comicChapterModel.Number, comicChapterModel.Name, comicChapterModel.ReleaseDate ?? DateTime.Now, comicChapterModel.Visible, comicChapterModel.TitleId),
            NovelChapterModel novelChapterModel => novelChapterModel.ChapterId.HasValue ? 
                new NovelChapter(novelChapterModel.ChapterId.Value, novelChapterModel.Volume, novelChapterModel.Number, novelChapterModel.Name, novelChapterModel.ReleaseDate ?? DateTime.Now, novelChapterModel. Visible, novelChapterModel.TitleId) : 
                new NovelChapter(novelChapterModel.Volume, novelChapterModel.Number, novelChapterModel.Name, novelChapterModel.ReleaseDate ?? DateTime.Now, novelChapterModel. Visible, novelChapterModel.TitleId),
            _ => throw new ArgumentException()
        };

        public static void MapEditModelToEntity(ChapterModel chapterModel, ref Chapter chapter)
        {
            chapter.Volume = chapterModel.Volume;
            chapter.Number = chapterModel.Number;
            chapter.Name = chapterModel.Name;
            chapter.ReleaseDate = chapterModel.ReleaseDate ?? chapter.ReleaseDate;
            chapter.Visible = chapterModel.Visible;
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static T Map<T>(IEnumerable<Parameter> parameters) where T : new()
        {
            var t = new T();

            foreach (var property in t.GetType().GetProperties().Where(property => Attribute.IsDefined(property, typeof(ParameterAttribute))))
            {
                var attribute = (ParameterAttribute) property.GetCustomAttribute(typeof(ParameterAttribute))!;
                var parameter = parameters.SingleOrDefault(entity => entity.Type == attribute.ParameterType);

                if (parameter == null)
                    continue;

                property.SetValue(t, parameter.Value == null ? null : Convert.ChangeType(parameter.Value, Nullable.GetUnderlyingType(property.PropertyType) is var underlyingType && underlyingType != null ? underlyingType : property.PropertyType));
            }

            return t;
        }
    }
}