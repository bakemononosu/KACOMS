using EllipticCurve.Utils;
using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ElsWebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class StudentMyCourseController(
            ILogger<StudentMyCourseController> logger,
            ElsWebAppDbContext context,
            IWebHostEnvironment env,
            SignInManager<IdentityUser> sInMng,
            IStudentMyCourseService svcMyCrs,
            IMovieContentsService svcMvc,
            ICreateExamService svcCreExam
        ) : BaseController(logger, context, env, sInMng)
    {
        private IStudentMyCourseService _studentMyCourseService = svcMyCrs;
        private IMovieContentsService _movieContentsService = svcMvc;
        private ICreateExamService _createExamService = svcCreExam;

        /// <summary>
        /// マイコース一覧
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> ShowMyCourse()
        {
            //ログインユーザーのユーザーID
            var userId = this._loginUser?.UserId.ToString() ?? "";

            List<MyCourse> myCourseList = await this._studentMyCourseService.GetUserCourseList(userId);

            var myCourseModel = new StudentMyCourseViewModel
            {
                MyCourseList = myCourseList
            };

            return View(myCourseModel);
        }

        /// <summary>
        /// 受講者講座の表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("/StudentMyCourse/ShowMyChapter")]
        [Route("/StudentMyCourse/ShowMyChapter/{userId}/{courseId}")]
        public async Task<IActionResult> ShowMyChapter(string userId = "", string courseId = "")
        {

            // 講座一覧を取得
            List<MyChapter> myChapterList = await this._studentMyCourseService.GetUserChapterList(userId, courseId);

            // 画面へ受け渡す
            StudentMyCourseViewModel viewModel = new StudentMyCourseViewModel
            {
                UserId = userId,
                CourseId = courseId,
                MyChapterList = myChapterList
            };

            return View(viewModel);
        }

        /// <summary>
        /// 動画コンテンツの表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("/StudentMyCourse/ShowVideoContents/{userId}/{courseId}/{chapterId}")]
        public async Task<IActionResult> ShowVideoContents(string userId, string courseId, string chapterId)
        {
            // 講座一覧を取得
            List<MyChapter> myChapterList = await this._studentMyCourseService.GetUserChapterList(userId, courseId);

            // 対象講座を取得
            var myChapter = myChapterList.Where(x => string.Compare(x.ChapterId, chapterId, true) == 0);
            // 学習順序を取得
            var orderNo = myChapter.Select(x => x.OrderNo).FirstOrDefault();
            // 動画再生可否を取得
            var isPlayNotAvailable = myChapter.Select(x => x.DeletedFlg).FirstOrDefault();

            // 動画コンテンツを取得
            MovieContents movieContent = await this._movieContentsService.SelectByChapterId(Guid.Parse(chapterId));

            // iOS
            var iOs = CommonService.CheckiOS(Request.Headers.UserAgent.ToString());

            // 前講座情報を取得
            bool isFirstContent = orderNo <= 1;
            if (!isFirstContent)
            {
                MChapter prevChapter = await this._studentMyCourseService.GetPreviousNextChapter(courseId, (byte?)(orderNo - 1));
                isFirstContent = prevChapter.DeletedFlg;
            }

            // 次講座情報を取得
            bool isLastContent = orderNo >= myChapterList.Count;
            if (!isLastContent)
            {
                MChapter nextChapter = await this._studentMyCourseService.GetPreviousNextChapter(courseId, (byte?)(orderNo + 1));
                isLastContent = nextChapter.DeletedFlg;
            }

            // 画面へ受け渡す
            var showVideoContentData = new ShowVideoContent
            {
                UserId = userId,
                ChapterId = chapterId,
                CourseId = courseId,
                MovieContents = movieContent,
                IsFirstContent = isFirstContent, // 前講座が削除済の場合も最初のコンテンツと同じ扱い
                IsLastContent = isLastContent, // 次講座が削除済の場合も最後のコンテンツと同じ扱い
                CurrentOrderNo = (int)orderNo!
            };

            StudentMyCourseViewModel viewModel = new()
            {
                ShowVideoContent = showVideoContentData,
                IsPlayNotAvailable = isPlayNotAvailable,
                IsIOs = iOs
            };

            return View(viewModel);
        }
        /// <summary>
        /// テスト実施確認画面を表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("/StudentMyCourse/ConfirmExamination/{userId}/{courseId}/{chapterId}")]
        public async Task<IActionResult> ConfirmExamination(string userId, string courseId, string chapterId)
        {
            var chapterList = await _studentMyCourseService.GetUserChapterList(userId, courseId);
            var chapter = chapterList.Where(x => string.Compare(x.ChapterId, chapterId, true) == 0)
                .FirstOrDefault();

            //過去問題参照ボタン活性非活性Flg用処理
            bool examinationHistoryFlg = false;
            if (chapter?.ChapterId != null)
            {
                examinationHistoryFlg = await _studentMyCourseService.IsShowExaminationHistory(userId, chapter.ChapterId);
            }

            var confirmExamination = new ConfirmExamination
            {
                UserId = userId,
                ChapterName = chapter?.ChapterName ?? "",
                CourseId = courseId.ToString(),
                ChapterId = chapterId,
                NoDataFlg = chapter?.DeletedFlg.ToString() ?? "False",
                ExaminationHistoryFlg = examinationHistoryFlg
            };

            var studentMyCourseViewModel = new StudentMyCourseViewModel
            {
                ConfirmExaminationData = confirmExamination,
            };

            return View("ConfirmExamination", studentMyCourseViewModel);

        }

        /// <summary>
        /// テストコンテンツの表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("/StudentMyCourse/ShowTestContents")]
        [Route("/StudentMyCourse/ShowTestContents/{userId}/{courseId}/{chapterId}")]
        public async Task<IActionResult> ShowTestContents(string userId = "", string courseId = "", string chapterId = "", string times = "0")
        {
            ShowTestContentsViewModel? model;
            if (times == "0")
            {
                // コース情報の取得
                var courseValid = await this._createExamService.CheckCourseValid(courseId);
                if (!courseValid)
                {
                    throw new Exception("実施可能なテスト情報が存在しません");
                }

                // テスト情報の生成
                model = await this._createExamService.CreateStudentExamination(userId, courseId, chapterId);
            }
            else
            {
                // 採点結果の取得
                model = await this._createExamService.GetStudentExaminationResult(userId, courseId, chapterId, times);
            }

            //return View(model);
            return View("ShowTestContentsNew", model);
        }

        /// <summary>
        /// 問題の解答を採点する
        /// </summary>
        /// <param name="json">問題と解答情報</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GradeAnswers(string json)
        {
            var model = JsonSerializer.Deserialize<ShowTestContentsViewModel>(json);

            if (model == null)
            {
                throw new Exception("解答データを送信できませんでした。");
            }

            System.Diagnostics.Debug.WriteLine("==>GradeAnswers()");
            // 受講者-講座情報の取得
            var result = await this._createExamService.GradeStudentExamination(this._loginUser!.UserId, model);
            if (result)
            {
                var userChapter = await this._createExamService.GetUserChapterData(model.UserChapterId);
                if (userChapter.UserChapterId != Guid.Empty)
                {
                    return Redirect($"/StudentMyCourse/ShowTestContents/{userChapter.UserId}/{userChapter.CourseId}/{userChapter.ChapterId}?Times={model.Times}");
                }
            }

            return View(model);
        }

        /// <summary>
        /// 過去問題の表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("/StudentMyCourse/ShowExaminationHistory/{userId}/{courseId}/{chapterId}")]
        public async Task<IActionResult> ShowExaminationHistory(string userId, string courseId, string chapterId)
        {
            var model = await this._createExamService.GetExaminationHistory(userId, courseId, chapterId);
            //return View(model);
            return View("ShowExaminationHistoryNew", model);
        }

        /// <summary>
        /// 動画再生時の受講開始または終了日時更新
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UpdateUserChapter(string userId, string chapterId, string type)
        {
            var result = await _studentMyCourseService.UpdateUserChapter(userId, chapterId, type);
            if (result > -1)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// 前講座または次講座の表示
        /// </summary>
        /// <returns></returns>
        [Route("/StudentMyCourse/ShowPreviousNextChapter/{userId}/{courseId}/{chapterId}/{type}")]
        public async Task<IActionResult> ShowPreviousNextChapter(string userId, string courseId, string chapterId, string type)
        {
            // 講座一覧を取得
            List<MyChapter> myChapterList = await this._studentMyCourseService.GetUserChapterList(userId, courseId);

            // 対象講座の学習順序を取得
            var orderNo = myChapterList.Where(x => Guid.Parse(x.ChapterId!) == Guid.Parse(chapterId)).Select(x => x.OrderNo).FirstOrDefault();

            // 学習順序のインクリメントまたはデクリメント
            orderNo = (byte?)(type == "previous" ? orderNo - 1 : orderNo + 1);

            // 次対象の講座情報を取得
            MChapter targetChapter = await this._studentMyCourseService.GetPreviousNextChapter(courseId, orderNo);

            // 次対象の講座情報の前講座情報を取得
            bool isFirstContent = orderNo <= 1;
            if (!isFirstContent)
            {
                MChapter prevChapter = await this._studentMyCourseService.GetPreviousNextChapter(courseId, (byte?)(orderNo - 1));
                isFirstContent = prevChapter.DeletedFlg;
            }

            // 次対象の講座情報の次講座情報を取得
            bool isLastContent = orderNo >= myChapterList.Count;
            if (!isLastContent)
            {
                MChapter nextChapter = await this._studentMyCourseService.GetPreviousNextChapter(courseId, (byte?)(orderNo + 1));
                isLastContent = nextChapter.DeletedFlg;
            }

            // 学習コンテンツ区分がテストの場合、テスト実施確認画面へ
            if (targetChapter.ContentsType == ConstService.SystemCode.SYSCODE_CON_TEST)
            {
                return Redirect(string.Format("/StudentMyCourse/ConfirmExamination/{0}/{1}/{2}", userId, courseId, targetChapter.ChapterId));
            }
            else
            {
                // 学習コンテンツ区分が動画の場合、コース講座受講画面へ
                MovieContents movieContent = await this._movieContentsService.SelectByChapterId(targetChapter.ChapterId);
                var showVideoContentData = new ShowVideoContent
                {
                    UserId = userId,
                    ChapterId = targetChapter.ChapterId.ToString(),
                    CourseId = courseId,
                    MovieContents = movieContent,
                    IsFirstContent = isFirstContent, // 前講座が削除済の場合も最初のコンテンツと同じ扱い
                    IsLastContent = isLastContent, // 次講座が削除済の場合も最後のコンテンツと同じ扱い
                    CurrentOrderNo = (int)orderNo!
                };

                StudentMyCourseViewModel studentMyCourseViewModel = new()
                {
                    ShowVideoContent = showVideoContentData,
                };

                return View("ShowVideoContents", studentMyCourseViewModel);
            }
        }
    }
}
