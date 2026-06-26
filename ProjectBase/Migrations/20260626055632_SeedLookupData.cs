using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectBase.Migrations
{
    /// <inheritdoc />
    public partial class SeedLookupData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PracticeLevel",
                columns: new[] { "ID", "Description", "title" },
                values: new object[,]
                {
                    { 1, "Easy", "Easy" },
                    { 2, "Medium", "Medium" },
                    { 3, "Hard", "Hard" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleID", "RoleName" },
                values: new object[,]
                {
                    { 1L, "Admin" },
                    { 2L, "Customer" },
                    { 3L, "Marketing" },
                    { 4L, "Sale" },
                    { 5L, "Expert" },
                    { 6L, "Guest" }
                });

            migrationBuilder.InsertData(
                table: "SubjectTopic",
                columns: new[] { "id", "subjectId", "title" },
                values: new object[] { 1, 0L, "General" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PracticeLevel",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PracticeLevel",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PracticeLevel",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleID",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleID",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleID",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleID",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleID",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleID",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "SubjectTopic",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
