using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace ElsWebApp.Services
{
    public class StudentCoursesService(
        ElsWebAppDbContext ctx,
        ILogger<StudentMyCourseService> logger,
        ISysCodeService svcSysCode
    ) : IStudentCoursesService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<StudentMyCourseService> _logger = logger;
        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        protected ISysCodeService _sysCodeService = svcSysCode;

        /// <inheritdoc/>
        public async Task<List<StudentCourse>> GetUserCourseList(Guid userId)
        {
            // 講座一覧を取得
            string sql = $@"
                    SELECT
                         CourseId
                        ,CourseName
                        ,BegineDateTime
                        ,EndDateTime
                        ,CourseExplaination
                        ,CAST(PublicFlg AS BIT) AS PublicFlg
                        ,CASE WHEN PublicFlg = 1 THEN @yesPublic ELSE @noPublic END  AS PublicName
                        ,CAST(CASE WHEN PublicFlg  = 0 THEN 0
                                   WHEN DeletedFlg = 1 THEN 0
                                   ELSE 1
                         END AS BIT) AS AvailableFlg
                        ,CAST(CASE WHEN MyCourseId IS NULL THEN 0 ELSE 1 END AS BIT) AS LearningFlag
                    FROM (
                        SELECT
                             MCourse.CourseId
                            ,MCourse.CourseName
                            ,MCourse.BegineDateTime
                            ,MCourse.EndDateTime
                            ,MCourse.CourseExplaination
                            ,MCourse.DeletedFlg
                            ,CASE WHEN PrimaryReference = @sysPriFlg
                                  THEN PublicFlg 
                                  WHEN PrimaryReference = @sysPriPeriod
                                  THEN CASE WHEN BegineDateTime >  GETDATE() OR EndDateTime < GETDATE() THEN 0 ELSE 1 END
                                  ELSE 0
                             END AS PublicFlg
                            ,UserCourse.CourseId AS MyCourseId
                        FROM MCourse 
                        LEFT JOIN UserCourse 
                            ON  UserCourse.CourseId = MCourse.CourseId 
                            AND UserCourse.UserId = @userId
                            AND UserCourse.DeletedFlg = 0
                        WHERE MCourse.DeletedFlg = 0

                        UNION

                        SELECT 
                             MCourse.CourseId
                            ,MCourse.CourseName
                            ,MCourse.BegineDateTime
                            ,MCourse.EndDateTime
                            ,MCourse.CourseExplaination
                            ,MCourse.DeletedFlg
                            ,CASE WHEN PrimaryReference = @sysPriFlg
                                  THEN PublicFlg 
                                  WHEN PrimaryReference = @sysPriPeriod
                                  THEN CASE WHEN BegineDateTime >  GETDATE() OR EndDateTime < GETDATE() THEN 0 ELSE 1 END
                                  ELSE 0
                             END AS PublicFlg
                            ,UserCourse.CourseId AS MyCourseId
                        FROM MCourse 
                        LEFT JOIN UserCourse 
                            ON  UserCourse.CourseId = MCourse.CourseId 
                            AND UserCourse.UserId = @userId
                            AND UserCourse.DeletedFlg = 0
                        WHERE MCourse.DeletedFlg = 1
                            AND UserCourse.UserCourseId IS NOT NULL
                    ) C
                    ORDER BY
                            PublicFlg DESC
                        ,BegineDateTime
                        ,CourseName
                    ";

            // 公開・非公開を取得
            var publicList = await this._sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_PUBLIC);
            var yesPublic = CommonService.GetValueByCode(publicList, ConstService.SystemCode.SYSCODE_PUB_YES);
            var noPublic = CommonService.GetValueByCode(publicList, ConstService.SystemCode.SYSCODE_PUB_NO);
            var courseList = await _context.Database.SqlQueryRaw<StudentCourse>(
                @sql,
                    new SqlParameter("@yesPublic", yesPublic),
                    new SqlParameter("@noPublic", noPublic),
                    new SqlParameter("@sysPriFlg", ConstService.SystemCode.SYSCODE_PRI_FLAG),
                    new SqlParameter("@sysPriPeriod", ConstService.SystemCode.SYSCODE_PRI_PERIOD),
                    new SqlParameter("@userId", userId)
                ).ToListAsync();
            return courseList;
        }

        /// <inheritdoc/>
        public async Task<UserCourse> SelectUserCourseByKey(Guid userId, Guid courseId)
        {
            UserCourse userCourse = await this._context.UserCourse
                    .Where(x => x.UserId == userId)
                    .Where(x => x.CourseId == courseId)
                    .FirstOrDefaultAsync() ?? new();
            return userCourse;
        }

        /// <inheritdoc/>
        public async Task<int> UpdateByDeletedFlg(UserCourse userCourse)
        {
            var result = 0;
            // 更新対象データを検索
            UserCourse updUserCourse = await this.SelectUserCourseByKey(userCourse.UserId, userCourse.CourseId);

            // 存在したら更新する
            if (updUserCourse != null)
            {
                updUserCourse.DeletedFlg = userCourse.DeletedFlg;
                updUserCourse.UpdatedBy = userCourse.UpdatedBy;

                result = this._context.SaveChanges();
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> InsertUserCourse(UserCourse userCourse)
        {
            int result = 0;
            try
            {
                // 受講者コースを登録
                this._context.UserCourse.Add(userCourse);

                // 講座リストから、受講者講座を生成
                List<UserChapter> userChapterList = await this._context.MChapter
                    .Where(x => x.CourseId == userCourse.CourseId)
                    .Where(x => x.DeletedFlg == false)
                    .Select(
                        x => new UserChapter
                        {
                            UserId = userCourse.UserId,
                            CourseId = x.CourseId,
                            ChapterId = x.ChapterId,
                            Status = ConstService.SystemCode.SYSCODE_STS_WAITING,
                            UpdatedBy = userCourse.UpdatedBy,
                            CreatedBy = userCourse.CreatedBy,
                        }
                    )
                    .ToListAsync();

                if (userChapterList != null && userChapterList.Count > 0)
                {
                    // コースとセットで登録するので、未登録のはずだが、
                    // 念のため受講者講座データが存在していないことを確認
                    int userChapaterDataCount = await this._context.UserChapter
                            .Where(x => x.UserId == userCourse.UserId)
                            .Where(x => x.CourseId == userCourse.CourseId)
                            .CountAsync();
                    if (userChapaterDataCount == 0)
                    {
                        // 受講者講座を一括登録
                        this._context.UserChapter.UpdateRange(userChapterList);
                    }
                }
                // データベースに反映
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
                result = 0;
            }
            return result;
        }

        public async Task<List<Chapters>> GetChapterList(string courseId)
        {
            string sql = $@"
                      SELECT 
                           MChapter.OrderNo 'OrderNo'
                         , MChapter.ChapterName 'ChapterName'
                         , MChapter.ContentsType 'ContentsType'
                         , MovieContents.PlaybackTime 'PlaybackTime'
                         , '' 'LimitTime'
                      FROM 
                          MChapter
                      INNER JOIN MovieContents
                          ON MChapter.CourseId = @courseId
                          AND MChapter.DeletedFlg =  @sysDelNo
                          AND MChapter.ChapterId = MovieContents.ChapterId
                          AND MovieContents.DeletedFlg = @sysDelNo
                      UNION
                      SELECT 
                           MChapter.OrderNo 'OrderNo'
                         , MChapter.ChapterName 'ChapterName'
                         , MChapter.ContentsType 'ContentsType'
                         , '' 'PlaybackTime'
                         , TestContents.LimitTime 'LimitTime'
                      FROM 
                          MChapter
                      INNER JOIN TestContents
                          ON MChapter.CourseId = @courseId
                          AND MChapter.DeletedFlg = @sysDelNo
                          AND MChapter.ChapterId = TestContents.ChapterId
                          AND TestContents.DeletedFlg = @sysDelNo
                      ORDER BY 
                          MChapter.OrderNo                      
                   ";

            var chapterList = await _context.Database.SqlQueryRaw<Chapters>(
                @sql,
                    new SqlParameter("@courseId", courseId),
                    new SqlParameter("@sysDelNo", ConstService.SystemCode.SYSCODE_DEL_NO)
                ).ToListAsync();
            return chapterList;
        }
    }
}



