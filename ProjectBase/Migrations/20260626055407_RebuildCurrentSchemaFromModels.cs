using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBase.Migrations
{
    /// <inheritdoc />
    public partial class RebuildCurrentSchemaFromModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PricePackages",
                table: "PricePackages");

            migrationBuilder.RenameTable(
                name: "PricePackages",
                newName: "Price_package");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Dob",
                table: "Users",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpires",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "registerDate",
                table: "Subjects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "publishAt",
                table: "Slider",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updatedAt",
                table: "Slider",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "userID",
                table: "Slider",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "PackageType",
                table: "Price_package",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Price_package",
                table: "Price_package",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "PracticeLevel",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeLevel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "QuizBank",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectID = table.Column<long>(type: "bigint", nullable: false),
                    TopicID = table.Column<int>(type: "int", nullable: false),
                    LevelID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    GroupID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qcorrect = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizBank", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Recipe",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PricePackage_ID = table.Column<long>(type: "bigint", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    SubjectID = table.Column<long>(type: "bigint", nullable: false),
                    PricePackage_Type = table.Column<long>(type: "bigint", nullable: false),
                    BuyAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Recipe_Price_package_PricePackage_ID",
                        column: x => x.PricePackage_ID,
                        principalTable: "Price_package",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recipe_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recipe_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Subject_Category",
                columns: table => new
                {
                    SubjectID = table.Column<long>(type: "bigint", nullable: false),
                    CategoryID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject_Category", x => new { x.SubjectID, x.CategoryID });
                    table.ForeignKey(
                        name: "FK_Subject_Category_Category_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subject_Category_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectTopic",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subjectId = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectTopic", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SimulationExam",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectID = table.Column<long>(type: "bigint", nullable: false),
                    LevelID = table.Column<int>(type: "int", nullable: false),
                    ExamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number_Question = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Passrate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationExam", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SimulationExam_PracticeLevel_LevelID",
                        column: x => x.LevelID,
                        principalTable: "PracticeLevel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimulationExam_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizHandle",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    PracticeID = table.Column<long>(type: "bigint", nullable: false),
                    QuizID = table.Column<long>(type: "bigint", nullable: false),
                    QAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isMark = table.Column<bool>(type: "bit", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    isCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizHandle", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuizHandle_QuizBank_QuizID",
                        column: x => x.QuizID,
                        principalTable: "QuizBank",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Practice",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    SubjectID = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    taken_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    duration = table.Column<TimeOnly>(type: "time", nullable: false),
                    number_quest = table.Column<int>(type: "int", nullable: false),
                    number_correct = table.Column<int>(type: "int", nullable: false),
                    levelID = table.Column<int>(type: "int", nullable: false),
                    topicID = table.Column<int>(type: "int", nullable: false),
                    time_taken = table.Column<TimeOnly>(type: "time", nullable: false),
                    Quest_group = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Practice", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Practice_PracticeLevel_levelID",
                        column: x => x.levelID,
                        principalTable: "PracticeLevel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Practice_SubjectTopic_topicID",
                        column: x => x.topicID,
                        principalTable: "SubjectTopic",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Practice_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Practice_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Slider_userID",
                table: "Slider",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Price_package_SubjectID",
                table: "Price_package",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Practice_levelID",
                table: "Practice",
                column: "levelID");

            migrationBuilder.CreateIndex(
                name: "IX_Practice_SubjectID",
                table: "Practice",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Practice_topicID",
                table: "Practice",
                column: "topicID");

            migrationBuilder.CreateIndex(
                name: "IX_Practice_UserID",
                table: "Practice",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizHandle_QuizID",
                table: "QuizHandle",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_PricePackage_ID",
                table: "Recipe",
                column: "PricePackage_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_SubjectID",
                table: "Recipe",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_UserID",
                table: "Recipe",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationExam_LevelID",
                table: "SimulationExam",
                column: "LevelID");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationExam_SubjectID",
                table: "SimulationExam",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_Category_CategoryID",
                table: "Subject_Category",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Price_package_Subjects_SubjectID",
                table: "Price_package",
                column: "SubjectID",
                principalTable: "Subjects",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Slider_Users_userID",
                table: "Slider",
                column: "userID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Role_RoleID",
                table: "Users",
                column: "RoleID",
                principalTable: "Role",
                principalColumn: "RoleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Price_package_Subjects_SubjectID",
                table: "Price_package");

            migrationBuilder.DropForeignKey(
                name: "FK_Slider_Users_userID",
                table: "Slider");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Role_RoleID",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Practice");

            migrationBuilder.DropTable(
                name: "QuizHandle");

            migrationBuilder.DropTable(
                name: "Recipe");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "SimulationExam");

            migrationBuilder.DropTable(
                name: "Subject_Category");

            migrationBuilder.DropTable(
                name: "SubjectTopic");

            migrationBuilder.DropTable(
                name: "QuizBank");

            migrationBuilder.DropTable(
                name: "PracticeLevel");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Slider_userID",
                table: "Slider");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Price_package",
                table: "Price_package");

            migrationBuilder.DropIndex(
                name: "IX_Price_package_SubjectID",
                table: "Price_package");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpires",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "registerDate",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "publishAt",
                table: "Slider");

            migrationBuilder.DropColumn(
                name: "updatedAt",
                table: "Slider");

            migrationBuilder.DropColumn(
                name: "userID",
                table: "Slider");

            migrationBuilder.RenameTable(
                name: "Price_package",
                newName: "PricePackages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Dob",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PackageType",
                table: "PricePackages",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PricePackages",
                table: "PricePackages",
                column: "ID");
        }
    }
}
