using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace ElsWebApp.Services
{
    public class ElsService(
        IUserService svcUser,
        ICourseService svcCourse
    ) : IElsService
    {
        protected IUserService _userService = svcUser;
        protected ICourseService _courseService = svcCourse;

        public void StartTrans()
        {

        }

        public void EndTrans()
        {

        }

        /// <inheritdoc/>
        public async Task<string> GetLoginUserName(string loginId)
        {
            var useName = await this._userService.GetUserName(loginId);

            return useName;
        }

        /// <inheritdoc/>
        public async Task<List<MUser>> GetStudentList()
        {
            var userList = await this._userService.GetUserList();

            return userList.Where(x => x.UserRole == "9").ToList();
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

            return userList.Where(x => x.UserRole != "9").ToList();
        }
        /// <inheritdoc/>
        public async Task<bool> UpdateAvailableFlg(string UserId, bool availableFlg)
        {
            var user = await this._userService.SelectById(UserId);

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
        public async Task<bool> UpdateDeleteFlg(string UserId)
        {
            var user = await this._userService.SelectById(UserId);

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
    }
}
