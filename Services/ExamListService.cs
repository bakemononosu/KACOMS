using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class ExamListService(
        ElsWebAppDbContext ctx,
        ILogger<ExamListService> logger
    ) : IExamListService
    {
        private readonly ElsWebAppDbContext _ctx = ctx;
        private readonly ILogger<ExamListService> _logger = logger;

        /// <inheritdoc/>
        public async Task<ExamList> SelectById(Guid examId)
        {
            ExamList? examList = null;
            try
            {
                examList = await this._ctx.ExamList
                    .Where(x => x.ExamId == examId)
                    .Where(x => x.QuestionCatalog != null && !x.QuestionCatalog.DeletedFlg) 
                    .Select(x => new ExamList
                    {
                        ExamId = x.ExamId,
                        QuestionId = x.QuestionId,
                        QuestionCatalog = x.QuestionCatalog,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ExamListService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return examList?? new();
        }

        /// <inheritdoc/>
        public async Task<int> Insert(ExamList examList)
        {
            var result = 0;
            try
            {
                this._ctx.ExamList.Add(examList);
                result = await this._ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ExamListService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> Update(ExamList examList)
        {
            var result = 0;
            try
            {
                var data = await this._ctx.ExamList
                    .Where(x => x.ExamId == examList.ExamId)
                    .FirstOrDefaultAsync();

                if (data == null)
                {
                    return 0;
                }

                data.ContentsId = examList.ContentsId;
                data.QuestionId = examList.QuestionId;
                result = await this._ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ExamListService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> Delete(ExamList examList)
        {
            var result = 0;
            try
            {
                this._ctx.ExamList.Remove(examList);
                result = await this._ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ExamListService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<ExamList>> SelectByContentsId(Guid contentsId)
        {
            List<ExamList>? examList = null;
            try
            {
                examList = await this._ctx.ExamList
                    .Where(x => x.ContentsId == contentsId)
                    .Where(x => x.QuestionCatalog != null && !x.QuestionCatalog.DeletedFlg)
                    .Select(x => new ExamList
                    {
                        ExamId = x.ExamId,
                        ContentsId = x.ContentsId,
                        QuestionId = x.QuestionId,
                        QuestionCatalog = x.QuestionCatalog,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ExamListService>(this._logger, ex);
                throw new Exception(ex.Message);
            }

            return examList?? [];    
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteByContentsId(Guid contentsId)
        {
            var result = true;

            try
            {
                var target = await this.SelectByContentsId(contentsId);

                foreach (var rec in target)
                {
                    this._ctx.ExamList.Remove(rec);
                }

                await this._ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ExamListService>(this._logger, ex);
                result = false;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> InsertFromQuestionIdList(Guid userId, Guid contentsId, List<Guid> questionIdList)
        {
            var result = true;
            try
            {
                foreach (var id in questionIdList)
                {
                    var examList = new ExamList
                    {
                        ContentsId = contentsId,
                        QuestionId = id,
                        CreatedBy = userId
                    };

                    await this._ctx.ExamList.AddAsync(examList);
                }

                await this._ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CommonService.CriticalError<ExamListService>(this._logger, ex);
                result = false;
            }

            return result;
        }
    }
}
