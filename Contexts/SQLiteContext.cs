using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Contexts
{
    public class SQLiteContext : BaseContext
    {
        public SQLiteContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Announcement>(typeBuilder =>
            {
                typeBuilder.Property(announcement => announcement.CreationDate).HasColumnType("DATETIME");
                typeBuilder.Property(announcement => announcement.ReleaseDate).HasColumnType("DATETIME");
            });

            builder.Entity<Chapter>(typeBuilder =>
            {
                typeBuilder.Property(chapter => chapter.Volume).HasColumnType("UNSIGNED SMALLINT");
                typeBuilder.Property(chapter => chapter.Number).HasColumnType("DECIMAL");
                typeBuilder.Property(chapter => chapter.CreationDate).HasColumnType("DATETIME");
                typeBuilder.Property(chapter => chapter.ReleaseDate).HasColumnType("DATETIME");
            });

            builder.Entity<Link>(typeBuilder => typeBuilder.Property(link => link.Active).HasColumnType("BOOLEAN"));

            builder.Entity<Page>(typeBuilder => typeBuilder.Property(page => page.Animated).HasColumnType("BOOLEAN"));

            builder.Entity<ComicPage>(typeBuilder => typeBuilder.Property(page => page.Number).HasColumnType("UNSIGNED SMALLINT"));
            
            builder.Entity<Title>(typeBuilder =>
            {
                typeBuilder.Property(title => title.Status).HasColumnType("UNSIGNED TINYINT");
                typeBuilder.Property(title => title.Visible).HasColumnType("BOOLEAN");
            });

            builder.Entity<Comic>(typeBuilder => typeBuilder.Property(comic => comic.LongStrip).HasColumnType("BOOLEAN"));

            builder.Entity<IdentityUser<Guid>>(typeBuilder =>
            {
                typeBuilder.Property(user => user.EmailConfirmed).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.PhoneNumberConfirmed).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.TwoFactorEnabled).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.LockoutEnabled).HasColumnType("BOOLEAN");
                typeBuilder.Property(user => user.LockoutEnd).HasColumnType("DATETIME");
            });
        }
    }
}