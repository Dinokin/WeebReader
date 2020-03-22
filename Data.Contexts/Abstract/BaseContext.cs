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
        public DbSet<Title> Titles { get; private set; } = null!;
        public DbSet<Chapter> Chapters { get; private set; } = null!;
        public DbSet<Page> Pages { get; private set; } = null!;
        public DbSet<Comic> Comics { get; private set; } = null!;
        public DbSet<ComicChapter> ComicChapters { get; private set; } = null!;
        public DbSet<ComicPage> ComicPages { get; private set; } = null!;
        public DbSet<Tag> Tags { get; private set; } = null!;
        public DbSet<TitleTag> TitleTags { get; private set; } = null!;
        public DbSet<Post> Posts { get; private set; } = null!;
        public DbSet<Parameter> Settings { get; private set; } = null!;

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

                typeBuilder.HasData(new IdentityRole<Guid>
                {
                    Id = Guid.Parse("08d79ae6-7eb5-4426-82ba-7c4a4ae9d84b"),
                    Name = "Administrator",
                    NormalizedName = "Administrator".ToUpperInvariant(),
                    ConcurrencyStamp = "26cd3943-23ff-41f5-86ed-8b867cf233b4"
                }, new IdentityRole<Guid>
                {
                    Id = Guid.Parse("08d79ae6-7ec1-478f-867c-a8170f075a27"),
                    Name = "Moderator",
                    NormalizedName = "Moderator".ToUpperInvariant(),
                    ConcurrencyStamp = "31bbe05d-7b5a-4b3a-9255-ed262a6a02c7"
                }, new IdentityRole<Guid>
                {
                    Id = Guid.Parse("08d79ae6-7ec3-42ce-8a94-00a56192c379"),
                    Name = "Uploader",
                    NormalizedName = "Uploader".ToUpperInvariant(),
                    ConcurrencyStamp = "352e1584-d439-45dc-8015-9428b4e47c76"
                });
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
                typeBuilder.HasMany(title => title.TitleTags).WithOne(titleTag => titleTag.Title!).HasForeignKey(titleTag => titleTag.TitleId);
                typeBuilder.HasDiscriminator<string>("TitleType").HasValue<Comic>("Comic");
            });
            
            builder.Entity<Chapter>(typeBuilder =>
            {
                typeBuilder.Property(chapter => chapter.Number).IsRequired();
                typeBuilder.Property(chapter => chapter.Name).HasMaxLength(100);
                typeBuilder.Property(chapter => chapter.ReleaseDate).IsRequired();
                typeBuilder.Property(chapter => chapter.Visible).IsRequired();
                typeBuilder.Property(chapter => chapter.TitleId).IsRequired();
                typeBuilder.HasIndex(chapter => new {chapter.Number, chapter.TitleId}).IsUnique();
                typeBuilder.HasDiscriminator<string>("ChapterType").HasValue<ComicChapter>("Comic");
            });
            
            builder.Entity<Page>(typeBuilder =>
            {
                typeBuilder.Property(page => page.Animated).IsRequired();
                typeBuilder.Property(page => page.ChapterId).IsRequired();
            });

            builder.Entity<Comic>(typeBuilder =>
            {
                typeBuilder.Property(comic => comic.LongStrip);

                typeBuilder.HasMany(comic => comic.Chapters).WithOne(comic => comic.Title!).HasForeignKey(chapter => chapter.TitleId);
            });
            
            builder.Entity<ComicChapter>(typeBuilder => typeBuilder.HasMany(chapter => chapter.Pages).WithOne(page => page.Chapter!).HasForeignKey(page => page.ChapterId));
            
            builder.Entity<ComicPage>(typeBuilder =>
            {
                typeBuilder.Property(page => page.Number).IsRequired();
                typeBuilder.HasIndex(page => new {page.Number, page.ChapterId}).IsUnique();
            });

            builder.Entity<Tag>(typeBuilder =>
            {
                typeBuilder.Property(tag => tag.Name).IsRequired().HasMaxLength(20);
                typeBuilder.HasIndex(tag => tag.Name).IsUnique();
                typeBuilder.HasMany(tag => tag.TitleTag).WithOne(titleTag => titleTag.Tag!).HasForeignKey(titleTag => titleTag.TagId);
            });

            builder.Entity<TitleTag>(typeBuilder => typeBuilder.HasIndex(titleTag => new {titleTag.TagId, titleTag.TitleId}).IsUnique());

            builder.Entity<Post>(typeBuilder =>
            {
                typeBuilder.Property(post => post.Title).IsRequired().HasMaxLength(100);
                typeBuilder.HasIndex(post => post.Title).IsUnique();
                typeBuilder.Property(post => post.Content).IsRequired();
                typeBuilder.Property(post => post.ReleaseDate).IsRequired();
                typeBuilder.Property(post => post.Visible).IsRequired();
            });
            
            builder.Entity<Parameter>(typeBuilder =>
            {
                typeBuilder.Property(setting => setting.Type).IsRequired();
                typeBuilder.HasIndex(setting => setting.Type).IsUnique();

                typeBuilder.HasData(
                    new Parameter(Guid.NewGuid(), Parameter.Types.SiteName, "WeebReader"), 
                    new Parameter(Guid.NewGuid(), Parameter.Types.SiteDescription, "We read weebs."),
                    new Parameter(Guid.NewGuid(), Parameter.Types.SiteAddress, "http://127.0.0.1:5000"));
            });
        }
    }
}