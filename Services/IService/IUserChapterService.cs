using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IUserChapterService : IBaseEntity<UserChapter>
    {
        /// <summary>
        /// 受講者講座データをインデックで取得する
        /// </summary>
        /// <param name="userId">ユーザ識別子</param>
        /// <param name="courseId">コース識別子</param>
        /// <param name="chapterd">講座識別子</param>
        /// <returns></returns>
        public Task<UserChapter> SelectByUniqueIndex(Guid userId, Guid courseId, Guid chapterd);
    }
}
