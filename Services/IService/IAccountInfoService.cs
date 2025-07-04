using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IAccountInfoService
    {


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
        /// MUserの情報を取得、初期表示時に起動
        /// UserRole=9は弾く
        /// </summary>
        /// 
        /// <returns></returns>      
        public Task<List<MUser>> GetAccountList();

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
        public Task<bool> UpdateAvailableFlg(string UserId, bool availableFlg, Guid mineUserId);

        /// <summary>
        /// ユーザの削除フラグを更新する
        /// </summary>
        public Task<bool> UpdateDeleteFlg(string UserId, Guid mineUserId);
        //--------------------------------------------------------------------------------------
        /// <summary>
        /// DB更新機能
        /// result = 1[正常終了、更新項目あり]
        /// result = 0[正常終了、更新項目はなし、0件更新]
        /// result = -1[異常終了]
        /// </summary>
        public Task<int> Update(MUser user, string id, string studentFlg = "false");
        public Task<MUser> SelectById(string id);
    }
}
