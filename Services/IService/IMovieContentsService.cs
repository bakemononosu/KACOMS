using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IMovieContentsService : IBaseEntity<MovieContents> 
    {
        /// <summary>
        /// 講座識別子から、コンテンツ情報(動画)を取得する
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns></returns>
        public Task<MovieContents> SelectByChapterId(Guid chapterId);
    }
}
