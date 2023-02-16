using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Testing.API.Infrastructure.TestingMigrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TitleValue = table.Column<string>(name: "Title_Value", type: "text", nullable: false),
                    DescriptionText = table.Column<string>(name: "Description_Text", type: "text", nullable: false),
                    DescriptionExamples = table.Column<string>(name: "Description_Examples", type: "text", nullable: false),
                    DescriptionSomeCases = table.Column<string>(name: "Description_SomeCases", type: "text", nullable: true),
                    DescriptionNote = table.Column<string>(name: "Description_Note", type: "text", nullable: true),
                    DifficultyId = table.Column<int>(name: "Difficulty_Id", type: "integer", nullable: false),
                    DifficultyName = table.Column<string>(name: "Difficulty_Name", type: "text", nullable: false),
                    TypeId = table.Column<int>(name: "Type_Id", type: "integer", nullable: false),
                    TypeName = table.Column<string>(name: "Type_Name", type: "text", nullable: false),
                    ProgrammingLanguageId = table.Column<int>(name: "ProgrammingLanguage_Id", type: "integer", nullable: false),
                    ProgrammingLanguageName = table.Column<string>(name: "ProgrammingLanguage_Name", type: "text", nullable: false),
                    SolutionExampleDescription = table.Column<string>(name: "SolutionExample_Description", type: "text", nullable: true),
                    SolutionExampleSolution = table.Column<string>(name: "SolutionExample_Solution", type: "text", nullable: false),
                    ExecutionConditionTests = table.Column<string>(name: "ExecutionCondition_Tests", type: "text", nullable: false),
                    ExecutionConditionTimeLimit = table.Column<TimeSpan>(name: "ExecutionCondition_TimeLimit", type: "interval", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Solutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(name: "User_Id", type: "uuid", nullable: false),
                    UserEmail = table.Column<string>(name: "User_Email", type: "text", nullable: false),
                    UserLastname = table.Column<string>(name: "User_Lastname", type: "text", nullable: false),
                    UserFirstname = table.Column<string>(name: "User_Firstname", type: "text", nullable: false),
                    ValueValue = table.Column<string>(name: "Value_Value", type: "text", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solutions_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_TaskId",
                table: "Solutions",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Solutions");

            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
