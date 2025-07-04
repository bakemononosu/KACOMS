using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ElsWebApp.Controllers
{
    /// <summary>
    /// 管理者用コントローラークラス
    /// </summary>
    [AutoValidateAntiforgeryToken]
    public class AdminTestStatusController(
            ILogger<AdminTestStatusController> logger,
            ElsWebAppDbContext context,
            IWebHostEnvironment env,
            SignInManager<IdentityUser> sInMng,
            IAdminTestStatusService svcAdts,
            IElsService svcEls
        ) : BaseController(logger, context, env, sInMng)
    {
        private readonly IElsService _elsService = svcEls;
        private readonly IAdminTestStatusService _adminTestStatusService = svcAdts;

        /// <summary>
        /// 受講者（コース指定）毎のテスト実施状況の表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("/AdminTestStatus/ShowCourseTestStatus")]
        public async Task<IActionResult> ShowCourseTestStatus()
        {
            var coursesList = await GetSelectList();

            var testStatusViewModel = new TestStatusViewModel
            {
                CourseTestStatusList = null,
                ScoresHeader = [],
                CoursesList = coursesList,
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
            };

            return View(testStatusViewModel);
        }

        /// <summary>
        /// 受講者のテスト実施状況の表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("/AdminTestStatus/ShowCourseStudentTestStatus")]
        public async Task<IActionResult> ShowCourseStudentTestStatus(string companyName, string courseId, string userId)
        {
            var coursesList = await GetSelectList(courseId);
            List<CourseStudentTestStatus> testStatusList = this._adminTestStatusService.GetTestStatus(String.Empty, userId);
            testStatusList = await this._adminTestStatusService.FilterTestStatusData(testStatusList);

            var testStatusViewModel = new TestStatusViewModel
            {
                CourseStudentTestStatusList = testStatusList.Count > 0 ? testStatusList : null,
                ScoresHeader = null,
                CoursesList = coursesList,
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
                UserEnteredCorporateName = companyName,
                UserEnteredCourse = courseId
            };

            return View(testStatusViewModel);
        }

        /// <summary>
        /// テスト実施状況の検索
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SearchTestStatus(string companyName, string courseId)
        {
            var coursesList = await GetSelectList(courseId);
            List<CourseStudentTestStatus> testStatusList = this._adminTestStatusService.GetTestStatus(courseId, String.Empty);
            // 法人名のフィルタ
            if (!string.IsNullOrEmpty(companyName))
            {
                testStatusList = testStatusList.Where(t => t.CompanyName != null && t.CompanyName.StartsWith(companyName)).ToList();
            }

            // ContentsNameでグループ化
            var pivotData = testStatusList.GroupBy(item => new { item.UserId, item.UserName, item.CompanyName, item.CourseName })
                .Select(group => new
                {
                    group.Key.UserId,
                    group.Key.UserName,
                    group.Key.CompanyName,
                    group.Key.CourseName,
                    Scores = group.ToDictionary(item => item.ChapterName ?? "NULL", item => item.得点)
                })
                .ToList();

            List<CourseTestStatus> typedPivotData = pivotData
                .Select(item => new CourseTestStatus
                {
                    UserId = item.UserId,
                    UserName = item.UserName,
                    CompanyName = item.CompanyName,
                    CourseName = item.CourseName,
                    Scores = item.Scores
                })
                .ToList();

            List<string> scoresHeader = await this._adminTestStatusService.GetScoresHeader(courseId);

            var testStatusViewModel = new TestStatusViewModel
            {
                CourseTestStatusList = typedPivotData.Count > 0 ? typedPivotData : null,
                ScoresHeader = (scoresHeader.Count == 0 || (scoresHeader.Count == 1 && String.IsNullOrEmpty(scoresHeader[0]))) ? null : scoresHeader,
                CoursesList = coursesList,
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
                UserEnteredCorporateName = companyName,
                UserEnteredCourse = courseId
            };

            return View("ShowCourseTestStatus", testStatusViewModel);
        }

        // 検索条件SelectList整形
        private async Task<List<SelectListItem>> GetSelectList(string selectedCourse = "")
        {
            // コース
            var courses = await this._elsService.GetCourseList(true);
            var coursesList = courses.Select(code => new SelectListItem
            {
                Value = code.CourseId.ToString(),
                Text = code.CourseName,
                Selected = code.CourseId.ToString() == selectedCourse
            }).ToList();


            return (coursesList);
        }
    }
}
