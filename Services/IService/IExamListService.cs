using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IExamListService
    {
        /// <summary>
        /// コンテンツIDから問題カタログIDリストを取得する
        /// </summary>
        /// <param name="contentsId">コンテンツId</param>
        /// <returns></returns>
        public Task<List<ExamList>> SelectByContentsId(Guid contentsId);

        /// <summary>
        /// コンテンツIDに紐づく全ての出題リストを削除する
        /// </summary>
        /// <param name="contentsId">コンテンツID</param>
        /// <returns></returns>
        public Task<bool> DeleteByContentsId(Guid contentsId);

        /// <summary>
        /// 問題カタログ識別子のリストから出題リストを作成する。
        /// </summary>
        /// <param name="userId">ユーザ識別子</param>
        /// <param name="contentsId">コンテンツID</param>
        /// <param name="questionIdList">問題カタログ識別子リスト</param>
        /// <returns></returns>
        public Task<bool> InsertFromQuestionIdList(Guid userId, Guid contentsId, List<Guid> questionIdList);
    }
}
