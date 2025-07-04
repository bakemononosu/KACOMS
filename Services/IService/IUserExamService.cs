using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IUserExamService: IBaseEntity<UserExam>
    {
        /// <summary>
        /// 受講者-講座識別子から、実施回数が最大の受講者出題情報を取得する
        /// </summary>
        /// <param name="userChapterId">受講者講座データ</param>
        /// <returns></returns>
        public Task<List<UserExam>> GetMaxTimeExamInfo(Guid userChapterId);

        /// <summary>
        /// 受講者-講座識別子から、実施回数がN回目の受講者出題情報を取得する
        /// </summary>
        /// <param name="userChapterId"></param>
        /// <param name="times">実施回数</param>
        /// <returns></returns>
        public Task<List<UserExam>> GetNthTimeExamInfo(Guid userChapterId, int times);

        /// <summary>
        /// 受講者-講座識別子から、全ての受講者出題情報を取得する
        /// </summary>
        /// <param name="userChapterId"></param>
        /// <returns></returns>
        public Task<List<UserExam>> GetAllTimesExamInfo(Guid userChapterId);
    }
}
