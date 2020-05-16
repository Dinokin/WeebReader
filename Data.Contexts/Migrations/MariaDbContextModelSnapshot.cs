﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WeebReader.Data.Contexts;

namespace WeebReader.Data.Contexts.Migrations
{
    [DbContext(typeof(MariaDbContext))]
    partial class MariaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = new Guid("08d79ae6-7eb5-4426-82ba-7c4a4ae9d84b"),
                            ConcurrencyStamp = "26cd3943-23ff-41f5-86ed-8b867cf233b4",
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR"
                        },
                        new
                        {
                            Id = new Guid("08d79ae6-7ec1-478f-867c-a8170f075a27"),
                            ConcurrencyStamp = "31bbe05d-7b5a-4b3a-9255-ed262a6a02c7",
                            Name = "Moderator",
                            NormalizedName = "MODERATOR"
                        },
                        new
                        {
                            Id = new Guid("08d79ae6-7ec3-42ce-8a94-00a56192c379"),
                            ConcurrencyStamp = "352e1584-d439-45dc-8015-9428b4e47c76",
                            Name = "Uploader",
                            NormalizedName = "UPLOADER"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(320) CHARACTER SET utf8mb4")
                        .HasMaxLength(320);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("BOOLEAN");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("BOOLEAN");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("DATETIME");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("varchar(320) CHARACTER SET utf8mb4")
                        .HasMaxLength(320);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("BOOLEAN");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("BOOLEAN");

                    b.Property<string>("UserName")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("ClaimValue")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(512) CHARACTER SET utf8mb4")
                        .HasMaxLength(512);

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Abstract.Chapter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ChapterType")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<decimal>("Number")
                        .HasColumnType("DECIMAL(5,1)");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("DATETIME");

                    b.Property<Guid>("TitleId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Visible")
                        .HasColumnType("BOOLEAN");

                    b.Property<ushort?>("Volume")
                        .HasColumnType("SMALLINT UNSIGNED");

                    b.HasKey("Id");

                    b.HasIndex("Number", "TitleId")
                        .IsUnique();

                    b.ToTable("Chapters");

                    b.HasDiscriminator<string>("ChapterType").HasValue("Chapter");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Abstract.Page", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("Animated")
                        .HasColumnType("BOOLEAN");

                    b.Property<Guid>("ChapterId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Pages");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Page");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Abstract.Title", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Artist")
                        .IsRequired()
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("OriginalName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PreviousChaptersLink")
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<byte>("Status")
                        .HasColumnType("TINYINT UNSIGNED");

                    b.Property<string>("Synopsis")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TitleType")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Visible")
                        .HasColumnType("BOOLEAN");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Titles");

                    b.HasDiscriminator<string>("TitleType").HasValue("Title");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Parameter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<ushort>("Type")
                        .HasColumnType("SMALLINT UNSIGNED");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Type")
                        .IsUnique();

                    b.ToTable("Parameters");

                    b.HasData(
                        new
                        {
                            Id = new Guid("b2f7adc5-9090-417c-bbc1-805071fc7a81"),
                            Type = (ushort)0,
                            Value = "WeebReader"
                        },
                        new
                        {
                            Id = new Guid("27c51234-bf33-40de-86db-1941c8622a73"),
                            Type = (ushort)1,
                            Value = "We read weebs."
                        });
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<bool>("Visible")
                        .HasColumnType("BOOLEAN");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.TitleTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TagId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TitleId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("TitleId");

                    b.HasIndex("TagId", "TitleId")
                        .IsUnique();

                    b.ToTable("TitleTags");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.ComicChapter", b =>
                {
                    b.HasBaseType("WeebReader.Data.Entities.Abstract.Chapter");

                    b.HasIndex("TitleId");

                    b.HasDiscriminator().HasValue("Comic");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.NovelChapter", b =>
                {
                    b.HasBaseType("WeebReader.Data.Entities.Abstract.Chapter");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("LONGTEXT");

                    b.HasIndex("TitleId")
                        .HasName("IX_Chapters_TitleId1");

                    b.HasDiscriminator().HasValue("Novel");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.ComicPage", b =>
                {
                    b.HasBaseType("WeebReader.Data.Entities.Abstract.Page");

                    b.Property<ushort>("Number")
                        .HasColumnType("SMALLINT UNSIGNED");

                    b.HasIndex("ChapterId");

                    b.HasIndex("Number", "ChapterId")
                        .IsUnique();

                    b.HasDiscriminator().HasValue("ComicPage");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Comic", b =>
                {
                    b.HasBaseType("WeebReader.Data.Entities.Abstract.Title");

                    b.Property<bool>("LongStrip")
                        .HasColumnType("BOOLEAN");

                    b.HasDiscriminator().HasValue("Comic");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Novel", b =>
                {
                    b.HasBaseType("WeebReader.Data.Entities.Abstract.Title");

                    b.HasDiscriminator().HasValue("Novel");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WeebReader.Data.Entities.TitleTag", b =>
                {
                    b.HasOne("WeebReader.Data.Entities.Tag", "Tag")
                        .WithMany("TitleTag")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WeebReader.Data.Entities.Abstract.Title", "Title")
                        .WithMany("TitleTags")
                        .HasForeignKey("TitleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WeebReader.Data.Entities.ComicChapter", b =>
                {
                    b.HasOne("WeebReader.Data.Entities.Comic", "Title")
                        .WithMany("Chapters")
                        .HasForeignKey("TitleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WeebReader.Data.Entities.NovelChapter", b =>
                {
                    b.HasOne("WeebReader.Data.Entities.Novel", "Title")
                        .WithMany("Chapters")
                        .HasForeignKey("TitleId")
                        .HasConstraintName("FK_Chapters_Titles_TitleId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WeebReader.Data.Entities.ComicPage", b =>
                {
                    b.HasOne("WeebReader.Data.Entities.ComicChapter", "Chapter")
                        .WithMany("Pages")
                        .HasForeignKey("ChapterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
