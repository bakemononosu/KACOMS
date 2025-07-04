using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using Microsoft.AspNetCore.Mvc;

namespace ElsWebApp.Services.IService
{ 
    public interface IAdminCourseService
    {
        /// <summary>
        /// 登録済コース情報を取得する
        /// </summary>
        /// <param name="isOnlyPublic">
        ///     true:公開中のコースのみ取得
        ///     false:非公開も含め取得
        /// </param>
        /// <returns></returns>
        public Task<List<AdminCourse>> GetCourseList(bool isOnlyPublic);

        /// <summary>
        /// コース識別子に紐づく講座情報を取得する
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <returns></returns>
        public Task<List<AdminCourseChapter>> GetCourseChapterList(Guid courseId);

        /// <summary>
        /// 講座識別子から講座情報を取得する
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <param name="chapterId">講座識別子</param>
        /// <returns></returns>
        public Task<ShowChapterViewModel> GetChapterInfo(string courseId, string chapterId);

        /// <summary>
        /// 大・中・小分類コードで、問題カタログを検索する。
        /// </summary>
        /// <param name="majorCd">大分類コード</param>
        /// <param name="middleCd">中分類コード</param>
        /// <param name="minorCd">小分類コード</param>
        /// <returns></returns>
        public Task<List<QuestionData>> SearchQuestionCatalog(string majorCd, string middleCd, string minorCd);

        /// <summary>
        /// コース講座(チャプター)を更新する。
        /// </summary>
        /// <param name="userId">ユーザ識別子(ログインユーザ)</param>
        /// <param name="model">チャプター情報モデル</param>
        /// <returns></returns>
        public Task<bool> RegisterChapterInfo(Guid userId, ShowChapterViewModel model);

        /// <summary>
        /// 問題カタログ識別子から該当グループを取得する
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public Task<QAnswerData> SearchAnswerGroup(Guid questionId);

        /// <summary>
        /// コースマスタを更新する。
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <param name="primaryReference">優先参照先区分</param>
        /// <param name="publicFlg">公開フラグ</param>
        /// <returns></returns>
        public Task<bool> HandlRegisterCourseInfo(string courseId, string primaryReference, bool publicFlg);

        /// <summary>
        /// コースマスタを削除（削除フラグ更新）する。
        /// </summary>
        public Task<bool> UpdateDeleteFlg(string courseId, Guid userId);

        /// <summary>
        /// コース識別子に紐づくコース情報を取得する
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <returns></returns>
        public Task<MCourse> GetCourseById(string courseId);

        /// <summary>
        /// 新規コースを登録する
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public Task<bool> InsertCourse(MCourse course);

        /// <summary>
        /// コースの個別更新を実行する
        /// </summary>
        /// <param name="course">コース情報</param>
        /// <returns></returns>
        public Task<bool> UpdateCourse(MCourse course);

        /// <summary>
        /// 講座と講座に紐づくデータを削除する
        /// </summary>
        /// <param name="chapterId">講座識別子</param>
        /// <param name="updatedBy">処理ユーザ識別子</param>
        /// <returns></returns>
        public Task<bool> DeleteChapterById(string chapterId, Guid updatedBy);

        /// <summary>
        /// 講座の学習順序を更新する
        /// </summary>
        /// <param name="fixedTableCourseChapterData">画面上のテーブルのJSON</param>
        /// <param name="fixedTableCourseChapterData.ChapterId">講座識別子</param>
        /// <param name="fixedTableCourseChapterData.OrderNo">学習順序</param>
        /// <param name="updatedBy">処理ユーザ識別子</param>
        /// <returns></returns>
        public Task<bool> UpdateChapterOrderNo(string fixedTableCourseChapterData, Guid updatedBy);

        /// <summary>
        /// 受講者コースデータの存在有無
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <returns></returns>
        public Task<bool> IsExistUserCourse(Guid courseId);

    }
}
