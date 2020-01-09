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
        public DbSet<Post> Posts { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected BaseContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser<Guid>>(typeBuilder =>
            {
                typeBuilder.Property(user => user.UserName).HasMaxLength(25);
                typeBuilder.Property(user => user.NormalizedUserName).HasMaxLength(25);
                typeBuilder.Property(user => user.Email).HasMaxLength(320);
                typeBuilder.Property(user => user.NormalizedEmail).HasMaxLength(320);
                typeBuilder.ToTable("Users");
            });
            
            builder.Entity<IdentityRole<Guid>>(typeBuilder =>
            {
                typeBuilder.Property(role => role.Name).HasMaxLength(25);
                typeBuilder.Property(role => role.NormalizedName).HasMaxLength(25);
                typeBuilder.ToTable("Roles");
            });

            builder.Entity<IdentityUserClaim<Guid>>(typeBuilder =>
            {
                typeBuilder.Property(role => role.ClaimType).HasMaxLength(25);
                typeBuilder.Property(role => role.ClaimValue).HasMaxLength(25);
                typeBuilder.ToTable("UserClaims");
            });
            
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");

            builder.Entity<IdentityUserLogin<Guid>>(typeBuilder =>
            {
                typeBuilder.Property(userLogin => userLogin.LoginProvider).HasMaxLength(50);
                typeBuilder.Property(userLogin => userLogin.ProviderKey).HasMaxLength(512);
                typeBuilder.ToTable("UserLogins");
            });
            
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

            builder.Entity<IdentityUserToken<Guid>>(typeBuilder =>
            {
                typeBuilder.Property(userToken => userToken.LoginProvider).HasMaxLength(50);
                typeBuilder.Property(userToken => userToken.Name).HasMaxLength(50);
                typeBuilder.ToTable("UserTokens");
            });

            builder.Entity<Title>(typeBuilder =>
            {
                typeBuilder.Property(title => title.Name).IsRequired().HasMaxLength(200);
                typeBuilder.Property(title => title.Author).IsRequired().HasMaxLength(50);
                typeBuilder.Property(title => title.Artist).IsRequired().HasMaxLength(50);
                typeBuilder.Property(title => title.Synopsis).IsRequired();
                typeBuilder.Property(title => title.Status).IsRequired();
                typeBuilder.Property(title => title.Visible).IsRequired();
                typeBuilder.HasMany(title => title.TitleTags).WithOne(titleTag => titleTag.Title).HasForeignKey(titleTag => titleTag.TitleId);
                typeBuilder.HasDiscriminator(title => title.Type).HasValue<Comic>(0).HasValue<Novel>(1);
            });
            
            builder.Entity<Chapter>(typeBuilder =>
            {
                typeBuilder.Property(chapter => chapter.Number).IsRequired();
                typeBuilder.Property(chapter => chapter.Name).HasMaxLength(100);
                typeBuilder.Property(chapter => chapter.Date).IsRequired();
                typeBuilder.Property(chapter => chapter.TitleId).IsRequired();
                typeBuilder.HasIndex(chapter => new {chapter.Number, chapter.TitleId}).IsUnique();
                typeBuilder.HasDiscriminator(chapter => chapter.Type).HasValue<ComicChapter>(0).HasValue<NovelChapter>(1);
            });
            
            builder.Entity<Page>(typeBuilder =>
            {
                typeBuilder.Property(page => page.Animated).IsRequired();
                typeBuilder.Property(page => page.ChapterId).IsRequired();
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
                typeBuilder.Property(tag => tag.Name).IsRequired().HasMaxLength(20);
                typeBuilder.HasIndex(tag => tag.Name).IsUnique();
                typeBuilder.HasMany(tag => tag.TitleTag).WithOne(titleTag => titleTag.Tag).HasForeignKey(titleTag => titleTag.TagId);
            });

            builder.Entity<TitleTag>(typeBuilder => typeBuilder.HasIndex(titleTag => new {titleTag.TagId, titleTag.TitleId}).IsUnique());

            builder.Entity<Post>(typeBuilder =>
            {
                typeBuilder.Property(announcement => announcement.Name).IsRequired().HasMaxLength(100);
                typeBuilder.Property(announcement => announcement.Content).IsRequired();
                typeBuilder.Property(announcement => announcement.Date).IsRequired();
            });
            
            builder.Entity<Resource>(typeBuilder => typeBuilder.Property(package => package.Name).IsRequired().HasMaxLength(100));
            
            builder.Entity<Link>(typeBuilder =>
            {
                typeBuilder.Property(link => link.Name).IsRequired().HasMaxLength(20);
                typeBuilder.HasIndex(link => link.Name).IsUnique();
                typeBuilder.Property(link => link.Destination).IsRequired();
                typeBuilder.Property(link => link.Active).IsRequired();
            });
            
            builder.Entity<Setting>(typeBuilder =>
            {
                typeBuilder.Property(setting => setting.Key).IsRequired().HasMaxLength(50);
                typeBuilder.HasIndex(setting => setting.Key).IsUnique();
            });
        }
    }
}