using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class UserChapterService(
        ElsWebAppDbContext ctx,
        ILogger<UserChapterService> logger
    ) : IUserChapterService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<UserChapterService> _logger = logger;
    
        /// <inheritdoc/>
        public async Task<UserChapter> SelectById(string userChapterId)
        {
            UserChapter? uChaper = null;

            try
            {
                uChaper = await this._context.UserChapter
                    .Where(x => x.UserChapterId == Guid.Parse(userChapterId))
                    .FirstOrDefaultAsync();
            }
            catch ( Exception ex )
            {
                CommonService.CriticalError<UserChapterService>(this._logger, ex);
                throw new Exception(ex.Message);
            }
            return uChaper?? new();
        }

        /// <inheritdoc/>
        public async Task<int> Insert(UserChapter data)
        {
            int result = 0;
            try
            {
                await this._context.UserChapter.AddAsync(data);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserChapterService>(this._logger, ex);
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <inheritdoc/>
        public async Task<int> Update(UserChapter data)
        {
            int result = 0;
            try
            {
                var userChapter = await this._context.UserChapter
                    .Where(x => x.UserChapterId == data.UserChapterId)
                    .FirstOrDefaultAsync();

                if (userChapter != null)
                {
                    userChapter.UserId = data.UserId;
                    userChapter.CourseId = data.CourseId;
                    userChapter.ChapterId = data.ChapterId;
                    userChapter.Status = data.Status;
                    userChapter.StartDatetime = data.StartDatetime;
                    userChapter.EndDatetime = data.EndDatetime;
                    userChapter.UpdatedAt = data.UpdatedAt;
                    userChapter.UpdatedBy = data.UpdatedBy;
                    userChapter.CreatedAt = data.CreatedAt;
                    userChapter.CreatedBy = data.CreatedBy;

                    result = await this._context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserChapterService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<UserChapter> SelectByUniqueIndex(Guid userId, Guid courseId, Guid chapterId)
        {
            UserChapter? userChapter = null;
            try
            {
                userChapter = await this._context.UserChapter
                    .Where(x => x.UserId == userId)
                    .Where(x => x.MUser != null && !x.MUser.DeletedFlg) 
                    .Where(x => x.CourseId == courseId)
                    .Where(x => x.ChapterId == chapterId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserChapterService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return userChapter?? new();
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
