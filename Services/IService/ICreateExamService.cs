using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface ICreateExamService
    {
        /// <summary>
        /// コンテンツIDから、問題カタログ、解答グループを取得する
        /// </summary>
        /// <returns></returns>
        public Task<ShowTestContentsViewModel> CreateStudentExamination(string userId, string courseId, string chapterId);

        /// <summary>
        /// コース識別子のコースが公開されているか確認する
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public Task<bool> CheckCourseValid(string courseId);

        /// <summary>
        /// 受講者の解答を採点する
        /// </summary>
        /// <param name="userId">ユーザ識別子(ログインユーザ)</param>
        /// <param name="model">問題・解答ビューモデル</param>
        /// <returns></returns>
        public Task<bool> GradeStudentExamination(Guid userId, ShowTestContentsViewModel model);

        /// <summary>
        /// 受講者-講座識別子から、受講者講座データを種痘する
        /// </summary>
        /// <param name="userChapterId"></param>
        /// <returns></returns>
        public Task<UserChapter> GetUserChapterData(Guid userChapterId);

        /// <summary>
        /// N回目の問題、及び採点結果を取得する
        /// </summary>
        /// <param name="userId">ユーザ識別子(受講者)</param>
        /// <param name="courseId">コース識別子</param>
        /// <param name="chapterId">講座識別子</param>
        /// <param name="times">実施回数</param>
        /// <returns></returns>
        public Task<ShowTestContentsViewModel> GetStudentExaminationResult(string userId, string courseId, string chapterId, string times);

        /// <summary>
        /// テスト実施履歴を取得する
        /// </summary>
        /// <param name="userId">ユーザ識別子(受講者)</param>
        /// <param name="courseId">コース識別子</param>
        /// <param name="chapterId">講座識別子</param>
        /// <param name="times">実施回数</param>
        /// <returns></returns>
        public Task<ShowExaminationHistoryViewModel> GetExaminationHistory(string userId, string courseId, string chapterId);

    }
}
