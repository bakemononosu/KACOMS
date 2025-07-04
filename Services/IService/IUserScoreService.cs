using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IUserScoreService : IBaseEntity<UserScore>
    {
        /// <summary>
        /// 受講者-講座識別子、実施回数、問題カタログ識別子に該当する、
        /// 受講者採点データのリストを取得する
        /// </summary>
        /// <param name="userChapterId">受講者-講座識別子</param>
        /// <param name="times">実施回数</param>
        /// <param name="questionId">問題カタログ識別子</param>
        /// <returns></returns>
        public Task<List<UserScore>> GetUserScoreList(Guid userChapterId, int times, Guid questionId);

        /// <summary>
        /// 受講者採点データリストの内容で受講者採点データを更新する
        /// </summary>
        /// <param name="scoreListArray">受講者採点データリスト配列</param>
        /// <returns></returns>
        public Task<int> UpdateFromUserScoreList(Dictionary<Guid, List<UserScore>> scoreListArray);
    }
}
