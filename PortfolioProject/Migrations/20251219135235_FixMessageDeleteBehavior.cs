using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioProject.Migrations
{
    public partial class FixMessageDeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "5211872b-c0d1-4904-94ef-b7ee174a9d32");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "5acbd350-28b7-4cb6-94bf-42dd86c53af2");

            migrationBuilder.RenameColumn(
                name: "isPrivate",
                table: "Users",
                newName: "IsPrivate");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.CreateTable(
                name: "Cvs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cvs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cvs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    EducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    School = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartYear = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndYear = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.EducationId);
                });

            migrationBuilder.CreateTable(
                name: "Experiences",
                columns: table => new
                {
                    ExperienceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartYear = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndYear = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiences", x => x.ExperienceId);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Messages_Users_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Messages_Users_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CvEducation",
                columns: table => new
                {
                    CvsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationsEducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvEducation", x => new { x.CvsId, x.EducationsEducationId });
                    table.ForeignKey(
                        name: "FK_CvEducation_Cvs_CvsId",
                        column: x => x.CvsId,
                        principalTable: "Cvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CvEducation_Educations_EducationsEducationId",
                        column: x => x.EducationsEducationId,
                        principalTable: "Educations",
                        principalColumn: "EducationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CvExperience",
                columns: table => new
                {
                    CvsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExperiencesExperienceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvExperience", x => new { x.CvsId, x.ExperiencesExperienceId });
                    table.ForeignKey(
                        name: "FK_CvExperience_Cvs_CvsId",
                        column: x => x.CvsId,
                        principalTable: "Cvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CvExperience_Experiences_ExperiencesExperienceId",
                        column: x => x.ExperiencesExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "ExperienceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUser",
                columns: table => new
                {
                    ProjectsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUser", x => new { x.ProjectsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ProjectUser_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CvSkill",
                columns: table => new
                {
                    CvsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvSkill", x => new { x.CvsId, x.SkillsId });
                    table.ForeignKey(
                        name: "FK_CvSkill_Cvs_CvsId",
                        column: x => x.CvsId,
                        principalTable: "Cvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CvSkill_Skills_SkillsId",
                        column: x => x.SkillsId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cvs",
                columns: new[] { "Id", "UserId" },
                values: new object[] { new Guid("b5a93a46-0783-45d7-8e3e-ad57a6c9d405"), "USER_ID_1" });

            migrationBuilder.InsertData(
                table: "Educations",
                columns: new[] { "EducationId", "Degree", "EndYear", "School", "StartYear" },
                values: new object[] { new Guid("d4cb6f22-9aff-41aa-8649-b4ba4c926d77"), "Systemvetenskap", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Örebro University", new DateTime(2022, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Experiences",
                columns: new[] { "ExperienceId", "Company", "EndYear", "Role", "StartYear" },
                values: new object[] { new Guid("94f72b64-e06e-4168-8b23-6c5d81aa46eb"), "Tech AB", new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Junior Developer", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Body", "FromUserId", "IsRead", "SentAt", "ToUserId" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "Welcome to the platform!", "USER_ID_1", false, new DateTime(2025, 12, 19, 13, 52, 35, 226, DateTimeKind.Utc).AddTicks(9927), "USER_ID_2" });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Description", "Title" },
                values: new object[,]
                {
                    { new Guid("2f889ac5-74e6-4ffa-a0d8-b7873516bfc2"), "MVC + EF Core", "Portfolio Website" },
                    { new Guid("3618ea92-6d09-4738-8d67-51db88047668"), "User-to-user chat", "Messaging System" }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("90b2e727-2409-4141-9545-48f7dbcac5d8"), "ASP.NET Core" },
                    { new Guid("b8d2b92c-a1fb-489f-b658-1aee8633c457"), "SQL" },
                    { new Guid("bf765436-b7c0-4c9b-9503-9432d36440b8"), "C#" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CvEducation_EducationsEducationId",
                table: "CvEducation",
                column: "EducationsEducationId");

            migrationBuilder.CreateIndex(
                name: "IX_CvExperience_ExperiencesExperienceId",
                table: "CvExperience",
                column: "ExperiencesExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_Cvs_UserId",
                table: "Cvs",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CvSkill_SkillsId",
                table: "CvSkill",
                column: "SkillsId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_FromUserId",
                table: "Messages",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ToUserId",
                table: "Messages",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_UsersId",
                table: "ProjectUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CvEducation");

            migrationBuilder.DropTable(
                name: "CvExperience");

            migrationBuilder.DropTable(
                name: "CvSkill");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "ProjectUser");

            migrationBuilder.DropTable(
                name: "Educations");

            migrationBuilder.DropTable(
                name: "Experiences");

            migrationBuilder.DropTable(
                name: "Cvs");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.RenameColumn(
                name: "IsPrivate",
                table: "Users",
                newName: "isPrivate");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "isActive");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "isActive", "isPrivate" },
                values: new object[] { "5211872b-c0d1-4904-94ef-b7ee174a9d32", 0, "dc8f85f8-34df-4178-8055-5581630355dd", "user@portfolio.se", false, "Anna", "Andersson", false, null, null, null, "password", null, false, "ddabac5f-c75e-449f-8948-1c57209d05f2", false, "user", true, false });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "isActive", "isPrivate" },
                values: new object[] { "5acbd350-28b7-4cb6-94bf-42dd86c53af2", 0, "317d5d5c-3d85-4712-9825-60a740511be6", "admin@portfolio.se", false, "Leo", "Wallenstål", false, null, null, null, "password", null, false, "5b489bf8-5157-4ae6-8801-119ca922b56d", false, "admin", true, false });
        }
    }
}
