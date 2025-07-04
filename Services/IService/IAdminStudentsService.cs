using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IAdminStudentsService
    {
        /// <summary>
        /// 受講者の一覧を取得する
        /// </summary>
        /// <returns></returns>
        public Task<List<MUser>> GetStudentList();

        /// <summary>
        /// ユーザを取得する
        /// </summary>
        /// <param name="UserId">ユーザ識別子</param>
        /// <returns></returns>
        public Task<MUser> GetUser(string UserId);

        /// <summary>
        /// ユーザの受講可否フラグを更新する
        /// </summary>
        /// <param name="UserId">ユーザ識別子</param>
        /// <param name="availableFlg">受講可否フラグ</param>
        /// <param name="loginUserId">ログインユーザのユーザ識別子</param>
        /// <returns></returns>
        public Task<bool> UpdateAvailableFlg(string UserId, bool availableFlg, Guid loginUserId);

        /// <summary>
        /// ユーザの削除フラグを更新する
        /// </summary>
        /// <param name="UserId">ユーザ識別子</param>
        /// <param name="loginUserId">ログインユーザのユーザ識別子</param>
        /// <returns></returns>
        public Task<bool> UpdateDeleteFlg(string UserId, Guid loginUserId);

        /// <summary>
        /// 受講者管理画面用ユーザ更新
        /// </summary>
        /// <param name="user">ユーザマスタモデル</param>
        /// <returns></returns>
        public Task<int> UpdateForAdminStudent(MUser user);
    }
}

