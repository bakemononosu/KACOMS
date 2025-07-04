using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ElsWebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class StudentCoursesController(
            ILogger<StudentCoursesController> logger,
            ElsWebAppDbContext context,
            IWebHostEnvironment env,
            SignInManager<IdentityUser> sInMng,
            ICourseService svcCourse,
            IStudentCoursesService svcCrs
        ) : BaseController(logger, context, env, sInMng)
    {
        private ICourseService _courseService = svcCourse;
        private IStudentCoursesService _studentCoursesService = svcCrs;

        /// <summary>
        /// コース一覧の表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> ShowCourses()
        {

            // ログインユーザを取得
            Guid loginUserId = this._loginUser?.UserId ?? Guid.Empty;

            // 講座一覧を取得
            List<StudentCourse> courseList = await this._studentCoursesService.GetUserCourseList(loginUserId);

            // 画面へ受け渡す
            StudentCoursesViewModel viewModel = new ()
            {
                CourseList = courseList
            };

            return View(viewModel);
        }


       
        /// <summary>
        /// マイコースへの追加・削除
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateMyCourse(string courseId, bool deletedFlg)
        {
            try
            {

                // ログインユーザ
                Guid loginUserId = this._loginUser?.UserId ?? Guid.Empty;

                // 更新データを作成
                UserCourse userCourse = new()
                {
                    UserId = loginUserId,
                    CourseId = Guid.Parse(courseId),
                    DeletedFlg = deletedFlg,
                    UpdatedBy = loginUserId,
                    CreatedBy = loginUserId,
                };

                if (!deletedFlg)
                {
                    // 受講の場合
                    var available = await this._courseService.CheckCourseIsAvailable(Guid.Parse(courseId));
                    if (!available) {
                        // コースが無効(非公開、または削除)
                        return Ok(new { status="NG", message="選択された講座は受講できません。"});
                    }
                }

                // 更新を実行
                int result = await this._studentCoursesService.UpdateByDeletedFlg(userCourse);
                // 更新データがなかった場合は追加
                if (result == 0)
                {
                    if (!deletedFlg)
                    {
                        result = await this._studentCoursesService.InsertUserCourse(userCourse);
                    }
                    else
                    {
                        // 削除の場合はデータなしでも結果が同じなのでスルー
                        result = 1;
                    }
                }

                if (result > 0)
                {
                    return Ok(new { status = "OK", message = (!deletedFlg)? "追加しました" : "解除しました"});
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
                return BadRequest();
            }
        }

        /// <summary>
        /// 講座一覧の表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("StudentCourses/ShowChapters/{courseId}")]
        public async Task<IActionResult> ShowChapters(string courseId)
        {
            // 講座一覧を取得

            List<Chapters> chapterList = await this._studentCoursesService.GetChapterList(courseId);

            StudentCoursesViewModel viewModel = new ()
            {

                CourseId = courseId,
                ChapterList = chapterList,
            };

            return View(viewModel);
        }
    }
}
