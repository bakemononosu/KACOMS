using ElsWebApp.Models;

namespace ElsWebApp.Services.IService
{
    public interface IAdminTestStatusService
    {
        /// <summary>
        /// テスト結果を取得する
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <param name="userId">ユーザ識別子</param>
        /// <returns></returns>
        public List<CourseStudentTestStatus> GetTestStatus(string courseId, string userId);

        /// <summary>
        /// 講座マスタから表示する得点1～nを判断する配列を取得する
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <returns></returns>
        public Task<List<string>> GetScoresHeader(string courseId);

        /// <summary>
        /// テストコンテンツのあるデータに絞り込む
        /// </summary>
        /// <param name="courseStudentTestList">テスト結果リスト</param>
        /// <returns></returns>
        public Task<List<CourseStudentTestStatus>> FilterTestStatusData(List<CourseStudentTestStatus> courseStudentTestList);
    }
}
