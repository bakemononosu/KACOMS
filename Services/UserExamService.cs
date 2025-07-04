using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class UserExamService(
        ElsWebAppDbContext ctx,
        ILogger<UserExamService> logger     
    ): IUserExamService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<UserExamService> _logger = logger;

        /// <inheritdoc/>
        public async Task<UserExam> SelectById(string userExamId)
        {
            UserExam? userExam = null;

            try
            {
                userExam = await this._context.UserExam
                    .Where(x => x.UserExamId == Guid.Parse(userExamId))
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserExamService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return userExam?? new();
        }

        /// <inheritdoc/>        /// <inheritdoc/>
        public async Task<int> Insert(UserExam userExam)
        {
            int result;
            try
            {
                await this._context.UserExam.AddAsync(userExam);
                result = await this._context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserExamService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> Update(UserExam data)
        {
            int result;
            try
            {
                var userExam = await this._context.UserExam
                    .Where(x => x.UserExamId == data.UserExamId)
                    .FirstOrDefaultAsync();

                if (userExam != null)
                {
                    userExam.UserChapterId = data.UserChapterId;
                    userExam.NthTime = data.NthTime;
                    userExam.DisplayOrder = data.DisplayOrder;
                    userExam.QuestionId = data.QuestionId;
                    userExam.UpdatedAt = data.UpdatedAt;
                    userExam.UpdatedBy = data.UpdatedBy;
                    userExam.CreatedAt = data.CreatedAt;
                    userExam.CreatedBy = data.CreatedBy;
                }
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserExamService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<UserExam>> GetMaxTimeExamInfo(Guid userChapterId)
        {
            List<UserExam>? maxTimeExam = null;
            var userExam = await this._context.UserExam
                .Where(x => x.UserChapterId == userChapterId)
                .OrderBy(x => x.NthTime)
                .ToListAsync();

            var maxTime = userExam
                .OrderByDescending(x => x.NthTime)
                .Take(1)
                .Select(x => x.NthTime)
                .FirstOrDefault();

            if (maxTime != 0)
            {
                maxTimeExam = userExam.Where(x => x.NthTime == maxTime).ToList();
            }

            return maxTimeExam?? [];
        }

        /// <inheritdoc/>
        public async Task<List<UserExam>> GetNthTimeExamInfo(Guid userChapterId, int times)
        {
            List<UserExam>? nthTimeExam = null;
            try
            {
                nthTimeExam = await this._context.UserExam
                    .Where(x => x.UserChapterId == userChapterId)
                    .Where(x => x.NthTime == times)
                    .Where(x => x.QuestionCatalog != null)      // 過去問の取得を考慮し、削除データも含める
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new UserExam
                    {
                        UserExamId = x.UserExamId,
                        UserChapterId = x.UserChapterId,
                        QuestionId = x.QuestionId,
                        QuestionCatalog = x.QuestionCatalog,
                        NthTime = x.NthTime,
                        DisplayOrder = x.DisplayOrder,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserExamService>(this._logger, ex);
                throw new Exception(ex.Message);
            }
            return nthTimeExam ?? [];
        }

        public async Task<List<UserExam>> GetAllTimesExamInfo(Guid userChapterId)
        {
            List<UserExam>? nthTimeExam = null;
            try
            {
                nthTimeExam = await this._context.UserExam
                    .Where(x => x.UserChapterId == userChapterId)
                    .Where(x => x.QuestionCatalog != null)      // 過去問の取得を考慮し、削除データは含める
                    .OrderBy(x => x.NthTime)
                    .ThenBy(x => x.DisplayOrder)
                    .Select(x => new UserExam
                    {
                        UserExamId = x.UserExamId,
                        UserChapterId = x.UserChapterId,
                        QuestionId = x.QuestionId,
                        QuestionCatalog = x.QuestionCatalog,
                        NthTime = x.NthTime,
                        DisplayOrder = x.DisplayOrder,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserExamService>(this._logger, ex);
                throw new Exception(ex.Message);
            }
            return nthTimeExam ?? [];
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
