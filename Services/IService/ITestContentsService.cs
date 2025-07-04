using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface ITestContentsService : IBaseEntity<TestContents>
    {
        /// <summary>
        /// 講座識別子から、コンテンツ情報(テスト)を取得する。
        /// </summary>
        /// <param name="chapterId">講座識別子</param>
        /// <returns></returns>
        public Task<TestContents> SelectByChapterId(Guid chapterId);
    }
}
