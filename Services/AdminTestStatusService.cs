using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class AdminTestStatusService(
        ElsWebAppDbContext ctx,
        ILogger<AdminTestStatusService> logger,
        IUserService svcUser,
        ICourseService svcCourse
    ) : IAdminTestStatusService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<AdminTestStatusService> _logger = logger;
        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        protected IUserService _userService = svcUser;
        protected ICourseService _courseService = svcCourse;

        /// <inheritdoc/>
        public List<CourseStudentTestStatus> GetTestStatus(string courseId, string userId)
        {
            string sql = $@"
                        SELECT
                            UserId
                            , UserName
                            , CompanyName
                            , CourseName
                            , OrderNo
                            , ChapterId
                            , MAX(ChapterName) AS ChapterName 
                            , CASE 
                                WHEN SUM(配点) > 0 
                                    THEN CONCAT(SUM(得点), '/', SUM(配点)) 
                                ELSE '---/---' 
                                END AS '得点'
                            , ISNULL( 
                                CONVERT(VARCHAR (16), MAX(StartDatetime), 111) + ' ' + CONVERT(VARCHAR (8), MAX(StartDatetime), 108)
                                , '----/--/-- --:--:--'
                            ) AS '実施日時'
                        FROM
                            ( 
                                SELECT
                                    baseData.UserId
                                    , baseData.CourseId
                                    , baseData.CourseName
                                    , baseData.OrderNo
                                    , MAX(baseData.UserName) AS UserName
                                    , MAX(baseData.CompanyName) AS CompanyName
                                    , UserExam.QuestionId
                                    , MAX(QuestionCatalog.Score) AS 配点
                                    , MAX(baseData.ChapterId) AS ChapterId
                                    , MAX(ChapterName ) AS ChapterName
                                    , MAX(ContentsName) AS ContentsName
                                    , MAX(UserScore.Result) AS 正答
                                    , CASE 
                                        WHEN MIN(UserScore.Result) = @answerflg 
                                            THEN MAX(QuestionCatalog.Score) 
                                        ELSE 0 
                                        END AS 得点
                                    , MAX(UserChapter.StartDatetime) AS StartDatetime 
                                FROM
                                    ( 
                                        SELECT
                                            MUser.UserId
                                            , MUser.UserName
                                            , MCourse.CourseName
                                            , MCourse.CourseId
                                            , MUser.CompanyName
                                            , MChapter.ChapterName 
                                            , MChapter.ChapterId
                                            , MChapter.OrderNo 
                                        FROM
                                            MUser 
                                            LEFT JOIN UserCourse 
                                                ON MUser.UserId = UserCourse.UserId 
                                            LEFT JOIN MCourse 
                                                ON UserCourse.CourseId = MCourse.CourseId 
                                            LEFT JOIN MChapter 
                                                ON UserCourse.CourseId = MChapter.CourseId 
                                                AND MChapter.ContentsType = @contenttype
                                        WHERE
                                            MUser.DeletedFlg = @delflg 
                                            AND CourseName IS NOT NULL 
                        ";
            if(!string.IsNullOrEmpty(courseId))
            {
                sql += $@"AND MCourse.CourseId = @courseId";
            }
            
            if(!string.IsNullOrEmpty(userId))
            {
                sql += $@"AND MUser.UserId = @userId";
            }

            sql += $@"
                                            
                                    ) baseData 
                                    LEFT JOIN UserChapter 
                                        ON baseData.UserId = UserChapter.UserId 
                                        AND baseData.ChapterId = UserChapter.ChapterId 
                                    LEFT JOIN TestContents 
                                        ON UserChapter.ChapterId = TestContents.ChapterId 
                                    LEFT JOIN UserExam 
                                        ON UserChapter.UserChapterId = UserExam.UserChapterId 
                                        AND UserExam.NthTime = 1 
                                    LEFT JOIN UserScore 
                                        ON UserExam.UserChapterId = UserScore.UserChapterId 
                                        AND UserExam.QuestionId = UserScore.QuestionId 
                                        AND UserExam.NthTime = UserScore.NthTime 
                                    LEFT JOIN QuestionCatalog 
                                        ON UserScore.QuestionId = QuestionCatalog.QuestionId 
                                    LEFT JOIN ExamList 
                                        ON UserExam.QuestionId = ExamList.QuestionId 
                                GROUP BY
                                    baseData.UserId
                                    , baseData.CourseId
                                    , UserExam.QuestionId
                                    , baseData.CourseName
                                    , baseData.OrderNo
                            ) detail 
                        GROUP BY
                            UserId
                            , UserName
                            , CompanyName
                            , CourseName
                            , ContentsName
                            , OrderNo
                            , ChapterId 
                        ORDER BY
                            UserId
                            , CourseName
                            , OrderNo
                        ";

            var courseStudentTestList = _context.Database.SqlQueryRaw<CourseStudentTestStatus>(
               @sql,
                new SqlParameter("@answerflg", ConstService.SystemCode.SYSCODE_ANS_CORRECT),
                new SqlParameter("@contenttype", ConstService.SystemCode.SYSCODE_CON_TEST),
                new SqlParameter("@delflg", ConstService.SystemCode.SYSCODE_DEL_NO),
                new SqlParameter("@courseId", courseId),
                new SqlParameter("@userId", userId)
                ).ToList();

            return courseStudentTestList;
        }

        /// <inheritdoc/>
        public async Task<List<string>> GetScoresHeader(string courseId)
        {
            List<string> headers = [];
            try
            {
                string sql = $@"
                            SELECT
                                ChapterName 
                            FROM
                                MChapter 
                            WHERE
                                MChapter.CourseId = @courseId 
                                AND ContentsType = @contenttype
                                AND DeletedFlg = @delflg 
                            ORDER BY OrderNo
                            ";

                headers = await _context.Database.SqlQueryRaw<string>(
                   @sql,
                    new SqlParameter("@courseId", courseId),
                    new SqlParameter("@contenttype", ConstService.SystemCode.SYSCODE_CON_TEST),
                    new SqlParameter("@delflg", ConstService.SystemCode.SYSCODE_DEL_NO)
                    ).ToListAsync() ?? [];
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }
            return headers;
        }

        /// <inheritdoc/>
        public async Task<List<CourseStudentTestStatus>> FilterTestStatusData(List<CourseStudentTestStatus> courseStudentTestList)
        {
            var listData = new List<CourseStudentTestStatus>(courseStudentTestList);
            for (int i = listData.Count - 1; i >= 0; i--)
            {
                List<TestContents> testContents = await this._context.TestContents.Where(x => x.ChapterId == listData[i].ChapterId).ToListAsync();
                if (testContents.Count < 1)
                {
                    courseStudentTestList.RemoveAt(i);
                }
            }

            return courseStudentTestList;
        }
    }
}
