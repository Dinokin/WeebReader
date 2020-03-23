using System;
using Microsoft.AspNetCore.Identity;
using WeebReader.Data.Entities;
using WeebReader.Web.Models.BlogManager;
using WeebReader.Web.Models.Home;
using WeebReader.Web.Models.UsersManager;

namespace WeebReader.Web.Models
{
    public static class Mapper
    {
        public static PostModel Map(Post post)
        {
            return new PostModel
            {
                PostId = post.Id,
                Title = post.Title,
                Content = post.Content,
                ReleaseDate = post.ReleaseDate,
                Visible = post.Visible
            };
        }

        public static Post Map(PostModel postModel)
        {
            return postModel.PostId.HasValue ? 
                new Post(postModel.PostId.Value, postModel.Title, postModel.Content, postModel.ReleaseDate ?? DateTime.Now, postModel.Visible) : 
                new Post(postModel.Title, postModel.Content, postModel.ReleaseDate ?? DateTime.Now, postModel.Visible);
        }

        public static void Map(PostModel postModel, ref Post post)
        {
            post.Title = postModel.Title;
            post.Content = postModel.Content;
            post.Visible = postModel.Visible;
            post.ReleaseDate = postModel.ReleaseDate ?? post.ReleaseDate;
        }

        public static IdentityUser<Guid> Map(InstallerModel installerModel)
        {
            return new IdentityUser<Guid>
            {
                UserName = installerModel.Username,
                Email = installerModel.Email
            };
        }
        
        public static UserModel Map(IdentityUser<Guid> user)
        {
            return new UserModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email
            };
        }
        
        public static IdentityUser<Guid> Map(UserModel userModel)
        {
            return new IdentityUser<Guid>
            {
                Id = userModel.UserId ?? Guid.Empty,
                UserName = userModel.Username,
                Email = userModel.Email
            };
        }

        public static void Map(UserModel userModel, ref IdentityUser<Guid> user)
        {
            user.UserName = userModel.Username;
            user.Email = userModel.Email;
        }
    }
}