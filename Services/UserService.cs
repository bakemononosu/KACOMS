using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace ElsWebApp.Services
{
    /// <summary>
    /// ユーザ情報サービス
    /// </summary>
    public class UserService(ElsWebAppDbContext ctx, ILogger<UserService> logger) : IUserService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<UserService> _logger = logger;

        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        /// <inheritdoc/>
        public async Task<MUser> SelectById(string id)
        {
            MUser mUser = new ();
            try
            {
                mUser = await this._context.MUser
                    .Where(x => x.UserId == Guid.Parse(id))
                    .FirstOrDefaultAsync()?? new ();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return mUser;
        }

        /// <inheritdoc/>
        public async Task<int> Insert(MUser user)
        {
            int result = 0;
            try
            {
                this._context.MUser.Add(user);
                result = await this._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> Update(MUser user)
        {
            var result = 0;
            var mUser = await this.SelectById(user.UserId.ToString());
        
            try
            {
                mUser.UserName = user.UserName;
                mUser.CompanyName = user.CompanyName;
                mUser.DepartmentName = user.DepartmentName;
                mUser.Email = user.Email;
                mUser.EmployeeNo = user.EmployeeNo;
                mUser.Remarks1 = user.Remarks1;
                mUser.Remarks2 = user.Remarks2;
                mUser.UserRole = user.UserRole;
                mUser.AvailableFlg = user.AvailableFlg;
                mUser.TempRegisterId = user.TempRegisterId;
                mUser.DeletedFlg = user.DeletedFlg;

                result = this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<string> GetUserName(string loginId)
        {
            var userName = string.Empty;
            try
            {
                userName = await this._context.MUser
                    .Where(x => x.LoginId == loginId)
                    .Select(x => x.UserName)
                    .FirstOrDefaultAsync() ?? "";
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return userName;
        }

        /// <inheritdoc/>
        public async Task<List<MUser>> GetUserList()
        {
            List<MUser> userList = [];
            try
            {
                userList = await this._context.MUser
                    .Where(x => !x.DeletedFlg)
                    .ToListAsync() ?? [];
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return userList;
        }

        /// <inheritdoc/>
        public async Task<MUser> GetUserByLoginId(string loginId, string role = "")
        {
            MUser? user = null;
            try
            {
                var query = this._context.MUser
                    .Where(x => !x.DeletedFlg)
                    .Where(x => x.LoginId == loginId);
                if (role != string.Empty)
                {
                    if (role == ConstService.SystemCode.SYSCODE_USR_USERS)
                    {
                        // 受講者を検索
                        query = query.Where(x => x.UserRole == ConstService.SystemCode.SYSCODE_USR_USERS);
                    }
                    else
                    {
                        // 受講者以外(システム管理者、法人代表管理者)を検索
                        query = query.Where(x => x.UserRole != ConstService.SystemCode.SYSCODE_USR_USERS);
                    }
                }
                user = await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return user?? new MUser();
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
