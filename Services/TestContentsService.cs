using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ElsWebApp.Services
{
    public class TestContentsService(ElsWebAppDbContext ctx, ILogger<TestContentsService> logger): ITestContentsService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<TestContentsService> _logger = logger;

        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        /// <inheritdoc/>
        public async Task<TestContents> SelectById(string id)
        {
            TestContents mTest = new();
            try
            {
                mTest = await this._context.TestContents
                    .Where(x => x.ContentsId == Guid.Parse(id))
                    .FirstOrDefaultAsync() ?? new();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return mTest;
        }

        /// <inheritdoc/>
        public async Task<int> Insert(TestContents data)
        {
            int result = 0;
            try
            {
                this._context.TestContents.Add(data);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }

        public async Task<int> Update(TestContents data)
        {
            var result = 0;
            var mTest = await this.SelectById(data.ContentsId.ToString());

            try
            {
                mTest.ChapterId = data.ChapterId;
                mTest.ContentsName = data.ContentsName;
                mTest.Questions = data.Questions;
                mTest.LimitTime = data.LimitTime;
                mTest.DeletedFlg = data.DeletedFlg;
                mTest.UpdatedBy = data.UpdatedBy;

                result = this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }

        public async Task<TestContents> SelectByChapterId(Guid chapterId)
        {
            TestContents? mTest = null;
            try
            {
                mTest = await this._context.TestContents
                    .Where(x => x.ChapterId == chapterId)
                    .Where(x => x.Chapter != null && !x.Chapter.DeletedFlg)
                    .Where(x => !x.DeletedFlg)
                    .Select(x => new TestContents
                    {
                        ContentsId = x.ContentsId,
                        ChapterId = x.ChapterId,
                        Chapter = x.Chapter,
                        ContentsName = x.ContentsName,
                        Questions = x.Questions,
                        LimitTime = x.LimitTime,
                        DeletedFlg = x.DeletedFlg,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return mTest?? new();
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
