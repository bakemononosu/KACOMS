using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IChapterService : IBaseEntity<MChapter>
    {
        /// <summary>
        /// コース識別子で講座情報を取得する
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <returns></returns>
        public Task<List<MChapter>> SelectByCourseId(Guid courseId);

        /// <summary>
        /// 受講者の次の講座識別子を取得する
        /// </summary>
        /// <param name="chapterId">講座識別子</param>
        /// <returns></returns>
        public Task<MChapter> GetNextChapter(Guid chapterId);

        /// <summary>
        /// 受講者の前の講座識別子を取得する
        /// </summary>
        /// <param name="chapterId">受講者講座識別子</param>
        /// <returns></returns>
        public Task<MChapter> GetPrevChapter(Guid chapterId);
    }
}
