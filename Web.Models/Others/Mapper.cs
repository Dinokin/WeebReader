using System;
using System.Collections.Generic;
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
        public static PostModel Map(Post post) => new PostModel
        {
            PostId = post.Id,
            Title = post.Title,
            Content = post.Content,
            ReleaseDate = post.ReleaseDate,
            Visible = post.Visible
        };

        public static Post Map(PostModel postModel) => postModel.PostId.HasValue ? 
            new Post(postModel.PostId.Value, postModel.Title, postModel.Content, postModel.ReleaseDate ?? DateTime.Now, postModel.Visible) : 
            new Post(postModel.Title, postModel.Content, postModel.ReleaseDate ?? DateTime.Now, postModel.Visible);

        public static void Map(PostModel postModel, ref Post post)
        {
            post.Title = postModel.Title;
            post.Content = postModel.Content;
            post.Visible = postModel.Visible;
            post.ReleaseDate = postModel.ReleaseDate ?? post.ReleaseDate;
        }

        public static IdentityUser<Guid> Map(InstallerModel installerModel) => new IdentityUser<Guid>
        {
            UserName = installerModel.Username,
            Email = installerModel.Email
        };

        public static UserModel Map(IdentityUser<Guid> user) => new UserModel
        {
            UserId = user.Id,
            Username = user.UserName,
            Email = user.Email
        };

        public static IdentityUser<Guid> Map(UserModel userModel) => new IdentityUser<Guid>
        {
            Id = userModel.UserId ?? Guid.Empty,
            UserName = userModel.Username,
            Email = userModel.Email
        };

        public static void Map(UserModel userModel, ref IdentityUser<Guid> user)
        {
            user.UserName = userModel.Username;
            user.Email = userModel.Email;
        }

        public static TitleModel Map(Title title, IEnumerable<Tag>? tags = null) => new TitleModel
        {
            TitleId = title.Id,
            Name = title.Name,
            OriginalName = title.OriginalName,
            Author = title.Author,
            Artist = title.Artist,
            Synopsis = title.Synopsis,
            Status = title.Status,
            Visible = title.Visible,
            Tags = tags == null ? null : string.Join(",", tags.Select(tag => tag.Name)),
            PreviousChapterLink = title.PreviousChapterLink
        };

        private static void Map(TitleModel titleModel, ref Title title)
        {
            title.Name = titleModel.Name;
            title.OriginalName = titleModel.OriginalName;
            title.Author = titleModel.Author;
            title.Artist = titleModel.Artist;
            title.Synopsis = titleModel.Synopsis;
            title.Status = titleModel.Status;
            title.Visible = titleModel.Visible;
            title.PreviousChapterLink = titleModel.PreviousChapterLink;
        }
        
        public static ComicModel Map(Comic comic, IEnumerable<Tag>? tags = null) => new ComicModel
        {
            TitleId = comic.Id,
            Name = comic.Name,
            OriginalName = comic.OriginalName,
            Author = comic.Author,
            Artist = comic.Artist,
            Synopsis = comic.Synopsis,
            Status = comic.Status,
            Visible = comic.Visible,
            LongStrip = comic.LongStrip,
            Tags = tags == null ? null : string.Join(",", tags.Select(tag => tag.Name)),
            PreviousChapterLink = comic.PreviousChapterLink
        };
        
        public static Comic Map(ComicModel comicModel) => comicModel.TitleId.HasValue ? 
            new Comic(comicModel.TitleId.Value, comicModel.Name, comicModel.OriginalName, comicModel.Author, comicModel.Artist, comicModel.Synopsis, comicModel.Status, comicModel.Visible, comicModel.LongStrip, comicModel.PreviousChapterLink) : 
            new Comic(comicModel.Name, comicModel.OriginalName, comicModel.Author, comicModel.Artist, comicModel.Synopsis, comicModel.Status, comicModel.Visible, comicModel.LongStrip, comicModel.PreviousChapterLink);

        public static void Map(ComicModel comicModel, ref Comic comic)
        {
            var title = (Title) comic;
            Map(comicModel, ref title);
            
            comic.LongStrip = comicModel.LongStrip;
        }

        public static ChapterModel Map(Chapter chapter) => new ChapterModel
        {
            ChapterId = chapter.Id,
            Volume = chapter.Volume,
            Number = chapter.Number,
            Name = chapter.Name,
            ReleaseDate = chapter.ReleaseDate,
            Visible = chapter.Visible,
            TitleId = chapter.TitleId
        };
        
        public static void Map(ChapterModel chapterModel, ref Chapter chapter)
        {
            chapter.Volume = chapterModel.Volume;
            chapter.Number = chapterModel.Number;
            chapter.Name = chapterModel.Name;
            chapter.ReleaseDate = chapterModel.ReleaseDate ?? chapter.ReleaseDate;
            chapter.Visible = chapterModel.Visible;
        }
        
        public static ComicChapterModel Map(ComicChapter chapter) => new ComicChapterModel
        {
            ChapterId = chapter.Id,
            Volume = chapter.Volume,
            Number = chapter.Number,
            Name = chapter.Name,
            ReleaseDate = chapter.ReleaseDate,
            Visible = chapter.Visible,
            TitleId = chapter.TitleId
        };
        
        public static ComicChapter Map(ComicChapterModel comicChapterModel) => comicChapterModel.ChapterId.HasValue ? 
            new ComicChapter(comicChapterModel.ChapterId.Value, comicChapterModel.Volume, comicChapterModel.Number, comicChapterModel.Name, comicChapterModel.ReleaseDate ?? DateTime.Now, comicChapterModel.Visible, comicChapterModel.TitleId) : 
            new ComicChapter(comicChapterModel.Volume, comicChapterModel.Number, comicChapterModel.Name, comicChapterModel.ReleaseDate ?? DateTime.Now, comicChapterModel.Visible, comicChapterModel.TitleId);

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