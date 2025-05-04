using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examination_System.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Dept_Branch",
                table: "Branch_Dept");

            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Dept_Department",
                table: "Branch_Dept");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Instructor_ManagerId",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Department_ManagerId",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Department");

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Branch_Dept",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Dept_ManagerId",
                table: "Branch_Dept",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Dept_Branches_branch_id",
                table: "Branch_Dept",
                column: "branch_id",
                principalTable: "Branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Dept_Department_dept_id",
                table: "Branch_Dept",
                column: "dept_id",
                principalTable: "Department",
                principalColumn: "dept_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Dept_Instructor_ManagerId",
                table: "Branch_Dept",
                column: "ManagerId",
                principalTable: "Instructor",
                principalColumn: "insid",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Dept_Branches_branch_id",
                table: "Branch_Dept");

            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Dept_Department_dept_id",
                table: "Branch_Dept");

            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Dept_Instructor_ManagerId",
                table: "Branch_Dept");

            migrationBuilder.DropIndex(
                name: "IX_Branch_Dept_ManagerId",
                table: "Branch_Dept");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Branch_Dept");

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Department",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_ManagerId",
                table: "Department",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Dept_Branch",
                table: "Branch_Dept",
                column: "branch_id",
                principalTable: "Branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Dept_Department",
                table: "Branch_Dept",
                column: "dept_id",
                principalTable: "Department",
                principalColumn: "dept_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Instructor_ManagerId",
                table: "Department",
                column: "ManagerId",
                principalTable: "Instructor",
                principalColumn: "insid",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
