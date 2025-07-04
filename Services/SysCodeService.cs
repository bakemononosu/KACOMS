using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class SysCodeService(
        ElsWebAppDbContext ctx, 
        ILogger<SysCodeService> logger) : ISysCodeService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<SysCodeService> _logger = logger;

        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        /// <inheritdoc/>
        public  Task<MSysCode> SelectById(string id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<int> Insert(MSysCode sysCode)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<int> Update(MSysCode sysCode)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<List<CodeValuePair>> GetClassCodeList(string classId)
        {
            List<CodeValuePair> pairList = []; 
            try
            {
                pairList = await this._context.MSysCode
                    .Where(x => x.ClassId == classId)
                    .Where(x => x.ClassCd != "--")
                    .OrderBy(x => x.ClassCd)
                    .Select(x => new CodeValuePair
                    {
                        Code = x.ClassCd,
                        Value = x.ClassName
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return pairList;
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
