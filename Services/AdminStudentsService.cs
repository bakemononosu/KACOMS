using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class AdminStudentsService(
        ElsWebAppDbContext ctx,
        ILogger<AdminStudentsService> logger,
        IUserService svcUser,
        ICourseService svcCourse
    ) : IAdminStudentsService
    {
        private readonly ElsWebAppDbContext _context = ctx;
        private readonly ILogger<AdminStudentsService> _logger = logger;
        private void CriticalError(Exception ex) => this._logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        protected IUserService _userService = svcUser;
        protected ICourseService _courseService = svcCourse;

        /// <inheritdoc/>
        public async Task<List<MUser>> GetStudentList()
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

            return userList.Where(x => x.UserRole == "9").ToList();
        }

        /// <inheritdoc/>
        public async Task<MUser> GetUser(string UserId)
        {
            var user = await this._userService.SelectById(UserId);

            return user;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAvailableFlg(string UserId, bool availableFlg, Guid loginUserId)
        {
            var user = await this._userService.SelectById(UserId);

            user.UpdatedBy = loginUserId;
            user.AvailableFlg = availableFlg;

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
        public async Task<bool> UpdateDeleteFlg(string UserId, Guid loginUserId)
        {
            var user = await this._userService.SelectById(UserId);
            user.UpdatedBy = loginUserId;
            user.DeletedFlg = true;

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
        public async Task<int> UpdateForAdminStudent(MUser user)
        {
            var mUser = await this._userService.SelectById(user.UserId.ToString());

            int result;
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
                mUser.UpdatedBy = user.UpdatedBy;

                result = this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                CriticalError(ex);
                result = -1;
            }

            return result;
        }
    }
}

