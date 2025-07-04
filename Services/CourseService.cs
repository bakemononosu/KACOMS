using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    /// <summary>
    /// コース情報サービス
    /// </summary>
    public class CourseService(ElsWebAppDbContext ctx, ILogger<CourseService> logger) : ICourseService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<CourseService> _logger = logger;

        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        /// <inheritdoc/>
        public async Task<MCourse> SelectById(string id)
        {
            MCourse course = new ();

            try
            {
                course = await this._context.MCourse
                    .Where(x => x.CourseId == Guid.Parse(id))
                    .FirstOrDefaultAsync()?? new ();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return course;
        }

        /// <inheritdoc/>
        public async Task<int> Insert(MCourse course)
        {
            var result = 0;
            try
            {
                await this._context.MCourse.AddAsync(course);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> Update(MCourse course)
        {
            var result = 0;
            var mCourse = await this.SelectById(course.CourseId.ToString());

            if (mCourse.CourseId != Guid.Empty)
            {
                mCourse.CourseName = course.CourseName;
                mCourse.CourseExplaination = course.CourseExplaination;
                mCourse.BegineDateTime = course.BegineDateTime;
                mCourse.EndDateTime = course.EndDateTime;
                mCourse.LearningTime = course.LearningTime;
                mCourse.PublicFlg = course.PublicFlg;
                mCourse.DeletedFlg = course.DeletedFlg;

                try
                {
                    result = await this._context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    CriticalError(ex);
                }
            }

            return result;
        }
    
        /// <inheritdoc/>
        public async Task<List<MCourse>> GetCourseList(bool isOnlyPublic)
        {
            List<MCourse> courseList = [];
            try
            {
                var now = DateTime.Now;
                var exp = this._context.MCourse
                    .Where(x => !x.DeletedFlg);

                if (isOnlyPublic)
                {
                    exp = exp.Where(x => ((x.PrimaryReference == ConstService.SystemCode.SYSCODE_PRI_PERIOD &&
                        (x.BegineDateTime <= now && x.EndDateTime >= now)) ||
                        (x.PrimaryReference == ConstService.SystemCode.SYSCODE_PRI_FLAG && x.PublicFlg == true)));
                }

                courseList = await exp.ToListAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return courseList;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckCourseIsAvailable(Guid courseId)
        {
            var date = DateTime.Now.Date;
            var any = false;
            try
            {
                any = await this._context.MCourse
                    .Where(x => x.CourseId == courseId)
                    .Where(x => !x.DeletedFlg)
                    .Where(x => (x.PrimaryReference == ConstService.SystemCode.SYSCODE_PRI_FLAG && x.PublicFlg) ||
                                (x.PrimaryReference == ConstService.SystemCode.SYSCODE_PRI_PERIOD && (x.BegineDateTime.Date <= date && x.EndDateTime >= date)))
                    .AnyAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return any;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 必要な場合以下に記載
            }
        }
    }
}
