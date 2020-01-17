﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WeebReader.Data.Contexts;

namespace WeebReader.Data.Contexts.Migrations
{
    [DbContext(typeof(MariaDbContext))]
    [Migration("20200117004257_AddSettings_1")]
    partial class AddSettings_1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
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
                        .HasColumnType("varchar(25) CHARACTER SET utf8mb4")
                        .HasMaxLength(25);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("varchar(25) CHARACTER SET utf8mb4")
                        .HasMaxLength(25);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");
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
                        .HasColumnType("varchar(25) CHARACTER SET utf8mb4")
                        .HasMaxLength(25);

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
                        .HasColumnType("varchar(25) CHARACTER SET utf8mb4")
                        .HasMaxLength(25);

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
                        .HasColumnType("varchar(25) CHARACTER SET utf8mb4")
                        .HasMaxLength(25);

                    b.Property<string>("ClaimValue")
                        .HasColumnType("varchar(25) CHARACTER SET utf8mb4")
                        .HasMaxLength(25);

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

                    b.Property<DateTime>("Date")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<decimal>("Number")
                        .HasColumnType("DECIMAL");

                    b.Property<Guid>("TitleId")
                        .HasColumnType("char(36)");

                    b.Property<byte>("Type")
                        .HasColumnType("TINYINT UNSIGNED");

                    b.Property<bool>("Visible")
                        .HasColumnType("BOOLEAN");

                    b.Property<ushort>("Volume")
                        .HasColumnType("SMALLINT UNSIGNED");

                    b.HasKey("Id");

                    b.HasIndex("Number", "TitleId")
                        .IsUnique();

                    b.ToTable("Chapters");

                    b.HasDiscriminator<byte>("Type");
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

                    b.Property<byte>("Status")
                        .HasColumnType("TINYINT UNSIGNED");

                    b.Property<string>("Synopsis")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte>("Type")
                        .HasColumnType("TINYINT UNSIGNED");

                    b.Property<bool>("Visible")
                        .HasColumnType("BOOLEAN");

                    b.HasKey("Id");

                    b.ToTable("Titles");

                    b.HasDiscriminator<byte>("Type");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Link", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("Active")
                        .HasColumnType("BOOLEAN");

                    b.Property<string>("Destination")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(20) CHARACTER SET utf8mb4")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Links");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("Date")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Resource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Setting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("Settings");

                    b.HasData(
                        new
                        {
                            Id = new Guid("040569bc-3251-47d1-b51a-1a728c3d49ec"),
                            Key = "SiteName",
                            Value = "WeebReader"
                        },
                        new
                        {
                            Id = new Guid("a49f13c1-bd9a-41ac-90a4-4d9051b0cdec"),
                            Key = "SiteDescription",
                            Value = "We read weebs."
                        },
                        new
                        {
                            Id = new Guid("94010814-1ba1-4fca-8e57-a879ef51ba1a"),
                            Key = "SiteAddress",
                            Value = "http://127.0.0.1:5000"
                        },
                        new
                        {
                            Id = new Guid("1c629bb9-e897-4e6c-9017-300aef64d077"),
                            Key = "SiteEmail",
                            Value = ""
                        },
                        new
                        {
                            Id = new Guid("af86f645-4209-4eeb-aec0-86ba7e2dc2f6"),
                            Key = "EmailEnabled",
                            Value = "False"
                        },
                        new
                        {
                            Id = new Guid("bd69a35d-2eec-49a5-a05f-cd2d6089c323"),
                            Key = "SmtpServer",
                            Value = ""
                        },
                        new
                        {
                            Id = new Guid("4fa188c4-1892-40cb-bf63-8606df9fcbc8"),
                            Key = "SmtpServerPort",
                            Value = "0"
                        },
                        new
                        {
                            Id = new Guid("0e51d7f2-d71a-4924-a5c9-363b9197b5b2"),
                            Key = "SmtpServerUser",
                            Value = ""
                        },
                        new
                        {
                            Id = new Guid("ff4d59aa-3a8a-428f-8840-b75e5aba23b4"),
                            Key = "SmtpServerPassword",
                            Value = ""
                        });
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(20) CHARACTER SET utf8mb4")
                        .HasMaxLength(20);

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

                    b.HasDiscriminator().HasValue((byte)0);
                });

            modelBuilder.Entity("WeebReader.Data.Entities.NovelChapter", b =>
                {
                    b.HasBaseType("WeebReader.Data.Entities.Abstract.Chapter");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasIndex("TitleId")
                        .HasName("IX_Chapters_TitleId1");

                    b.HasDiscriminator().HasValue((byte)1);
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

                    b.HasDiscriminator().HasValue((byte)0);
                });

            modelBuilder.Entity("WeebReader.Data.Entities.Novel", b =>
                {
                    b.HasBaseType("WeebReader.Data.Entities.Abstract.Title");

                    b.HasDiscriminator().HasValue((byte)1);
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
