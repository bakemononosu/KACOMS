using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class AccountInfoService(
        ElsWebAppDbContext ctx,
        ILogger<UserService> logger,
        IUserService svcUser,
        ICourseService svcCourse
    ) : IAccountInfoService
    {
        private readonly ILogger<UserService> _logger = logger;
        private readonly ElsWebAppDbContext _context = ctx;
        protected IUserService _userService = svcUser;
        protected ICourseService _courseService = svcCourse;



        /// <inheritdoc/>
        public async Task<string> GetLoginUserName(string loginId)
        {
            var useName = await this._userService.GetUserName(loginId);

            return useName;
        }

        /// <inheritdoc/>
        public async Task<List<MUser>> GetAccountList() //全件取得
        {
            List<MUser> userList = [];
            try
            {
                userList = await this._context.MUser.ToListAsync() ?? [];
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return userList;
        }

        /// <inheritdoc/>
        public async Task<MUser> GetUser(string UserId)
        {
            var user = await this._userService.SelectById(UserId);

            return user;
        }


        /// <inheritdoc/>
        public async Task<List<MCourse>> GetCourseList(bool isPublicOnly)
        {
            return await this._courseService.GetCourseList(isPublicOnly);
        }

        /// <inheritdoc/>
        public async Task<List<MUser>> GetAdminList()
        {
            var userList = await this._userService.GetUserList();

            return userList.Where(x => x.UserRole != "9").ToList();　// UserRoleが9の物は現在いない
        }
        /// <inheritdoc/>
        public async Task<bool> UpdateAvailableFlg(string UserId, bool availableFlg, Guid mineUserId)
        {
            var user = await this._userService.SelectById(UserId);

            user.AvailableFlg = availableFlg;
            user.UpdatedBy = mineUserId;
            var result = await this._userService.Update(user);

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <inheritdoc/>
        public async Task<bool> UpdateDeleteFlg(string UserId, Guid mineUserId)
        {
            var user = await this._userService.SelectById(UserId);

            user.DeletedFlg = true;
            user.UpdatedBy = mineUserId;

            var result = await this._userService.Update(user);

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //--------------------------------------------------------------------------------------

        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        /// <inheritdoc/>
        public async Task<MUser> SelectById(string id)
        {
            MUser mUser = new();
            try
            {
                mUser = await this._context.MUser
                    .Where(x => x.UserId == Guid.Parse(id))
                    .FirstOrDefaultAsync() ?? new();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return mUser;
        }

        /// <inheritdoc/>
        public async Task<int> Update(MUser user, string id, string studentFlg = "false")
        {

            var result = 0;
            var mUser = await this.SelectById(user.UserId.ToString());

            try
            {
                if (studentFlg == "true")
                {
                    mUser.UserName = user.UserName;
                    mUser.DepartmentName = user.DepartmentName;
                }
                else
                {
                    mUser.UserName = user.UserName;
                    mUser.CompanyName = id == "0" ? user.CompanyName : mUser.CompanyName;
                    mUser.DepartmentName = user.DepartmentName;
                    mUser.Email = mUser.Email;
                    mUser.EmployeeNo = id != "9" ? user.EmployeeNo : mUser.EmployeeNo;
                    mUser.Remarks1 = id != "9" ? user.Remarks1 : mUser.Remarks1;
                    mUser.Remarks2 = id != "9" ? user.Remarks2 : mUser.Remarks2;
                    mUser.UserRole = id != "9" ? user.UserRole : mUser.UserRole;
                    mUser.AvailableFlg = id != "9" ? user.AvailableFlg : mUser.AvailableFlg;
                    mUser.TempRegisterId = id != "9" ? user.TempRegisterId : mUser.TempRegisterId;
                    mUser.DeletedFlg = id != "9" ? user.DeletedFlg : mUser.DeletedFlg;
                    mUser.UpdatedBy = id != "9" ? user.UpdatedBy : mUser.UpdatedBy;
                }

                result = this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
            }

            return result;
        }
    }
}
