using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Contexts
{
    public class MariaDbContext : BaseContext
    {
        public MariaDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser<Guid>>(typeBuilder =>
            {
                typeBuilder.Property(user => user.EmailConfirmed).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.PasswordHash).HasColumnType("TEXT");
                typeBuilder.Property(user => user.SecurityStamp).HasColumnType("TEXT");
                typeBuilder.Property(user => user.ConcurrencyStamp).HasColumnType("TEXT");
                typeBuilder.Property(user => user.PhoneNumber).HasColumnType("TEXT");
                typeBuilder.Property(user => user.PhoneNumberConfirmed).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.TwoFactorEnabled).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.LockoutEnd).HasColumnType("DATETIME");
                typeBuilder.Property(user => user.LockoutEnabled).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.AccessFailedCount).HasColumnType("INT");
            });

            builder.Entity<IdentityRole<Guid>>(typeBuilder => typeBuilder.Property(role => role.ConcurrencyStamp).HasColumnType("TEXT"));

            builder.Entity<IdentityRoleClaim<Guid>>(typeBuilder =>
            {
                typeBuilder.Property(roleClaim => roleClaim.ClaimType).HasColumnType("TEXT");
                typeBuilder.Property(roleClaim => roleClaim.ClaimValue).HasColumnType("TEXT");
            });

            builder.Entity<IdentityUserToken<Guid>>(typeBuilder =>
            {
                typeBuilder.Property(userToken => userToken.Value).HasColumnType("TEXT");
            });

            builder.Entity<Title>(typeBuilder =>
            {
                typeBuilder.Property(title => title.Synopsis).HasColumnType("TEXT");
                typeBuilder.Property(title => title.Status).HasColumnType("TINYINT UNSIGNED");
                typeBuilder.Property(title => title.Type).HasColumnType("TINYINT UNSIGNED");
                typeBuilder.Property(title => title.Visible).HasColumnType("BOOLEAN");
            });

            builder.Entity<Chapter>(typeBuilder =>
            {
                typeBuilder.Property(chapter => chapter.Volume).HasColumnType("SMALLINT UNSIGNED");
                typeBuilder.Property(chapter => chapter.Number).HasColumnType("DECIMAL");
                typeBuilder.Property(chapter => chapter.Date).HasColumnType("DATETIME");
                typeBuilder.Property(chapter => chapter.Visible).HasColumnType("BOOLEAN");
                typeBuilder.Property(chapter => chapter.Type).HasColumnType("TINYINT UNSIGNED");
            });

            builder.Entity<Page>(typeBuilder => typeBuilder.Property(page => page.Animated).HasColumnType("BOOLEAN"));

            builder.Entity<Comic>(typeBuilder => typeBuilder.Property(comic => comic.LongStrip).HasColumnType("BOOLEAN"));

            builder.Entity<ComicPage>(typeBuilder => typeBuilder.Property(page => page.Number).HasColumnType("SMALLINT UNSIGNED"));
            
            builder.Entity<Post>(typeBuilder =>
            {
                typeBuilder.Property(post => post.Date).HasColumnType("DATETIME");
                typeBuilder.Property(post => post.Visible).HasColumnType("BOOLEAN");
            });

            builder.Entity<Resource>(typeBuilder => typeBuilder.Property(resource => resource.Visible).HasColumnType("BOOLEAN"));

            builder.Entity<Link>(typeBuilder =>
            {
                typeBuilder.Property(link => link.Destination).HasColumnType("TEXT");
                typeBuilder.Property(link => link.Active).HasColumnType("BOOLEAN");
            });
            
            builder.Entity<Setting>(typeBuilder =>
            {
                typeBuilder.Property(setting => setting.Key).HasColumnType("SMALLINT UNSIGNED");
                typeBuilder.Property(setting => setting.Value).HasColumnType("TEXT");
            });
        }
    }
}