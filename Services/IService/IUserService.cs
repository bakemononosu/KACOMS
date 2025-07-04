using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IUserService : IBaseEntity<MUser>
    {
        /// <summary>
        /// ログインユーザ名の取得
        /// </summary>
        /// <param name="loginId">ログインID</param>
        /// <returns></returns>
        public Task<string> GetUserName(string loginId);

        /// <summary>
        /// ユーザリストを取得する
        /// </summary>
        /// <returns></returns>
        public Task<List<MUser>> GetUserList();

        /// <summary>
        /// ログインIDからユーザ情報を取得する
        /// </summary>
        /// <param name="loginId">ログインID</param>
        /// <param name="role">利用者区分</param>
        /// <returns></returns>
        public Task<MUser> GetUserByLoginId(string loginId, string Role = "");
    }
}
