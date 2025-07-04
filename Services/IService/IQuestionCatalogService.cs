using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IQuestionCatalogService : IBaseEntity<QuestionCatalog>
    {
        public Task<List<QuestionCatalog>> GetAllQuestionList();
        public Task<List<CodeValuePair>> GetClassCodeListForViewModel(string classId);

        //public Task<(int result, QuestionCatalog data)> InsertQuestionCatalog(QuestionCatalog data);
            public Task<int> InsertAnswerGroup(AnswerGroup data);

        public Task<(int result, QuestionCatalog data)> UpdateQuestionCatalog(QuestionCatalog data);

        //public Task<int> UpdateAnswerGroup(AnswerGroup data);

        public Task<int> InsertQuestionCatalogAndAnswerGroup(AdminQuestionsViewModel data);

        public Task<AnswerGroup> SelectByIdForAnswerGroupCheckAnswerId(string id);

        //問題に対する解答を抽出する機能（削除用)
        public Task<AnswerGroup> SelectByIdForAnswerGroupQuestionDetailDelFlg(string id);

        public Task<bool> UpdateQuestionDetailDeleteFlg(string AnswerId);

        /// <summary>
        /// ユーザの削除フラグを更新する
        /// </summary>
        public Task<bool> UpdateDeleteFlg(string UserId, Guid mineUserId);

        public Task<List<AnswerGroup>> SelectByIdForAnswerGroup(string id);

        /// <summary>
        /// 大・中・小分類コードで問題カタログを検索する。
        /// </summary>
        /// <param name="majorCd">大分類コード</param>
        /// <param name="middleCd">中分類コード</param>
        /// <param name="minorCd">小分類コード</param>
        /// <returns></returns>
        public Task<List<QuestionCatalog>> SelectByClassCd(string majorCd, string middleCd, string minorCd);

        /// <summary>
        /// その問題が講座を経由してユーザに受講されているかどうかを確認する
        /// 戻り値はbool
        /// 1=受講しているユーザ有り
        /// 0=受講しているユーザ無し
        /// </summary>
        /// <returns></returns>
        public Task<List<CustomQuestionCatalog>> GetQuestionListInUsedFlg(string questionId = "1", string v = null); //全権取得



    }
}
