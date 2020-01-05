using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Contexts.Abstract
{
    public abstract class BaseContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public DbSet<Comic> Comics { get; set; }
        public DbSet<ComicChapter> ComicChapters { get; set; }
        public DbSet<ComicPage> ComicPages { get; set; }
        public DbSet<Novel> Novels { get; set; }
        public DbSet<NovelChapter> NovelChapters { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TitleTag> TitleTags { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected BaseContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser<Guid>>().ToTable("Users");
            builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

            builder.Entity<Title>(typeBuilder =>
            {
                typeBuilder.Property(title => title.Name).IsRequired();
                typeBuilder.Property(title => title.Author).IsRequired();
                typeBuilder.Property(title => title.Artist).IsRequired();
                typeBuilder.Property(title => title.Synopsis).IsRequired();
                typeBuilder.Property(title => title.Cover).IsRequired();
                typeBuilder.Property(title => title.Status).IsRequired();
                typeBuilder.Property(title => title.Visible).IsRequired();
                typeBuilder.HasIndex(title => new {title.Name, title.Type}).IsUnique();
                typeBuilder.HasMany(title => title.TitleTags).WithOne(titleTag => titleTag.Title).HasForeignKey(titleTag => titleTag.TitleId);
                typeBuilder.HasDiscriminator(title => title.Type).HasValue<Comic>(typeof(Comic).Name).HasValue<Novel>(typeof(Novel).Name);
            });
            
            builder.Entity<Chapter>(typeBuilder =>
            {
                typeBuilder.Property(chapter => chapter.Number).IsRequired();
                typeBuilder.Property(chapter => chapter.CreationDate).IsRequired();
                typeBuilder.Property(chapter => chapter.TitleId).IsRequired();
                typeBuilder.HasIndex(chapter => new {chapter.Number, chapter.TitleId}).IsUnique();
                typeBuilder.HasOne(chapter => chapter.Package).WithOne(package => package.Chapter).HasForeignKey<Package>(package => package.ChapterId);
                typeBuilder.HasDiscriminator<string>("Type").HasValue<ComicChapter>(typeof(ComicChapter).Name).HasValue<NovelChapter>(typeof(NovelChapter).Name);
            });
            
            builder.Entity<Page>(typeBuilder =>
            {
                typeBuilder.Property(page => page.Animated).IsRequired();
                typeBuilder.Property(page => page.ChapterId).IsRequired();
                typeBuilder.Property(page => page.Content).IsRequired();
            });

            builder.Entity<Comic>(typeBuilder =>
            {
                typeBuilder.Property(comic => comic.LongStrip);

                typeBuilder.HasMany(comic => comic.Chapters).WithOne(comic => comic.Title).HasForeignKey(chapter => chapter.TitleId);
            });
            
            builder.Entity<ComicChapter>(typeBuilder => typeBuilder.HasMany(chapter => chapter.Pages).WithOne(page => page.Chapter).HasForeignKey(page => page.ChapterId));
            
            builder.Entity<ComicPage>(typeBuilder =>
            {
                typeBuilder.Property(page => page.Number).IsRequired();
                typeBuilder.HasIndex(page => new {page.Number, page.ChapterId}).IsUnique();
            });

            builder.Entity<Novel>(typeBuilder => typeBuilder.HasMany(novel => novel.Chapters).WithOne(chapter => chapter.Title).HasForeignKey(chapter => chapter.TitleId));
            
            builder.Entity<NovelChapter>(typeBuilder => typeBuilder.Property(chapter => chapter.Content).IsRequired());

            builder.Entity<Tag>(typeBuilder =>
            {
                typeBuilder.Property(tag => tag.Name).IsRequired();
                typeBuilder.HasIndex(tag => tag.Name).IsUnique();
                typeBuilder.HasMany(tag => tag.TitleTag).WithOne(titleTag => titleTag.Tag).HasForeignKey(titleTag => titleTag.TagId);
            });

            builder.Entity<TitleTag>(typeBuilder => typeBuilder.HasIndex(titleTag => new {titleTag.TagId, titleTag.TitleId}).IsUnique());

            builder.Entity<Announcement>(typeBuilder =>
            {
                typeBuilder.Property(announcement => announcement.Name).IsRequired();
                typeBuilder.Property(announcement => announcement.Content).IsRequired();
                typeBuilder.Property(announcement => announcement.CreationDate).IsRequired();
            });
            
            builder.Entity<Resource>(typeBuilder =>
            {
                typeBuilder.Property(package => package.Name).IsRequired();
                typeBuilder.Property(package => package.Content).IsRequired();
                typeBuilder.HasDiscriminator<string>("Type").HasValue<Resource>(typeof(Resource).Name).HasValue<Package>(typeof(Package).Name);
            });

            builder.Entity<Package>(typeBuilder =>
            {
                typeBuilder.Property(package => package.ChapterId).IsRequired();
                typeBuilder.HasIndex(package => package.ChapterId).IsUnique();
            });
            
            builder.Entity<Link>(typeBuilder =>
            {
                typeBuilder.Property(link => link.Name).IsRequired();
                typeBuilder.HasIndex(link => link.Name).IsUnique();
                typeBuilder.Property(link => link.Destination).IsRequired();
                typeBuilder.Property(link => link.Active).IsRequired();
            });
            
            builder.Entity<Setting>(typeBuilder =>
            {
                typeBuilder.Property(setting => setting.Key).IsRequired();
                typeBuilder.HasIndex(setting => setting.Key).IsUnique();
                typeBuilder.Property(setting => setting.Key).IsRequired();
            });
        }
    }
}