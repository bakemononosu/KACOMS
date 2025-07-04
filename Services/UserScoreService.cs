using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class UserScoreService(
        ElsWebAppDbContext ctx,
        ILogger<UserScoreService> logger
    ) : IUserScoreService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<UserScoreService> _logger = logger;

        /// <inheritdoc/>
        public async Task<UserScore> SelectById(string id)
        {
            UserScore? userScore = null;
            try
            {
                userScore = await this._context.UserScore
                    .Where(x => x.UserScoreId == Guid.Parse(id))
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserScoreService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return userScore?? new();
        }

        /// <inheritdoc/>
        public async Task<int> Insert(UserScore data)
        {
            int result;
            try
            {
                await this._context.UserScore.AddAsync(data);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserScoreService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> Update(UserScore data)
        {
            int result;
            try
            {
                var userScore = await this._context.UserScore
                    .Where(x => x.UserScoreId == data.UserScoreId)
                    .FirstOrDefaultAsync();
                if (userScore != null)
                {
                    userScore.UserChapterId = data.UserChapterId;
                    userScore.NthTime = data.NthTime;
                    userScore.QuestionId = data.QuestionId;
                    userScore.AnswerId = data.AnswerId;
                    userScore.DisplayOrder = data.DisplayOrder;
                    userScore.AnswerValue = data.AnswerValue;
                    userScore.Result = data.Result;
                    userScore.UpdatedBy = data.UpdatedBy;
                    userScore.CreatedBy = data.CreatedBy;

                    result = await this._context.SaveChangesAsync();
                }

                await this._context.UserScore.AddAsync(data);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserScoreService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<UserScore>> GetUserScoreList(Guid userChapterId, int times, Guid questionId)
        {
            List<UserScore>? scoreList = null; 
            try
            {
                scoreList = await this._context.UserScore
                    .Where(x => x.UserChapterId == userChapterId)
                    .Where(x => x.NthTime == times)
                    .Where(x => x.QuestionId == questionId)
                    .Where(x => x.AnswerGroup != null)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new UserScore
                    {
                        UserScoreId = x.UserScoreId,
                        UserChapterId = x.UserChapterId,
                        NthTime = x.NthTime,
                        QuestionId = x.QuestionId,
                        AnswerId = x.AnswerId,
                        AnswerGroup = x.AnswerGroup,
                        DisplayOrder = x.DisplayOrder,
                        AnswerValue = x.AnswerValue,
                        Result = x.Result,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserScoreService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return scoreList ?? [];
        }

        /// <inheritdoc/>
        public async Task<int> UpdateFromUserScoreList(Dictionary<Guid, List<UserScore>> scoreListArray)
        {
            var efCount = 0;
            try
            {
                foreach (var key in scoreListArray.Keys)
                {
                    var src = scoreListArray[key];

                    foreach (var from in scoreListArray[key])
                    {
                        var to = await this._context.UserScore
                            .Where(x => x.UserScoreId == from.UserScoreId)
                            .FirstOrDefaultAsync();

                        if (to != null)
                        {
                            to.AnswerValue = from.AnswerValue;
                            to.Result = from.Result;
                            to.UpdatedBy = from.UpdatedBy;
                        }
                    }
                }

                efCount = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<UserScoreService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return efCount;
        } 

        /// <inheritdoc/>
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
