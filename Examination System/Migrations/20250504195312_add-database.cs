using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examination_System.Migrations
{
    /// <inheritdoc />
    public partial class adddatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    crsid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    crsname = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    hours = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__courses__A1BDCF3284EBE03C", x => x.crsid);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    dept_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departme__DCA659745EFD4017", x => x.dept_id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    PeriD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerTitle = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permissi__49641868BC4F7E2B", x => x.PeriD);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleTitle = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__8AFACE1AE63B84D8", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Question_Bank",
                columns: table => new
                {
                    QID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    crsid = table.Column<int>(type: "int", nullable: true),
                    mark = table.Column<int>(type: "int", nullable: true),
                    type = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Question__CAB147CB08B6EB5D", x => x.QID);
                    table.ForeignKey(
                        name: "FK__Question___crsid__4AB81AF0",
                        column: x => x.crsid,
                        principalTable: "courses",
                        principalColumn: "crsid");
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    topicid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    crsid = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Topics__7C3F7559076BAE60", x => x.topicid);
                    table.ForeignKey(
                        name: "FK__Topics__crsid__47DBAE45",
                        column: x => x.crsid,
                        principalTable: "courses",
                        principalColumn: "crsid");
                });

            migrationBuilder.CreateTable(
                name: "Permission_Role",
                columns: table => new
                {
                    PeriD = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permissi__F1CBB48B4D1567ED", x => new { x.PeriD, x.RoleID });
                    table.ForeignKey(
                        name: "FK__Permissio__PeriD__6C190EBB",
                        column: x => x.PeriD,
                        principalTable: "Permission",
                        principalColumn: "PeriD");
                    table.ForeignKey(
                        name: "FK__Permissio__RoleI__6D0D32F4",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleId");
                });

            migrationBuilder.CreateTable(
                name: "answers",
                columns: table => new
                {
                    ansid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quesid = table.Column<int>(type: "int", nullable: false),
                    isCorrect = table.Column<bool>(type: "bit", nullable: false),
                    title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__answers__2626EE586D8F3B74", x => x.ansid);
                    table.ForeignKey(
                        name: "FK__answers__quesid__5165187F",
                        column: x => x.quesid,
                        principalTable: "Question_Bank",
                        principalColumn: "QID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Branch_Dept",
                columns: table => new
                {
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    dept_id = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch_Dept", x => new { x.branch_id, x.dept_id });
                    table.ForeignKey(
                        name: "FK_Branch_Dept_Department_dept_id",
                        column: x => x.dept_id,
                        principalTable: "Department",
                        principalColumn: "dept_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    branch_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    location = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Branches__E55E37DE0C6F72E1", x => x.branch_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    st_city = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    dept_id = table.Column<int>(type: "int", nullable: true),
                    password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    branch_id = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3213E83F8B9D238A", x => x.id);
                    table.ForeignKey(
                        name: "FK__Users__branch_id__32E0915F",
                        column: x => x.branch_id,
                        principalTable: "Branches",
                        principalColumn: "branch_id");
                    table.ForeignKey(
                        name: "FK__Users__dept_id__31EC6D26",
                        column: x => x.dept_id,
                        principalTable: "Department",
                        principalColumn: "dept_id");
                });

            migrationBuilder.CreateTable(
                name: "Instructor",
                columns: table => new
                {
                    insid = table.Column<int>(type: "int", nullable: false),
                    jobTitle = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    salary = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Instruct__116B52F3D65ACC47", x => x.insid);
                    table.ForeignKey(
                        name: "FK__Instructo__insid__398D8EEE",
                        column: x => x.insid,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    stdid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Student__BA09E293A3A9E8B5", x => x.stdid);
                    table.ForeignKey(
                        name: "FK__Student__stdid__5441852A",
                        column: x => x.stdid,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "User_Role",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Rol__AF27604D4E94EA31", x => new { x.UserID, x.RoleId });
                    table.ForeignKey(
                        name: "FK__User_Role__RoleI__693CA210",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId");
                    table.ForeignKey(
                        name: "FK__User_Role__UserI__68487DD7",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "course_dept",
                columns: table => new
                {
                    crsid = table.Column<int>(type: "int", nullable: false),
                    dept_id = table.Column<int>(type: "int", nullable: false),
                    insid = table.Column<int>(type: "int", nullable: false),
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__course_d__73989414A190A2A8", x => new { x.crsid, x.dept_id, x.insid, x.branch_id });
                    table.ForeignKey(
                        name: "FK__course_de__branc__412EB0B6",
                        column: x => x.branch_id,
                        principalTable: "Branches",
                        principalColumn: "branch_id");
                    table.ForeignKey(
                        name: "FK__course_de__crsid__3E52440B",
                        column: x => x.crsid,
                        principalTable: "courses",
                        principalColumn: "crsid");
                    table.ForeignKey(
                        name: "FK__course_de__dept___3F466844",
                        column: x => x.dept_id,
                        principalTable: "Department",
                        principalColumn: "dept_id");
                    table.ForeignKey(
                        name: "FK__course_de__insid__403A8C7D",
                        column: x => x.insid,
                        principalTable: "Instructor",
                        principalColumn: "insid");
                });

            migrationBuilder.CreateTable(
                name: "Exam",
                columns: table => new
                {
                    Exid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    crsid = table.Column<int>(type: "int", nullable: true),
                    startat = table.Column<DateTime>(type: "datetime", nullable: true),
                    endat = table.Column<DateTime>(type: "datetime", nullable: true),
                    duration = table.Column<int>(type: "int", nullable: true),
                    insid = table.Column<int>(type: "int", nullable: true),
                    deptid = table.Column<int>(type: "int", nullable: false),
                    branchid = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Exam__36723235AB8F771D", x => x.Exid);
                    table.ForeignKey(
                        name: "FK_Exam_Branches_branchid",
                        column: x => x.branchid,
                        principalTable: "Branches",
                        principalColumn: "branch_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exam_Department_deptid",
                        column: x => x.deptid,
                        principalTable: "Department",
                        principalColumn: "dept_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Exam__crsid__440B1D61",
                        column: x => x.crsid,
                        principalTable: "courses",
                        principalColumn: "crsid");
                    table.ForeignKey(
                        name: "FK__Exam__insid__44FF419A",
                        column: x => x.insid,
                        principalTable: "Instructor",
                        principalColumn: "insid");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    intakeNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stdid = table.Column<int>(type: "int", nullable: true),
                    crsid = table.Column<int>(type: "int", nullable: true),
                    grade = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    comments = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comments__0F6969FA8DE558D4", x => x.intakeNo);
                    table.ForeignKey(
                        name: "FK__Comments__crsid__5FB337D6",
                        column: x => x.crsid,
                        principalTable: "courses",
                        principalColumn: "crsid");
                    table.ForeignKey(
                        name: "FK__Comments__stdid__5EBF139D",
                        column: x => x.stdid,
                        principalTable: "Student",
                        principalColumn: "stdid");
                });

            migrationBuilder.CreateTable(
                name: "Student_course",
                columns: table => new
                {
                    stdid = table.Column<int>(type: "int", nullable: false),
                    crsid = table.Column<int>(type: "int", nullable: false),
                    grade = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    comments = table.Column<string>(type: "text", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Student___80123E60AADD987E", x => new { x.stdid, x.crsid });
                    table.ForeignKey(
                        name: "FK__Student_c__crsid__5812160E",
                        column: x => x.crsid,
                        principalTable: "courses",
                        principalColumn: "crsid");
                    table.ForeignKey(
                        name: "FK__Student_c__stdid__571DF1D5",
                        column: x => x.stdid,
                        principalTable: "Student",
                        principalColumn: "stdid");
                });

            migrationBuilder.CreateTable(
                name: "question_Exam",
                columns: table => new
                {
                    QID = table.Column<int>(type: "int", nullable: false),
                    ExamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__question__A82615D759C41BD1", x => new { x.QID, x.ExamId });
                    table.ForeignKey(
                        name: "FK__question_Ex__QID__4D94879B",
                        column: x => x.QID,
                        principalTable: "Question_Bank",
                        principalColumn: "QID");
                    table.ForeignKey(
                        name: "FK__question___ExamI__4E88ABD4",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Exid");
                });

            migrationBuilder.CreateTable(
                name: "Student_answer",
                columns: table => new
                {
                    stdid = table.Column<int>(type: "int", nullable: false),
                    ansid = table.Column<int>(type: "int", nullable: false),
                    qid = table.Column<int>(type: "int", nullable: false),
                    examId = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Student___2D43AD6563D95B27", x => new { x.stdid, x.ansid, x.qid, x.examId });
                    table.ForeignKey(
                        name: "FK__Student_a__ansid__6383C8BA",
                        column: x => x.ansid,
                        principalTable: "answers",
                        principalColumn: "ansid");
                    table.ForeignKey(
                        name: "FK__Student_a__examI__656C112C",
                        column: x => x.examId,
                        principalTable: "Exam",
                        principalColumn: "Exid");
                    table.ForeignKey(
                        name: "FK__Student_a__stdid__628FA481",
                        column: x => x.stdid,
                        principalTable: "Student",
                        principalColumn: "stdid");
                    table.ForeignKey(
                        name: "FK__Student_ans__qid__6477ECF3",
                        column: x => x.qid,
                        principalTable: "Question_Bank",
                        principalColumn: "QID");
                });

            migrationBuilder.CreateTable(
                name: "Student_Exam",
                columns: table => new
                {
                    stdid = table.Column<int>(type: "int", nullable: false),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Student___D89EB08F995384E9", x => new { x.stdid, x.ExamId });
                    table.ForeignKey(
                        name: "FK__Student_E__ExamI__5BE2A6F2",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Exid");
                    table.ForeignKey(
                        name: "FK__Student_E__stdid__5AEE82B9",
                        column: x => x.stdid,
                        principalTable: "Student",
                        principalColumn: "stdid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_answers_quesid",
                table: "answers",
                column: "quesid");

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Dept_branch_id_dept_id",
                table: "Branch_Dept",
                columns: new[] { "branch_id", "dept_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Dept_dept_id",
                table: "Branch_Dept",
                column: "dept_id");

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Dept_ManagerId",
                table: "Branch_Dept",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_location",
                table: "Branches",
                column: "location",
                unique: true,
                filter: "[location] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_ManagerId",
                table: "Branches",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_name",
                table: "Branches",
                column: "name",
                unique: true,
                filter: "[name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_crsid",
                table: "Comments",
                column: "crsid");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_stdid",
                table: "Comments",
                column: "stdid");

            migrationBuilder.CreateIndex(
                name: "IX_course_dept_branch_id",
                table: "course_dept",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_dept_dept_id",
                table: "course_dept",
                column: "dept_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_dept_insid",
                table: "course_dept",
                column: "insid");

            migrationBuilder.CreateIndex(
                name: "IX_Department_name",
                table: "Department",
                column: "name",
                unique: true,
                filter: "[name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_branchid",
                table: "Exam",
                column: "branchid");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_crsid",
                table: "Exam",
                column: "crsid");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_deptid",
                table: "Exam",
                column: "deptid");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_insid",
                table: "Exam",
                column: "insid");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Role_RoleID",
                table: "Permission_Role",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Question_Bank_crsid",
                table: "Question_Bank",
                column: "crsid");

            migrationBuilder.CreateIndex(
                name: "IX_question_Exam_ExamId",
                table: "question_Exam",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_answer_ansid",
                table: "Student_answer",
                column: "ansid");

            migrationBuilder.CreateIndex(
                name: "IX_Student_answer_examId",
                table: "Student_answer",
                column: "examId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_answer_qid",
                table: "Student_answer",
                column: "qid");

            migrationBuilder.CreateIndex(
                name: "IX_Student_course_crsid",
                table: "Student_course",
                column: "crsid");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Exam_ExamId",
                table: "Student_Exam",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_crsid",
                table: "Topics",
                column: "crsid");

            migrationBuilder.CreateIndex(
                name: "IX_User_Role_RoleId",
                table: "User_Role",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_branch_id",
                table: "Users",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_dept_id",
                table: "Users",
                column: "dept_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Dept_Branches_branch_id",
                table: "Branch_Dept",
                column: "branch_id",
                principalTable: "Branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Dept_Instructor_ManagerId",
                table: "Branch_Dept",
                column: "ManagerId",
                principalTable: "Instructor",
                principalColumn: "insid");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Instructor_ManagerId",
                table: "Branches",
                column: "ManagerId",
                principalTable: "Instructor",
                principalColumn: "insid",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Users__branch_id__32E0915F",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Branch_Dept");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "course_dept");

            migrationBuilder.DropTable(
                name: "Permission_Role");

            migrationBuilder.DropTable(
                name: "question_Exam");

            migrationBuilder.DropTable(
                name: "Student_answer");

            migrationBuilder.DropTable(
                name: "Student_course");

            migrationBuilder.DropTable(
                name: "Student_Exam");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "User_Role");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "answers");

            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Question_Bank");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Instructor");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
