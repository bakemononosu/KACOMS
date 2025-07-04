using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol;
using System;
using System.Text.RegularExpressions;

namespace ElsWebApp.Controllers
{
    public class BaseController : Controller
    {
        private readonly ILogger<BaseController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;

        /// <summary>
        /// 機能毎のルートURL
        /// </summary>
        private static readonly Dictionary<string, string> topMenu = new(){
            { ConstService.PathInfo.PATH_ST_PAGE_SHOW_MY_COURSE, "マイ講座一覧" },
            { ConstService.PathInfo.PATH_ST_PAGE_SHOW_COURSES, "講座一覧" },
            { ConstService.PathInfo.PATH_AD_PAGE_SHOW_STUDENTS , "受講者管理" },
            { "/AdminStudents/SearchStudents", "受講者管理" },
            { ConstService.PathInfo.PATH_AD_PAGE_SHOW_COURSES, "講座管理" },
            { ConstService.PathInfo.PATH_AD_PAGE_SHOW_COURSES_STATUS,"進捗管理" },
            { ConstService.PathInfo.PATH_AD_PAGE_SHOW_COURSES_TEST_STATUS,"テスト実施状況" },
            { ConstService.PathInfo.PATH_AD_PAGE_SHOW_ALL_QUESTIONS,"テスト管理" },
            { ConstService.PathInfo.PATH_AD_PAGE_SHOW_ACCOUNTS,"管理者アカウント管理" },
            { ConstService.PathInfo.PATH_ST_PAGE_SHOW_ACCOUNT,"アカウント情報" }
        };

        /// <summary>
        /// 2番目以降の遷移先のURL
        /// </summary>
        private static readonly Dictionary<string, string> nthMenu = new() {
            { "/StudentMyCourse/ShowMyChapter", "%COURSE%" },
            { "/StudentMyCourse/ShowVideoContents", "%CHAPTER%" },
            { "/StudentMyCourse/ShowTestContents", "%CHAPTER%" },
            { "/StudentMyCourse/ConfirmExamination", "テスト実施確認" },
            { "/StudentMyCourse/ShowExaminationHistory", "過去の問題の参照" },
            { "/StudentCourses/ShowChapters", "%COURSE%" },
            { "/AdminStudents/ShowIndividual", "%USER%"},
            { "/AdminStudents/ShowIndividualNew", "新規登録"},
            { "/AdminStudents/TransitionBulkUpload", "一括更新"},
            { "/AdminTaskStatus/ShowStudentCoursesStatus","%USER%"},
            { "/AdminTaskStatus/ShowStudentChaptersStatus","%USER%"},
            { "/AdminTestStatus/ShowCourseStudentTestStatus","%USER%"},
            { "/AdminCourses/ShowCourseChapters", "講座更新"},
            { "/AdminCourses/ShowChapter", "セクション更新"},
            { "/AdminQuestions/ShowQuestionsNewView","テスト個別更新"},
            { "/AdminQuestions/ShowQuestionsView","テスト個別更新"},
            { "/AccountInfo/ShowAdminAccount","%USER%" },
        };

        /// <summary>
        /// 機能毎の先頭のアクション
        /// </summary>
        private static readonly Dictionary<string, string> baseAction = new(){
            { "StudentMyCourse","/ShowMyCourse"},
            { "StudentCourses","/ShowCourses" },
            { "AdminStudents","/ShowStudents" },
            { "AdminTaskStatus","/ShowStudentsAnyCoursesStatus"},
            { "AdminTestStatus","/ShowCourseTestStatus"},
            { "AdminCourses","/ShowCourses"},
            { "AdminQuestions","/ShowAllQuestions"},
            { "AccountInfo/ShowAccounts","/AccountInfo/ShowAccounts" },
            { "AccountInfo/ShowAdminAccount","/AccountInfo/ShowAccounts" },
            { "AccountInfo/ShowStudentAccount","/AccountInfo/ShowStudentAccount" },
        };

        private readonly Dictionary<string, string> ContentsSwitch = new()
        {
            { ConstService.SystemCode.SYSCODE_CON_MOVIE, ConstService.PathInfo.PATH_AD_PAGE_SHOW_VIDEO },
            { ConstService.SystemCode.SYSCODE_CON_TEST, ConstService.PathInfo.PATH_AD_PAGE_CONFIRM_EXAM },
        };
        private const string BREADCRUMB_SEPARATER = " > "; 

        protected ElsWebAppDbContext _dbContext { get; set; }
        protected IWebHostEnvironment _env { get; set; }
        protected MUser? _loginUser { get; set; } = new MUser();

        public BaseController(
            ILogger<BaseController> logger,
            ElsWebAppDbContext context,
            IWebHostEnvironment env,
            SignInManager<IdentityUser> sInMng)
        {
            _dbContext = context;
            _logger = logger;
            _env = env;
            _signInManager = sInMng;
        }

        /// <summary>
        /// 例外エラーハンドラ
        /// </summary>
        /// <returns></returns>
        public IActionResult Error()
        {
            var model = new ErrorViewModel();
            return View("/Views/Shared/Error.cshtml", model);
        }

        /// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            RouteValueDictionary routeValue = context.RouteData.Values;

            // ログイン画面の場合は除外
            if (routeValue["controller"]?.ToString() == "Login")
            {
                return;
            }

            // 初期処理の場合
            if (routeValue["action"]?.ToString() == "Index")
            {
                // 処理
                Console.WriteLine("[デバッグログ]Index");

            }

            // 共通で行いたい処理
            this.CommonCheck();

            // 共通情報に設定 
            var loginId = User.Identity?.Name ?? "";
            TempData["LoginUser"] = loginId;

            if (!"".Equals(loginId))
            {
                this._loginUser = this._dbContext.MUser
                    .Where(x => x.LoginId == loginId)
                    .FirstOrDefault();

                ViewData["UserName"] = this._loginUser? .UserName?? "";
                ViewData["UserRole"] = this._loginUser? .UserRole ?? "9";
                ViewData["UserId"] = this._loginUser? .UserId.ToString() ?? "";
            }
            ViewData["LoginId"] = loginId;

            // .NET共通処理
            var controller = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
            var action = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ActionName;

            if ((this._loginUser?.UserRole ?? ConstService.SystemCode.SYSCODE_USR_USERS) == ConstService.SystemCode.SYSCODE_USR_USERS &&
               (controller.Contains("Admin") || action.Contains("Admin")))
            {
                throw new Exception("権限のないページに遷移しようとしました。");
            }

            CreateHistoryList($"/{controller}/{action}", context.ActionArguments);

            var param = new object[] { controller, action };
            this._logger.LogInformation("Enter /{controller}/{action}", param);
            base.OnActionExecuting(context);
            this._logger.LogInformation("Leave /{controller}/{action}", param);
        }

        /// <summary>
        /// 独自基盤共通処理
        /// </summary>
        /// <returns></returns>
        protected bool CommonCheck()
        {
            // トークンチェック
            if (!TokenCheck())
            {
                return false;
            }

            // 権限チェック
            if (!RoleCheck())
            {
                return false;
            }

            return true;
        }

        protected bool SessionCheck()
        {
            // セッションの値があればtrue。なければfalse。

            return true;
        }

        protected bool TokenCheck()
        {
            // セッションの値があればtrue。なければfalse。

            return true;
        }

        protected bool RoleCheck()
        {
            if (String.IsNullOrEmpty(User.Identity?.Name))
            {
                return false;
            }

            return true;
        }

        protected void LogInfoemation(string? message, params object?[] args)
        {
            this._logger.LogInformation(message, args);
        }

        protected async Task<string> SetUser(string loginId)
        {
            return await this._dbContext!.MUser
                .Where(x => x.LoginId == loginId)
                .Select(x =>x.UserName)
                .FirstOrDefaultAsync()?? "";
        }

        /// <summary>
        /// パンくずリストを生成する
        /// </summary>
        /// <param name="path">遷移先のURL</param>
        /// <param name="argments">遷移先へ連携するパラメータ</param>
        protected void CreateHistoryList(string path, IDictionary<string, object?> argments)
        {
            //Request.Headers.TryGetValue("Referer", out var referer);
            
            //if (referer.ToString() == "")
            //{
            //    return;
            //}

            foreach(var key in baseAction.Keys)
            {
                var controllerName = Regex.Match(path, @"^/\w+/").Value.Replace("/","");
                if (key == controllerName)
                {
                    ViewData["TopURL"] = $"/{key}{baseAction[key]}";
                    break;
                }
                else if ($"/{key}" == path)
                {
                    ViewData["TopURL"] = $"{baseAction[key]}";
                    break;
                }
            }

            // クエリ文字列生成
            var qArray = argments.Where(x => x.Value != null)
                .Select(x => $"{x.Key}={x.Value}")
                .ToArray();
            var qStr = (qArray.Length > 0)? $"?{string.Join('&', qArray)}" : string.Empty;

            string? value; 
            var breadcrumb = HttpContext.Session.GetString("breadcrumb")?? string.Empty;
            if (topMenu.TryGetValue(path, out value))
            {
                var option = argments.Select(x => argments[x.Key]).ToArray();
                breadcrumb = $"{path}{qStr}:{value}";
            }
            else
            {
                if (breadcrumb != string.Empty)
                {
                    var middle = breadcrumb!.IndexOf($"{BREADCRUMB_SEPARATER}{path}");
                    var last = (middle > 0)? 
                        breadcrumb.IndexOf(BREADCRUMB_SEPARATER, middle + BREADCRUMB_SEPARATER.Length) : -1;
                    if (last > 0)
                    {
                        // パンくずリストの途中のリンクが選択された場合
                        breadcrumb = breadcrumb[..last];
                    }
                    else if (nthMenu.TryGetValue(path, out value))
                    {
                        object? userId = "";
                        object? courseId = "";
                        object? chapterId = "";
                        switch (value)
                        {
                            case "%USER%":
                                if (argments.TryGetValue("userId", out userId))
                                {
                                    value = this._dbContext.MUser
                                        .Where(x => x.UserId.ToString() == userId!.ToString())
                                        .Select(x => x.UserName)
                                        .FirstOrDefault() ?? "不明なユーザー";
                                }
                                break;
                            case "%COURSE%":
                                if (argments.TryGetValue("courseId", out courseId))
                                {
                                    value = this._dbContext.MCourse
                                        .Where(x => x.CourseId.ToString() == courseId!.ToString())
                                        .Select(x => x.CourseName)
                                        .FirstOrDefault() ?? "不明なコース";
                                }
                                break;
                            case "%CHAPTER%":
                                userId = argments["userId"]? .ToString() ?? "";
                                courseId = argments["courseId"]?.ToString() ?? "";
                                MCourse? course = null;
                                course = this._dbContext.MCourse
                                    .Where(x => x.CourseId.ToString().Equals(courseId))
                                    .FirstOrDefault();

                                chapterId = argments["chapterId"]? .ToString() ?? "";
                                value = this._dbContext.UserChapter
                                    .Where(x => x.UserId.ToString() == (string)userId)
                                    .Where(x => x.CourseId.ToString() == (string)courseId)
                                    .Where(x => x.MChapter != null)
                                    .Select(x => new
                                    {
                                        pathName = ContentsSwitch[x.MChapter!.ContentsType],
                                        userId,
                                        courseId = x.CourseId,
                                        chapterName = x.MChapter!.ChapterName,
                                        status = x.Status,
                                        order = x.MChapter.OrderNo,
                                        chapterId = x.MChapter.ChapterId,
                                        selected = x.MChapter.ChapterId == Guid.Parse((string)chapterId),
                                        disabled = (x.MChapter.DeletedFlg || course == null || course.DeletedFlg),
                                    })
                                    .OrderBy(x => x.order)
                                    .ToJson();
                                break;
                        }
                        if (last < 0)
                        {
                            if (middle > 0)
                            {
                                breadcrumb = breadcrumb[..middle];
                            }
                            else
                            {
                                last = Math.Max(
                                    Math.Max(
                                        Math.Max(
                                            breadcrumb.LastIndexOf($"{BREADCRUMB_SEPARATER}{ConstService.PathInfo.PATH_AD_PAGE_SHOW_VIDEO}"),
                                            breadcrumb.LastIndexOf($"{BREADCRUMB_SEPARATER}{ConstService.PathInfo.PATH_AD_PAGE_SHOW_TEST}")
                                        ),
                                        breadcrumb.LastIndexOf($"{BREADCRUMB_SEPARATER}{ConstService.PathInfo.PATH_AD_PAGE_CONFIRM_EXAM}")
                                    ),
                                    breadcrumb.LastIndexOf($"{BREADCRUMB_SEPARATER}{ConstService.PathInfo.PATH_AD_PAGE_EXAM_HISTORY}")
                                );
                                if (last > 0)
                                {
                                    breadcrumb = breadcrumb[..last];
                                }
                            }
                        }
                        breadcrumb = $"{breadcrumb}{BREADCRUMB_SEPARATER}{path}{qStr}:{value}";
                    }
                }
            }
            HttpContext.Session.SetString("breadcrumb", breadcrumb);
        }
    }
}
