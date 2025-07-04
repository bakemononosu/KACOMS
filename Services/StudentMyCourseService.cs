using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace ElsWebApp.Services
{
    public class StudentMyCourseService(
        ElsWebAppDbContext ctx,
        ILogger<StudentMyCourseService> logger,
        ICourseService svcCourse,
        ISysCodeService svcSysCode
    ) : IStudentMyCourseService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<StudentMyCourseService> _logger = logger;
        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        protected ICourseService _courseService = svcCourse;

        protected ISysCodeService _sysCodeService = svcSysCode;


        /// <inheritdoc/>
        public async Task<string> GetCourseName(string courseId)
        {
            MCourse myCourse = await this._courseService.SelectById(courseId);

            return myCourse.CourseName;
        }

        public async Task<List<MyCourse>> GetUserCourseList(string userId)
        {
            string sql = $@"
                       SELECT
                           MCourse.CourseName 'CourseName'
                          , CONVERT(VARCHAR(36),UserCourse.CourseId) 'CourseId'
                          , MIN(UserChapter.StartDatetime) 'StartDatetime'
                          , CASE 
                               WHEN COUNT( 
                                   CASE 
                                       WHEN UserChapter.EndDatetime IS NULL THEN 1 END) = 0 
                                   THEN MAX(UserChapter.EndDatetime) 
                               END 'EndDatetime'
                          , NULLIF( 
                              COUNT(CASE WHEN UserChapter.Status = @sysStsComplete THEN 1 END)
                              , 0
                          ) / CONVERT(float, COUNT(CONVERT(INT, UserChapter.Status))) 'ProgressRate'
                          ,'' 'Status' 
                          , CONVERT(VARCHAR(36),UserCourse.UserId) 'UserId'
                       FROM 
                           UserCourse
                       LEFT JOIN UserChapter
                           ON  UserCourse.UserId = UserChapter.UserId
                           AND UserCourse.CourseId = UserChapter.CourseId
                       LEFT JOIN MCourse
                           ON MCourse.CourseId = UserCourse.CourseId
                       WHERE UserCourse.DeletedFlg = @sysDelNo
                          AND UserCourse.UserId = @userId
                       GROUP BY 
                           UserCourse.CourseId,MCourse.CourseName, UserCourse.UserId                           
                   ";

            var myCourseList = await _context.Database.SqlQueryRaw<MyCourse>(
                @sql,
                    new SqlParameter("@sysStsComplete", ConstService.SystemCode.SYSCODE_STS_COMPLETE),
                    new SqlParameter("@sysDelNo", ConstService.SystemCode.SYSCODE_DEL_NO),
                    new SqlParameter("@userId", userId)
                ).ToListAsync();
            return myCourseList;
        }

        /// <inheritdoc/>
        public async Task<List<MyChapter>> GetUserChapterList(string userId, string courseId)
        {
            // 学習状況（各表示名）を取得
            var statusList = await this._sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_STATUS);
            string StatWaiting = CommonService.GetValueByCode(statusList, ConstService.SystemCode.SYSCODE_STS_WAITING);
            string StatStudying = CommonService.GetValueByCode(statusList, ConstService.SystemCode.SYSCODE_STS_STUDYING);
            string StatComplete = CommonService.GetValueByCode(statusList, ConstService.SystemCode.SYSCODE_STS_COMPLETE);

            // コースマスタを取得
            var mCourse = await _context.MCourse
                .Where(x => x.CourseId == Guid.Parse(courseId))
                .FirstOrDefaultAsync();
            // コース状態(公開/非公開)を取得
            var courseInvalid = ((mCourse == null) || (mCourse.DeletedFlg) ||
                               ((mCourse.PrimaryReference == ConstService.SystemCode.SYSCODE_PRI_PERIOD) &&
                                ((mCourse.BegineDateTime > DateTime.Now) || (mCourse.EndDateTime < DateTime.Now))) ||
                               ((mCourse.PrimaryReference == ConstService.SystemCode.SYSCODE_PRI_FLAG) && (!mCourse.PublicFlg)));

            // 講座一覧を取得
            List<MyChapter> myChapterList = await this._context.UserChapter
                    .Where(x => x.UserId == Guid.Parse(userId) && x.CourseId == Guid.Parse(courseId))
                    .Select(x => new MyChapter
                    {
                        OrderNo = x.MChapter!.OrderNo,
                        ChapterId = x.ChapterId.ToString(),
                        ChapterName = x.MChapter.ChapterName,
                        ContentsType = x.MChapter.ContentsType,
                        Status = x.Status,
                        StatusName = x.Status == ConstService.SystemCode.SYSCODE_STS_COMPLETE ? StatComplete :
                                         x.Status == ConstService.SystemCode.SYSCODE_STS_STUDYING ? StatStudying : StatWaiting,
                        StartDatetime = x.StartDatetime,
                        EndDatetime = x.EndDatetime,
                        DeletedFlg = (x.MChapter.DeletedFlg || courseInvalid)   // 講座非公開の場合、講座は削除扱い
                    })
                    .OrderBy(x => x.OrderNo)
                    .ToListAsync();

            return myChapterList;
        }

        /// <inheritdoc/>
        private async Task<UserChapter> SelectByUserChapterId(string userId, string chapterId)
        {
            UserChapter userChapter = new();
            try
            {
                userChapter = await this._context.UserChapter
                    .Where(x => x.UserId == Guid.Parse(userId) && x.ChapterId == Guid.Parse(chapterId))
                    .FirstOrDefaultAsync() ?? new();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return userChapter;
        }

        /// <inheritdoc/>
        public async Task<int> UpdateUserChapter(string userId, string chapterId, string type)
        {
            var result = 0;
            var userChapter = await this.SelectByUserChapterId(userId, chapterId);
            // 受講開始
            if (type == "start" && !userChapter.StartDatetime.HasValue)
            {
                try
                {
                    userChapter.StartDatetime = DateTime.Now;
                    userChapter.Status = ConstService.SystemCode.SYSCODE_STS_STUDYING;
                    userChapter.UpdatedBy = Guid.Parse(userId);

                    result = this._context.SaveChanges();
                }
                catch (Exception ex)
                {
                    result = -1;
                    CriticalError(ex);
                }

            }

            // 受講終了
            if (type == "end" && userChapter.StartDatetime.HasValue && !userChapter.EndDatetime.HasValue)
            {
                try
                {
                    userChapter.EndDatetime = DateTime.Now;
                    userChapter.Status = ConstService.SystemCode.SYSCODE_STS_COMPLETE;
                    userChapter.UpdatedBy = Guid.Parse(userId);

                    result = this._context.SaveChanges();
                }
                catch (Exception ex)
                {
                    result = -1;
                    CriticalError(ex);
                }

            }
            return result;
        }

        /// <inheritdoc/>
        public async Task<MChapter> GetPreviousNextChapter(string courseId, byte? orderNo)
        {
            MChapter mChapter = new();
            try
            {
                mChapter = await this._context.MChapter
                    .Where(x => x.CourseId == Guid.Parse(courseId) && x.OrderNo == orderNo)
                    .FirstOrDefaultAsync() ?? new();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return mChapter;
        }

        public async Task<MCourse> GetMCourse(string courseId)
        {
            MCourse myMCourse = new();
            try
            {
                myMCourse = await this._context.MCourse
                    .Where(x => x.CourseId == Guid.Parse(courseId))
                    .FirstOrDefaultAsync() ?? new();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return myMCourse;
        }

        public async Task<bool> IsShowExaminationHistory(string userId, string ChapterId)
        {
            try
            {
                // ChapterIdをGUIDに変換
                var chapterGuid = Guid.Parse(ChapterId);
                var userGuid = Guid.Parse(userId);

                // UserChapterの取得
                var userChapter = await _context.UserChapter
                    .Where(x => x.ChapterId == chapterGuid)
                    .Where(x => x.UserId == userGuid)
                    .FirstOrDefaultAsync();

                if (userChapter == null)
                {
                    return false; // UserChapterが見つからない場合
                }

                // UserScoreの取得（コメントアウトされていた部分を有効にする）
                var userScore = await _context.UserScore
                    .Where(x => x.UserChapterId == userChapter.UserChapterId)
                    .FirstOrDefaultAsync();

                // UserScoreが存在するかどうかをチェック
                return userScore != null;
            }
            catch (Exception ex)
            {
                CriticalError(ex);
                return false;
            }
        }


    }
}

