using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioProject.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cvs",
                keyColumn: "Id",
                keyValue: new Guid("b5a93a46-0783-45d7-8e3e-ad57a6c9d405"));

            migrationBuilder.DeleteData(
                table: "Educations",
                keyColumn: "EducationId",
                keyValue: new Guid("d4cb6f22-9aff-41aa-8649-b4ba4c926d77"));

            migrationBuilder.DeleteData(
                table: "Experiences",
                keyColumn: "ExperienceId",
                keyValue: new Guid("94f72b64-e06e-4168-8b23-6c5d81aa46eb"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("2f889ac5-74e6-4ffa-a0d8-b7873516bfc2"));

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("3618ea92-6d09-4738-8d67-51db88047668"));

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: new Guid("90b2e727-2409-4141-9545-48f7dbcac5d8"));

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: new Guid("b8d2b92c-a1fb-489f-b658-1aee8633c457"));

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: new Guid("bf765436-b7c0-4c9b-9503-9432d36440b8"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
