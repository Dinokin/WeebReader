using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class intial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Parameters",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Type = table.Column<ushort>("SMALLINT UNSIGNED"),
                    Value = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Posts",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Title = table.Column<string>(maxLength: 100),
                    Content = table.Column<string>(),
                    ReleaseDate = table.Column<DateTime>("DATETIME"),
                    Visible = table.Column<bool>("BOOLEAN")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Roles",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Name = table.Column<string>(maxLength: 25, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 25, nullable: true),
                    ConcurrencyStamp = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Tags",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Name = table.Column<string>(maxLength: 20)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Titles",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Name = table.Column<string>(maxLength: 200),
                    OriginalName = table.Column<string>(nullable: true),
                    Author = table.Column<string>(maxLength: 50),
                    Artist = table.Column<string>(maxLength: 50),
                    Synopsis = table.Column<string>("TEXT"),
                    Status = table.Column<byte>("TINYINT UNSIGNED"),
                    Visible = table.Column<bool>("BOOLEAN"),
                    TitleType = table.Column<string>(),
                    LongStrip = table.Column<bool>("BOOLEAN", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    Id = table.Column<Guid>(),
                    UserName = table.Column<string>(maxLength: 25, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 25, nullable: true),
                    Email = table.Column<string>(maxLength: 320, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 320, nullable: true),
                    EmailConfirmed = table.Column<bool>("BOOLEAN"),
                    PasswordHash = table.Column<string>("TEXT", nullable: true),
                    SecurityStamp = table.Column<string>("TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>("TEXT", nullable: true),
                    PhoneNumber = table.Column<string>("TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>("BOOLEAN"),
                    TwoFactorEnabled = table.Column<bool>("BOOLEAN"),
                    LockoutEnd = table.Column<DateTimeOffset>("DATETIME", nullable: true),
                    LockoutEnabled = table.Column<bool>("BOOLEAN"),
                    AccessFailedCount = table.Column<int>("INT")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "RoleClaims",
                table => new
                {
                    Id = table.Column<int>()
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<Guid>(),
                    ClaimType = table.Column<string>("TEXT", nullable: true),
                    ClaimValue = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_RoleClaims_Roles_RoleId",
                        x => x.RoleId,
                        "Roles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Chapters",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Volume = table.Column<ushort>("SMALLINT UNSIGNED", nullable: true),
                    Number = table.Column<decimal>("DECIMAL"),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    ReleaseDate = table.Column<DateTime>("DATETIME"),
                    Visible = table.Column<bool>("BOOLEAN"),
                    TitleId = table.Column<Guid>(),
                    ChapterType = table.Column<string>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                    table.ForeignKey(
                        "FK_Chapters_Titles_TitleId",
                        x => x.TitleId,
                        "Titles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "TitleTags",
                table => new
                {
                    Id = table.Column<Guid>(),
                    TitleId = table.Column<Guid>(),
                    TagId = table.Column<Guid>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleTags", x => x.Id);
                    table.ForeignKey(
                        "FK_TitleTags_Tags_TagId",
                        x => x.TagId,
                        "Tags",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_TitleTags_Titles_TitleId",
                        x => x.TitleId,
                        "Titles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UserClaims",
                table => new
                {
                    Id = table.Column<int>()
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(),
                    ClaimType = table.Column<string>(maxLength: 25, nullable: true),
                    ClaimValue = table.Column<string>(maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_UserClaims_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UserLogins",
                table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 50),
                    ProviderKey = table.Column<string>(maxLength: 512),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        "FK_UserLogins_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UserRoles",
                table => new
                {
                    UserId = table.Column<Guid>(),
                    RoleId = table.Column<Guid>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        "FK_UserRoles_Roles_RoleId",
                        x => x.RoleId,
                        "Roles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_UserRoles_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UserTokens",
                table => new
                {
                    UserId = table.Column<Guid>(),
                    LoginProvider = table.Column<string>(maxLength: 50),
                    Name = table.Column<string>(maxLength: 50),
                    Value = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        "FK_UserTokens_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Pages",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Animated = table.Column<bool>("BOOLEAN"),
                    ChapterId = table.Column<Guid>(),
                    Discriminator = table.Column<string>(),
                    Number = table.Column<ushort>("SMALLINT UNSIGNED", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        "FK_Pages_Chapters_ChapterId",
                        x => x.ChapterId,
                        "Chapters",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                "Parameters",
                new[] { "Id", "Type", "Value" },
                new object[,]
                {
                    { new Guid("bc024453-6ebb-4da1-a206-17ea4b252a55"), (ushort)0, "WeebReader" },
                    { new Guid("3d690373-9ea5-48bb-94c1-050f3825b833"), (ushort)1, "We read weebs." },
                    { new Guid("1d105f2e-0b32-42dc-9fb5-d74cc1a81b12"), (ushort)2, "http://127.0.0.1:5000" }
                });

            migrationBuilder.InsertData(
                "Roles",
                new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                new object[,]
                {
                    { new Guid("08d79ae6-7eb5-4426-82ba-7c4a4ae9d84b"), "26cd3943-23ff-41f5-86ed-8b867cf233b4", "Administrator", "ADMINISTRATOR" },
                    { new Guid("08d79ae6-7ec1-478f-867c-a8170f075a27"), "31bbe05d-7b5a-4b3a-9255-ed262a6a02c7", "Moderator", "MODERATOR" },
                    { new Guid("08d79ae6-7ec3-42ce-8a94-00a56192c379"), "352e1584-d439-45dc-8015-9428b4e47c76", "Uploader", "UPLOADER" }
                });

            migrationBuilder.CreateIndex(
                "IX_Chapters_Number_TitleId",
                "Chapters",
                new[] { "Number", "TitleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Chapters_TitleId",
                "Chapters",
                "TitleId");

            migrationBuilder.CreateIndex(
                "IX_Pages_ChapterId",
                "Pages",
                "ChapterId");

            migrationBuilder.CreateIndex(
                "IX_Pages_Number_ChapterId",
                "Pages",
                new[] { "Number", "ChapterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Parameters_Type",
                "Parameters",
                "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Posts_Title",
                "Posts",
                "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_RoleClaims_RoleId",
                "RoleClaims",
                "RoleId");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "Roles",
                "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Tags_Name",
                "Tags",
                "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Titles_Name",
                "Titles",
                "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_TitleTags_TitleId",
                "TitleTags",
                "TitleId");

            migrationBuilder.CreateIndex(
                "IX_TitleTags_TagId_TitleId",
                "TitleTags",
                new[] { "TagId", "TitleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_UserClaims_UserId",
                "UserClaims",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_UserLogins_UserId",
                "UserLogins",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_UserRoles_RoleId",
                "UserRoles",
                "RoleId");

            migrationBuilder.CreateIndex(
                "EmailIndex",
                "Users",
                "NormalizedEmail");

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                "Users",
                "NormalizedUserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Pages");

            migrationBuilder.DropTable(
                "Parameters");

            migrationBuilder.DropTable(
                "Posts");

            migrationBuilder.DropTable(
                "RoleClaims");

            migrationBuilder.DropTable(
                "TitleTags");

            migrationBuilder.DropTable(
                "UserClaims");

            migrationBuilder.DropTable(
                "UserLogins");

            migrationBuilder.DropTable(
                "UserRoles");

            migrationBuilder.DropTable(
                "UserTokens");

            migrationBuilder.DropTable(
                "Chapters");

            migrationBuilder.DropTable(
                "Tags");

            migrationBuilder.DropTable(
                "Roles");

            migrationBuilder.DropTable(
                "Users");

            migrationBuilder.DropTable(
                "Titles");
        }
    }
}
