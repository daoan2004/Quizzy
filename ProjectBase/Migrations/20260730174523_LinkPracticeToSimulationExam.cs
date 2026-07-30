using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBase.Migrations
{
    /// <inheritdoc />
    public partial class LinkPracticeToSimulationExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SimulationExamID",
                table: "Practice",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [SimulationExam]
                SET [Number_Question] = 4
                WHERE [Number_Question] > 4;

                UPDATE practice
                SET [number_quest] = handles.[HandleCount]
                FROM [Practice] AS practice
                CROSS APPLY
                (
                    SELECT COUNT(*) AS [HandleCount]
                    FROM [QuizHandle] AS handle
                    WHERE handle.[PracticeID] = practice.[ID]
                ) AS handles
                WHERE practice.[Status] = 0
                  AND handles.[HandleCount] > 0
                  AND practice.[number_quest] <> handles.[HandleCount];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Practice_SimulationExamID",
                table: "Practice",
                column: "SimulationExamID");

            migrationBuilder.AddForeignKey(
                name: "FK_Practice_SimulationExam_SimulationExamID",
                table: "Practice",
                column: "SimulationExamID",
                principalTable: "SimulationExam",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Practice_SimulationExam_SimulationExamID",
                table: "Practice");

            migrationBuilder.DropIndex(
                name: "IX_Practice_SimulationExamID",
                table: "Practice");

            migrationBuilder.DropColumn(
                name: "SimulationExamID",
                table: "Practice");
        }
    }
}
