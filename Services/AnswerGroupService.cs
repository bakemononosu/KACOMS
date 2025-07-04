using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class AnswerGroupService(ElsWebAppDbContext ctx, ILogger<AnswerGroupService> logger) : IAnswerGroupService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<AnswerGroupService> _logger = logger;

        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        /// <inheritdoc/>
        public async Task<AnswerGroup> SelectById(string id)
        {
            AnswerGroup answer = new();

            try
            {
                answer = await this._context.AnswerGroup
                    .Where(x => x.AnswerId == Guid.Parse(id))
                    .FirstOrDefaultAsync()?? new();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
                throw new Exception(ex.Message);
            }

            return answer;
        }

        /// <inheritdoc/>
        public async Task<int> Insert(AnswerGroup data)
        {
            var result = 0;

            try
            {
                this._context.AnswerGroup.Add(data);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        public async Task<int> Update(AnswerGroup data)
        {
            var result = 0;
            var answer = await this.SelectById(data.AnswerId.ToString());
        
            try
            {
                answer.QuestionId = data.QuestionId;
                answer.AnswerText = data.AnswerText;
                answer.AnswerImageName = data.AnswerImageName;
                answer.AnswerImageData = data.AnswerImageData;
                answer.ExplanationText = data.ExplanationText;
                answer.ExplanationImageName = data.ExplanationImageName;
                answer.ExplanationImageData = data.ExplanationImageData;
                answer.ErrataFlg = data.ErrataFlg;
                answer.DeletedFlg = data.DeletedFlg;

                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<AnswerGroup>> SelectByQuestionId(Guid questionId)
        {
            List<AnswerGroup>? alist = null;

            try
            {
                alist = await this._context.AnswerGroup
                    .Where(x => x.QuestionId == questionId)
                    .Where(x => !x.DeletedFlg)
                    .Where(x => x.Question != null && !x.Question.DeletedFlg)
                    .OrderBy(x => x.OrderNo)
                    .Select(x => new AnswerGroup
                    {
                        Question = x.Question,
                        QuestionId = x.QuestionId,
                        AnswerId = x.AnswerId,
                        AnswerText = x.AnswerText,
                        AnswerImageName = x.AnswerImageName,
                        AnswerImageData = x.AnswerImageData,
                        ExplanationText = x.ExplanationText,
                        ExplanationImageData = x.ExplanationImageData,
                        ExplanationImageName = x.ExplanationImageName,
                        ErrataFlg = x.ErrataFlg,
                        DeletedFlg = x.DeletedFlg,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
                throw new Exception(ex.Message);
            }

            return alist?? [];
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
