using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examination_System.Migrations
{
    /// <inheritdoc />
    public partial class updateBranchDept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropTable(
                name: "Branche_Dept");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Dept_Branch",
                table: "Branch_Dept",
                column: "branch_id",
                principalTable: "Branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Dept_Department",
                table: "Branch_Dept",
                column: "dept_id",
                principalTable: "Department",
                principalColumn: "dept_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Dept_Instructor",
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
                name: "FK_Branch_Dept_Branch",
                table: "Branch_Dept");

            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Dept_Department",
                table: "Branch_Dept");

            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Dept_Instructor",
                table: "Branch_Dept");

            migrationBuilder.CreateTable(
                name: "Branche_Dept",
                columns: table => new
                {
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    dept_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Branche___B8945249DCE80468", x => new { x.branch_id, x.dept_id });
                    table.ForeignKey(
                        name: "FK__Branche_D__branc__2E1BDC42",
                        column: x => x.branch_id,
                        principalTable: "Branches",
                        principalColumn: "branch_id");
                    table.ForeignKey(
                        name: "FK__Branche_D__dept___2F10007B",
                        column: x => x.dept_id,
                        principalTable: "Department",
                        principalColumn: "dept_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branche_Dept_dept_id",
                table: "Branche_Dept",
                column: "dept_id");

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
    }
}
