using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IAdminTaskStatusService
    {
        /// <summary>
        /// 全体または個別の進捗情報を取得する
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <param name="isPersonalCourses">個別取得フラグ</param>
        /// <returns></returns>
        public List<Models.TaskStatus> GetTaskStatus(string courceId, bool isPersonalCourses);

        /// <summary>
        /// 指定のコースに属する講座の学習状況を取得する
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <returns></returns>
        public List<Models.TaskStatus> GetChapterStatus(string courceId);

        /// <summary>
        /// 進捗状況のCSVデータを作成する
        /// </summary>
        /// <param name="corpName">法人名</param>
        /// <param name="userName">受講者名</param>
        /// <param name="courseId">コース(講座)ID</param>
        /// <returns></returns>
        public List<DownloadCsvData> GetDownloadCsvData(string corpName, string userName, string courseId);

    }
}


