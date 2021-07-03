using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeebReader.Web.API.Data.Contexts.Abstract;
using WeebReader.Web.API.Data.Entities;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Contexts
{
    public class MariaDBContext : BaseContext
    {
        public MariaDBContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser<Guid>>(typeBuilder =>
            {
                typeBuilder.Property(user => user.EmailConfirmed).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.PhoneNumberConfirmed).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.TwoFactorEnabled).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.LockoutEnd).HasColumnType("DATETIME");
                typeBuilder.Property(user => user.LockoutEnabled).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.AccessFailedCount).HasColumnType("INT");
            });
            
            builder.Entity<Title>(typeBuilder =>
            {
                typeBuilder.Property(title => title.Synopsis).HasColumnType("TEXT");
                typeBuilder.Property(title => title.Status).HasColumnType("TINYINT UNSIGNED");
                typeBuilder.Property(title => title.Visible).HasColumnType("BOOLEAN");
                typeBuilder.Property<string>("TitleType").HasColumnType("VARCHAR(5)");
            });

            builder.Entity<Chapter>(typeBuilder =>
            {
                typeBuilder.Property(chapter => chapter.Volume).HasColumnType("SMALLINT UNSIGNED");
                typeBuilder.Property(chapter => chapter.Number).HasColumnType("DECIMAL(5,1)");
                typeBuilder.Property(chapter => chapter.ReleaseDate).HasColumnType("DATETIME");
                typeBuilder.Property(chapter => chapter.Visible).HasColumnType("BOOLEAN");
                typeBuilder.Property<string>("ChapterType").HasColumnType("VARCHAR(5)");
            });

            builder.Entity<Page>(typeBuilder =>
            {
                typeBuilder.Property(page => page.Animated).HasColumnType("BOOLEAN");
                typeBuilder.Property<string>("PageType").HasColumnType("VARCHAR(5)");
            });

            builder.Entity<Comic>(typeBuilder => typeBuilder.Property(comic => comic.LongStrip).HasColumnType("BOOLEAN"));

            builder.Entity<ComicPage>(typeBuilder => typeBuilder.Property(page => page.Number).HasColumnType("SMALLINT UNSIGNED"));

            builder.Entity<NovelChapterContent>(typeBuilder => typeBuilder.Property(content => content.Content).HasColumnType("MEDIUMTEXT"));
            
            builder.Entity<Post>(typeBuilder =>
            {
                typeBuilder.Property(post => post.ReleaseDate).HasColumnType("DATETIME");
                typeBuilder.Property(post => post.Visible).HasColumnType("BOOLEAN");
                typeBuilder.Property(post => post.Content).HasColumnType("TEXT");
            });
        }
    }
}