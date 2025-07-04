using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsWebApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateAllTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MCourse",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    CourseExplaination = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    BegineDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LearningTime = table.Column<int>(type: "int", nullable: false),
                    PublicFlg = table.Column<bool>(type: "bit", nullable: false),
                    PrimaryReference = table.Column<string>(type: "varchar(1)", nullable: false),
                    DeletedFlg = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCourse", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "MSysCode",
                columns: table => new
                {
                    ClassId = table.Column<string>(type: "varchar(2)", nullable: false),
                    ClassCd = table.Column<string>(type: "varchar(2)", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MSysCode", x => new { x.ClassId, x.ClassCd });
                });

            migrationBuilder.CreateTable(
                name: "MUser",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginId = table.Column<string>(type: "varchar(255)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    Email = table.Column<string>(type: "varchar(128)", nullable: false),
                    EmployeeNo = table.Column<string>(type: "varchar(16)", nullable: false),
                    Remarks1 = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    Remarks2 = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    UserRole = table.Column<string>(type: "varchar(1)", nullable: false),
                    AvailableFlg = table.Column<bool>(type: "bit", nullable: false),
                    TempRegisterId = table.Column<string>(type: "varchar(40)", nullable: false),
                    DeletedFlg = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "DateTime", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUser", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "QuestionCatalog",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MajorCd = table.Column<string>(type: "varchar(1)", nullable: false),
                    MiddleCd = table.Column<string>(type: "varchar(2)", nullable: false),
                    MinorCd = table.Column<string>(type: "varchar(2)", nullable: false),
                    SeqNo = table.Column<string>(type: "varchar(5)", nullable: false),
                    QuestionTitle = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(1024)", nullable: true),
                    QuestionImageName = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    QuestionImageData = table.Column<string>(type: "text", nullable: true),
                    QuestionType = table.Column<string>(type: "varchar(1)", nullable: false),
                    Level = table.Column<string>(type: "varchar(1)", nullable: false),
                    Score = table.Column<byte>(type: "tinyint", nullable: false),
                    DeletedFlg = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionCatalog", x => x.QuestionId);
                });

            migrationBuilder.CreateTable(
                name: "MChapter",
                columns: table => new
                {
                    ChapterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChapterName = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    ContentsType = table.Column<string>(type: "varchar(1)", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNo = table.Column<byte>(type: "tinyint", nullable: false),
                    DeletedFlg = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MChapter", x => x.ChapterId);
                    table.ForeignKey(
                        name: "FK_MChapter_MCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "MCourse",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCourse",
                columns: table => new
                {
                    UserCourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedFlg = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourse", x => x.UserCourseId);
                    table.ForeignKey(
                        name: "FK_UserCourse_MCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "MCourse",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCourse_MUser_UserId",
                        column: x => x.UserId,
                        principalTable: "MUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerGroup",
                columns: table => new
                {
                    AnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(1024)", nullable: true),
                    AnswerImageName = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    AnswerImageData = table.Column<string>(type: "text", nullable: true),
                    ExplanationText = table.Column<string>(type: "nvarchar(1024)", nullable: false),
                    ExplanationImageName = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    ExplanationImageData = table.Column<string>(type: "text", nullable: true),
                    ErrataFlg = table.Column<bool>(type: "bit", nullable: false),
                    DeletedFlg = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerGroup", x => x.AnswerId);
                    table.ForeignKey(
                        name: "FK_AnswerGroup_QuestionCatalog_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionCatalog",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieContents",
                columns: table => new
                {
                    ContentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentsName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    ContentsPath = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    PlaybackTime = table.Column<short>(type: "smallint", nullable: false),
                    DeletedFlg = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieContents", x => x.ContentsId);
                    table.ForeignKey(
                        name: "FK_MovieContents_MChapter_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "MChapter",
                        principalColumn: "ChapterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestContents",
                columns: table => new
                {
                    ContentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentsName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    Questions = table.Column<byte>(type: "tinyint", nullable: false),
                    LimitTime = table.Column<short>(type: "smallint", nullable: false),
                    DeletedFlg = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestContents", x => x.ContentsId);
                    table.ForeignKey(
                        name: "FK_TestContents_MChapter_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "MChapter",
                        principalColumn: "ChapterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserChapter",
                columns: table => new
                {
                    UserChapterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChapter", x => x.UserChapterId);
                    table.ForeignKey(
                        name: "FK_UserChapter_MChapter_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "MChapter",
                        principalColumn: "ChapterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChapter_MUser_UserId",
                        column: x => x.UserId,
                        principalTable: "MUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamList",
                columns: table => new
                {
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamList", x => x.ExamId);
                    table.ForeignKey(
                        name: "FK_ExamList_QuestionCatalog_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionCatalog",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamList_TestContents_ContentsId",
                        column: x => x.ContentsId,
                        principalTable: "TestContents",
                        principalColumn: "ContentsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserExam",
                columns: table => new
                {
                    UserExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserChapterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NthTime = table.Column<byte>(type: "tinyint", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExam", x => x.UserExamId);
                    table.ForeignKey(
                        name: "FK_UserExam_QuestionCatalog_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionCatalog",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserExam_UserChapter_UserChapterId",
                        column: x => x.UserChapterId,
                        principalTable: "UserChapter",
                        principalColumn: "UserChapterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserScore",
                columns: table => new
                {
                    UserScoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserChapterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NthTime = table.Column<byte>(type: "tinyint", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false),
                    AnswerValue = table.Column<bool>(type: "bit", nullable: false),
                    Result = table.Column<byte>(type: "tinyint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserScore", x => x.UserScoreId);
                    table.ForeignKey(
                        name: "FK_UserScore_AnswerGroup_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "AnswerGroup",
                        principalColumn: "AnswerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserScore_UserChapter_UserChapterId",
                        column: x => x.UserChapterId,
                        principalTable: "UserChapter",
                        principalColumn: "UserChapterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerGroup_QuestionId",
                table: "AnswerGroup",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamList_ContentsId",
                table: "ExamList",
                column: "ContentsId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamList_QuestionId",
                table: "ExamList",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_MChapter_CourseId_OrderNo",
                table: "MChapter",
                columns: new[] { "CourseId", "OrderNo" });

            migrationBuilder.CreateIndex(
                name: "IX_MCourse_BegineDateTime",
                table: "MCourse",
                column: "BegineDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_MovieContents_ChapterId",
                table: "MovieContents",
                column: "ChapterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MUser_LoginId",
                table: "MUser",
                column: "LoginId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCatalog_MajorCd_MiddleCd_MinorCd_SeqNo",
                table: "QuestionCatalog",
                columns: new[] { "MajorCd", "MiddleCd", "MinorCd", "SeqNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestContents_ChapterId",
                table: "TestContents",
                column: "ChapterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserChapter_ChapterId",
                table: "UserChapter",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChapter_UserId_CourseId_ChapterId",
                table: "UserChapter",
                columns: new[] { "UserId", "CourseId", "ChapterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCourse_CourseId",
                table: "UserCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourse_UserId_CourseId",
                table: "UserCourse",
                columns: new[] { "UserId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserExam_QuestionId",
                table: "UserExam",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExam_UserChapterId_NthTime_QuestionId",
                table: "UserExam",
                columns: new[] { "UserChapterId", "NthTime", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserScore_AnswerId",
                table: "UserScore",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserScore_UserChapterId_NthTime_QuestionId_AnswerId",
                table: "UserScore",
                columns: new[] { "UserChapterId", "NthTime", "QuestionId", "AnswerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamList");

            migrationBuilder.DropTable(
                name: "MovieContents");

            migrationBuilder.DropTable(
                name: "MSysCode");

            migrationBuilder.DropTable(
                name: "UserCourse");

            migrationBuilder.DropTable(
                name: "UserExam");

            migrationBuilder.DropTable(
                name: "UserScore");

            migrationBuilder.DropTable(
                name: "TestContents");

            migrationBuilder.DropTable(
                name: "AnswerGroup");

            migrationBuilder.DropTable(
                name: "UserChapter");

            migrationBuilder.DropTable(
                name: "QuestionCatalog");

            migrationBuilder.DropTable(
                name: "MChapter");

            migrationBuilder.DropTable(
                name: "MUser");

            migrationBuilder.DropTable(
                name: "MCourse");
        }
    }
}
