using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;

namespace ElsWebApp.Services.IService
{
    public interface IStudentMyCourseService
    {

        /// <summary>
        /// コース名を取得する
        /// </summary>
        /// <returns></returns>
        public Task<string> GetCourseName(string courseId);

        /// <summary>
        /// コースの一覧を取得する
        /// </summary>
        /// <returns></returns>
        public Task<List<MyCourse>> GetUserCourseList(string userId);

        /// <summary>
        /// 講座の一覧を取得する
        /// </summary>
        /// <returns></returns>
        public Task<List<MyChapter>> GetUserChapterList(string userId, string courseId);

        /// <summary>
        /// 受講開始または終了日時を更新する
        /// </summary>
        /// <returns></returns>
        public Task<int> UpdateUserChapter(string userId, string chapterId, string type);

        /// <summary>
        /// 現在の前または次の講座情報を取得する
        /// </summary>
        /// <returns></returns>
        public Task<MChapter> GetPreviousNextChapter(string courseId, byte? orderNo);

        /// <summary>
        /// コースを取得する
        /// </summary>
        /// <returns></returns>
        public Task<MCourse> GetMCourse(string courseId);


        /// <summary>
        /// 過去問題参照ボタン活性非活性制御Flg付与メソッド
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsShowExaminationHistory(string userId, string ChapterId);

    }
}

