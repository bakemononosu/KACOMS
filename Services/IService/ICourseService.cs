using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface ICourseService : IBaseEntity<MCourse>
    {
        public Task<List<MCourse>> GetCourseList(bool isOnlyPublic = false);
        /// <summary>
        /// 指定されたコースが公開中かどうかチェックする。
        /// </summary>
        /// <param name="courceId">コースID</param>
        /// <returns>
        ///     true:公開中
        ///     false:非公開
        /// </returns>
        public Task<bool> CheckCourseIsAvailable(Guid courceId);
    }
}
