using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IElsService
    {
        public void StartTrans();
        public void EndTrans();

        /// <summary>
        /// ログインユーザ名の取得
        /// </summary>
        /// <param name="loginId">ログインID</param>
        /// <returns></returns>
        public Task<string> GetLoginUserName(string loginId);

        /// <summary>
        /// 登録済コース情報を取得する
        /// </summary>
        /// <param name="isOnlyPublic">
        ///     true:公開中のコースのみ取得
        ///     false:非公開も含め取得
        /// </param>
        /// <returns></returns>
        public Task<List<MCourse>> GetCourseList(bool isOnlyPublic);

        /// <summary>
        /// 受講者の一覧を取得する
        /// </summary>
        /// <returns></returns>
        public Task<List<MUser>> GetStudentList();

        /// <summary>
        /// 管理者の一覧を取得する
        /// </summary>
        /// <returns></returns>
        public Task<List<MUser>> GetAdminList();

        /// <summary>
        /// ユーザを取得する
        /// </summary>
        /// <returns></returns>
        public Task<MUser> GetUser(string UserId);

        /// <summary>
        /// ユーザの受講可否フラグを更新する
        /// </summary>
        public Task<bool> UpdateAvailableFlg(string UserId, bool availableFlg);

        /// <summary>
        /// ユーザの削除フラグを更新する
        /// </summary>
        public Task<bool> UpdateDeleteFlg(string UserId);
    }
}
