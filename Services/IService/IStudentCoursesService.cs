using ElsWebApp.Models.Entitiy;
using ElsWebApp.Models;

namespace ElsWebApp.Services.IService
{
    public interface IStudentCoursesService
    {

        /// <summary>
        /// コース一覧を取得する
        /// </summary>
        /// <returns></returns>
        public Task<List<StudentCourse>> GetUserCourseList(Guid userId);

        /// <summary>
        /// 受講者コースデータを取得する
        /// </summary>
        /// <returns></returns>
        public Task<UserCourse> SelectUserCourseByKey(Guid userId, Guid courseId);

        /// <summary>
        /// 受講者コースデータの削除フラグを更新する
        /// </summary>
        /// <returns></returns>
        public Task<int> UpdateByDeletedFlg(UserCourse userCourse);

        /// <summary>
        /// 受講者コースデータを新規登録する
        /// </summary>
        /// <returns></returns>
        public Task<int> InsertUserCourse(UserCourse userCourse);

        /// <summary>
        /// 講座の一覧を取得する
        /// </summary>
        /// <returns></returns>
        public Task<List<Chapters>> GetChapterList(string courseId);

    }
}

