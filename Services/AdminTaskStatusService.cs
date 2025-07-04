using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class AdminTaskStatusService(
        ElsWebAppDbContext ctx,
        ILogger<AdminTaskStatusService> logger,
        IUserService svcUser,
        ICourseService svcCourse
    ) : IAdminTaskStatusService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<AdminTaskStatusService> _logger = logger;
        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        protected IUserService _userService = svcUser;
        protected ICourseService _courseService = svcCourse;

        /// <inheritdoc/>
        public List<Models.TaskStatus> GetTaskStatus(string courseId, bool isPersonalCourses)
        {
            string sql = "";
            if (string.IsNullOrEmpty(courseId) && !isPersonalCourses)
            {
                sql += $@"
                    SELECT
                        UserId
                        , MAX(LoginId) 'LoginId'
                        , UserName
                        , MIN(StartDatetime) 'StartDatetime'
                        , CASE 
                            WHEN COUNT(CASE WHEN EndDatetime IS NULL THEN 1 END) = 0 
                                THEN MAX(EndDatetime) 
                            END 'EndDatetime'
                        , SUM(ProgressRate) / COUNT(UserName) 'ProgressRate'
                        , CompanyName
                        , '' 'CourseName' 
                        , '' 'ChapterName' 
                        , '' 'CourseId' 
                        , '' 'Status' 
                    FROM
                        (";
            }
            sql += $@"
                SELECT
                    MUser.UserId
                    , MAX(MUser.LoginId) 'LoginId'
                    , MUser.UserName
                    , MCourse.CourseName
                    , CONVERT(VARCHAR(36), MCourse.CourseId) 'CourseId'
                    , MIN(UserChapter.StartDatetime) 'StartDatetime'
                    , CASE 
                        WHEN COUNT( 
                            CASE 
                                WHEN UserChapter.EndDatetime IS NULL THEN 1 END) = 0 
                            THEN MAX(UserChapter.EndDatetime) 
                        END 'EndDatetime'
                    , NULLIF( 
                        COUNT(CASE WHEN UserChapter.Status = @status THEN 1 END)
                        , 0
                    ) / CONVERT(float, COUNT(MChapter.ChapterId)) 'ProgressRate'
                    , MUser.CompanyName 
                    , '' 'ChapterName'
                    , '' 'Status' 
                FROM
                    MUser 
                    LEFT JOIN UserCourse 
                        ON MUser.UserId = UserCourse.UserId 
                    LEFT JOIN MCourse 
                        ON UserCourse.CourseId = MCourse.CourseId 
                    LEFT JOIN MChapter 
                        ON UserCourse.CourseId = MChapter.CourseId 
                    LEFT JOIN UserChapter 
                        ON UserCourse.UserId = UserChapter.UserId 
                        AND MChapter.ChapterId = UserChapter.ChapterId 
                WHERE
                    MUser.DeletedFlg = @delflg 
                    AND CourseName IS NOT NULL 
                GROUP BY
                    MUser.UserId
                    , UserName
                    , CourseName
                    , MCourse.CourseId
                    , MUser.CompanyName";

            if (!string.IsNullOrEmpty(courseId))
            {
                sql += $@"
                    ORDER BY
                        UserName";
            }

            if (isPersonalCourses)
            {
                sql += $@"
                    ORDER BY
                        ProgressRate desc";
            }

            if (string.IsNullOrEmpty(courseId) && !isPersonalCourses)
            {
                sql += $@"
                    ) data 
                    GROUP BY
                        UserId
                        , UserName
                        , CompanyName 
                    ORDER BY
                        UserName";
            }

            var taskStatusList = _context.Database.SqlQueryRaw<Models.TaskStatus>(
               @sql,
                new SqlParameter("@status", ConstService.SystemCode.SYSCODE_STS_COMPLETE),
                new SqlParameter("@delflg", ConstService.SystemCode.SYSCODE_DEL_NO)
                ).ToList();

            return taskStatusList;
        }

        /// <inheritdoc/>
        public List<Models.TaskStatus> GetChapterStatus(string courseId)
        {
            string sql = $@"
                        SELECT
                            MUser.UserId
                            , MUser.LoginId
                            , MUser.UserName
                            , MChapter.ChapterName
                            , CONVERT(VARCHAR (36), MCourse.CourseId) 'CourseId'
                            , UserChapter.StartDatetime
                            , UserChapter.EndDatetime
                            , UserChapter.Status
                            , MUser.CompanyName 
                            , '' 'CourseName' 
                            , NULL 'ProgressRate' 
                        FROM
                            MUser 
                            LEFT JOIN UserCourse 
                                ON MUser.UserId = UserCourse.UserId 
                            LEFT JOIN MCourse 
                                ON UserCourse.CourseId = MCourse.CourseId 
                            LEFT JOIN MChapter 
                                ON UserCourse.CourseId = MChapter.CourseId 
                            LEFT JOIN UserChapter 
                                ON UserCourse.UserId = UserChapter.UserId 
                                AND MChapter.ChapterId = UserChapter.ChapterId 
                        WHERE
                            MUser.DeletedFlg = @delflg  
                            AND CourseName IS NOT NULL
                            AND MCourse.CourseId = @courseId
                        ORDER BY
                            MChapter.OrderNo";

            var taskStatusList = _context.Database.SqlQueryRaw<Models.TaskStatus>(
                @sql,
                new SqlParameter("@delflg", ConstService.SystemCode.SYSCODE_DEL_NO),
                new SqlParameter("@courseId", courseId)
                ).ToList();

            return taskStatusList;
        }

        /// <inheritdoc/>
        public List<DownloadCsvData> GetDownloadCsvData(string corpName, string userName, string courseId)
        {
            var sql = $"SELECT";
                sql = $"{sql} UC.UserId,";
                sql = $"{sql} MU.UserName,";
                sql = $"{sql} MU.Email,";
                sql = $"{sql} MCo.CourseId,";
                sql = $"{sql} MCo.CourseName,";
                sql = $"{sql} MCh.ChapterId,";
                sql = $"{sql} MCh.OrderNo,";
                sql = $"{sql} MCh.ChapterName,";
                sql = $"{sql} MCh.ContentsType,";
                sql = $"{sql} MS0.ClassName AS ContentsTypeName,";
                sql = $"{sql} UC.StartDateTime,";
                sql = $"{sql} UC.EndDateTime,";
                sql = $"{sql} UC.Status,";
                sql = $"{sql} MS1.ClassName AS StatusName,";
                sql = $"{sql} SS.NthTime,";
                sql = $"{sql} SS.ToTal,";
                sql = $"{sql} SS.CollectAnswers";
                sql = $"{sql} FROM";
                sql = $"{sql} UserChapter UC INNER JOIN MUser MU";
                sql = $"{sql} ON UC.UserId = MU.UserId";
                if (!string.IsNullOrWhiteSpace(corpName))
                {
                    sql = $"{sql} AND  MU.CompanyName like @corp";
                }
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    sql = $"{sql} AND  MU.UserName like @user";
                }
                sql = $"{sql} LEFT JOIN";
                sql = $"{sql} (";
                sql = $"{sql} SELECT";
                sql = $"{sql} US.UserChapterId,";
                sql = $"{sql} US.NthTime,";
                sql = $"{sql} COUNT(US.NthTime) AS Total,";
                sql = $"{sql} SUM(CASE WHEN US.Collect= 1 THEN 1 ELSE 0 END) AS CollectAnswers";
                sql = $"{sql} FROM";
                sql = $"{sql} (";
                sql = $"{sql} SELECT";
                sql = $"{sql} UserChapterId,";
                sql = $"{sql} NthTime,";
                sql = $"{sql} QuestionId,";
                sql = $"{sql} MIN(Result) AS Collect";
                sql = $"{sql} FROM";
                sql = $"{sql} UserScore";
                sql = $"{sql} GROUP BY";
                sql = $"{sql} UserChapterId,";
                sql = $"{sql} NthTime,";
                sql = $"{sql} QuestionId";
                sql = $"{sql} ) US";
                sql = $"{sql} GROUP BY";
                sql = $"{sql} UserChapterId,";
                sql = $"{sql} NthTime";
                sql = $"{sql} ) SS";
                sql = $"{sql} ON UC.UserChapterId = SS.UserChapterId";
                sql = $"{sql} LEFT JOIN MCourse MCo";
                sql = $"{sql} ON UC.CourseId = MCo.CourseId";
                sql = $"{sql} LEFT JOIN MChapter MCh";
                sql = $"{sql} ON UC.CourseId = MCh.CourseId";
                sql = $"{sql} AND UC.ChapterId = MCh.ChapterId";
                sql = $"{sql} LEFT JOIN MSysCode MS0";
                sql = $"{sql} ON MS0.ClassId = @classid0"; 
                sql = $"{sql} AND MCh.ContentsType = MS0.ClassCd";
                sql = $"{sql} LEFT JOIN MSysCode MS1";
                sql = $"{sql} ON MS1.ClassId = @classid1"; 
                sql = $"{sql} AND UC.Status = MS1.ClassCd";
                sql = $"{sql} WHERE";
                sql = $"{sql} MU.DeletedFlg = @delflg";
                if (!string.IsNullOrWhiteSpace(courseId))
                { 
                    sql = $"{sql} AND UC.CourseId = @course";
                }
                sql = $"{sql} ORDER BY";
                sql = $"{sql} UserName,";
                sql = $"{sql} UserId,";
                sql = $"{sql} CourseId,";
                sql = $"{sql} OrderNo,";
                sql = $"{sql} NthTime";

            var lst = _context.Database.SqlQueryRaw<DownloadCsvData>(
                @sql,
                new SqlParameter("@delflg", ConstService.SystemCode.SYSCODE_DEL_NO),
                new SqlParameter("@classid0", ConstService.SystemCode.SYSCODE_CLASS_CONTENTS),
                new SqlParameter("@classid1", ConstService.SystemCode.SYSCODE_CLASS_STATUS),
                new SqlParameter("@corp", $"{corpName}%"),
                new SqlParameter("@user", $"{userName}%"),
                new SqlParameter("@course", $"{courseId}")
                ).ToList();

            return lst;
        }
    }
}


