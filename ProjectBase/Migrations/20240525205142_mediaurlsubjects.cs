using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBase.Migrations
{
    /// <inheritdoc />
    public partial class mediaurlsubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "media_url",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "media_url",
                table: "Subjects");
        }
    }
}
