using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examination_System.Migrations
{
    /// <inheritdoc />
    public partial class updatePhase2QuestionBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Question_Bank_insID",
                table: "Question_Bank",
                column: "insID");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Bank_Instructor_insID",
                table: "Question_Bank",
                column: "insID",
                principalTable: "Instructor",
                principalColumn: "insid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_Bank_Instructor_insID",
                table: "Question_Bank");

            migrationBuilder.DropIndex(
                name: "IX_Question_Bank_insID",
                table: "Question_Bank");
        }
    }
}
