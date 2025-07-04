using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IAnswerGroupService : IBaseEntity<AnswerGroup>
    {
        /// <summary>
        /// 問題カタログ識別子で解答グループデータを取得する
        /// </summary>
        /// <param name="questionId">問題カタログ識別子</param>
        /// <returns></returns>
        public Task<List<AnswerGroup>> SelectByQuestionId(Guid questionId);
    }
}
