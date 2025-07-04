using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text.Json;

namespace ElsWebApp.Controllers
{
    /// <summary>
    /// 管理者用コントローラークラス
    /// </summary>
    [AutoValidateAntiforgeryToken]
    public class AdminCoursesController(
            ILogger<AdminCoursesController> logger,
            ElsWebAppDbContext context,
            IWebHostEnvironment env,
            SignInManager<IdentityUser> sInMng,
            IAdminCourseService adminCourseSvc,
            ISysCodeService svcSys
        ) : BaseController(logger, context, env, sInMng)
    {
        private IAdminCourseService _adminCourseSvc = adminCourseSvc;
        private ISysCodeService _sysCodeService = svcSys;

        /// <summary>
        /// 登録コース一覧の表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("/AdminCourses/ShowCourses")]
        [Route("/AdminCourses/ShowCourses/{courseName}")]
        public async Task<IActionResult> ShowCourses(string courseName)
        {
            // 登録コース一覧
            var courseList = await this._adminCourseSvc.GetCourseList(false);

            // 優先参照先リスト
            var primaryReferenceList = await GetSelectList();

            // 検索時
            if (!string.IsNullOrEmpty(courseName))
            {
                courseList = courseList.Where(s => s.CourseName != null && s.CourseName.StartsWith(courseName)).ToList();
            }

            // 画面に返す
            var adminCoursesViewModel = new AdminCoursesViewModel
            {
                AdminCourseList = courseList,
                PrimaryReferenceList = primaryReferenceList,
                SearchWord = courseName
            };

            return View(adminCoursesViewModel);
        }

        /// <summary>
        /// コース別登録講座一覧の表示(コース･講座の新規登録、更新)
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("/AdminCourses/ShowCourseChapters")]
        [Route("/AdminCourses/ShowCourseChapters/{courseId}")]
        public async Task<IActionResult> ShowCourseChapters(string courseId)
        {

            AdminCourseChaptersViewModel viewModel = new();

            // 優先参照先区分（名称ラベル）
            var sysCodePrimary = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_PRIMARY);
            viewModel.PriPeriodLabel = CommonService.GetValueByCode(sysCodePrimary, ConstService.SystemCode.SYSCODE_PRI_PERIOD);
            viewModel.PriFlagLabel = CommonService.GetValueByCode(sysCodePrimary, ConstService.SystemCode.SYSCODE_PRI_FLAG);


            // 公開フラグ
            var sysCodePublic = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_PUBLIC);

            // 新規登録の場合
            if (courseId.IsNullOrEmpty())
            {
                // デフォルト設定
                viewModel.PrimaryReference = ConstService.SystemCode.SYSCODE_PRI_PERIOD;
                viewModel.BegineDateTime = DateTime.Now;
                viewModel.EndDateTime = DateTime.Now;
                // 選択肢リスト
                viewModel.PublicFlgList = sysCodePublic.Select(code => new SelectListItem
                {
                    Value = code.Code,
                    Text = code.Value,
                    Selected = (code.Code == ConstService.SystemCode.SYSCODE_PUB_NO)
                }).ToList();
                // コース受講者の有無
                viewModel.IsExistsUserCourse = 0;
            }
            // 編集の場合
            else
            {
                // コース情報
                MCourse course = await this._adminCourseSvc.GetCourseById(courseId);

                // 学習コンテンツ
                var sysCodeContents = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_CONTENTS);

                // 講座一覧
                List<AdminCourseChapter> chapterList = await this._adminCourseSvc.GetCourseChapterList(Guid.Parse(courseId));

                // 画面へ受け渡す
                viewModel.CourseId = courseId;
                viewModel.CourseName = course.CourseName;
                viewModel.CourseExplaination = course.CourseExplaination;
                viewModel.BegineDateTime = course.BegineDateTime;
                viewModel.EndDateTime = course.EndDateTime;
                viewModel.PublicFlg = (course.PublicFlg ? ConstService.SystemCode.SYSCODE_PUB_YES : ConstService.SystemCode.SYSCODE_PUB_NO);
                viewModel.PrimaryReference = course.PrimaryReference;
                // 選択肢リスト
                viewModel.PublicFlgList = sysCodePublic.Select(code => new SelectListItem
                {
                    Value = code.Code,
                    Text = code.Value,
                    Selected = (code.Code == viewModel.PublicFlg)
                }).ToList();
                viewModel.ContentsList = sysCodeContents.Select(code => new SelectListItem
                {
                    Value = code.Code,
                    Text = code.Value,
                }).ToList();
                // 講座一覧
                viewModel.ChapterList = chapterList;
                // コース受講者の有無
                viewModel.IsExistsUserCourse = Convert.ToInt32(await this._adminCourseSvc.IsExistUserCourse(Guid.Parse(courseId)));
            }

            return View(viewModel);
        }

        /// <summary>
        ///  コース別登録講座一覧のコース登録・更新
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateCourse(AdminCourseChaptersViewModel viewModel)
        {
            try
            {
                // ログインユーザ
                Guid loginUserId = this._loginUser?.UserId ?? Guid.Empty;
                bool result = false;
                string newCourseId = "";

                // コース情報の入力値を設定
                MCourse mCourse = new()
                {
                    CourseName = viewModel.CourseName!,
                    CourseExplaination = viewModel.CourseExplaination!,
                    LearningTime = 0,
                    DeletedFlg = false,
                    CreatedBy = loginUserId,
                    UpdatedBy = loginUserId,
                };

                if (viewModel.PrimaryReference == ConstService.SystemCode.SYSCODE_PRI_FLAG)
                {
                    mCourse.PrimaryReference = ConstService.SystemCode.SYSCODE_PRI_FLAG;
                }
                else
                {
                    mCourse.PrimaryReference = ConstService.SystemCode.SYSCODE_PRI_PERIOD;
                }
                mCourse.BegineDateTime = (viewModel.BegineDateTime ?? DateTime.Today).Date + new TimeSpan(0, 0, 0);
                mCourse.EndDateTime = (viewModel.EndDateTime ?? DateTime.Today).Date + new TimeSpan(23, 59, 59);
                mCourse.PublicFlg = (viewModel.PublicFlg == ConstService.SystemCode.SYSCODE_PUB_YES);

                if (viewModel.CourseId == null)
                {
                    // 新規登録
                    result = await this._adminCourseSvc.InsertCourse(mCourse);
                    if (result)
                    {
                        // 採番したコース識別子を取得
                        newCourseId = mCourse.CourseId.ToString();
                    }
                }
                else
                {
                    // 更新時のみコース識別子を設定する
                    mCourse.CourseId = Guid.Parse(viewModel.CourseId);

                    // 更新
                    result = await this._adminCourseSvc.UpdateCourse(mCourse);
                }

                if (result)
                {
                    return Ok(new { newCourseId });
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
        ///  コース別登録講座一覧の講座削除
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteChapter(string chapterId, string courseId)
        {
            try
            {
                var isExistsUserCourse = await this._adminCourseSvc.IsExistUserCourse(Guid.Parse(courseId));
                if (isExistsUserCourse)
                {
                    return BadRequest(new { errData = new { errorMessage = "既に受講者が存在している為、削除できません。" } });
                }

                // ログインユーザ
                Guid loginUserId = this._loginUser?.UserId ?? Guid.Empty;

                // 削除を実行
                var ret = await this._adminCourseSvc.DeleteChapterById(chapterId, loginUserId);

                return ret? Ok(): BadRequest(new { errData = new { errorMessage = "削除できませんでした" } }); ;
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
                return BadRequest();
            }
        }

        /// <summary>
        /// 講座の表示(講座の新規登録・更新)
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("/AdminCourses/ShowChapter")]
        [Route("/AdminCourses/ShowChapter/{courseId}")]
        [Route("/AdminCourses/ShowChapter/{courseId}/{chapterId}")]
        public async Task<IActionResult> ShowChapter(string courseId = "", string chapterId = "")
        {
            ShowChapterViewModel? model = null;
            model = await this._adminCourseSvc.GetChapterInfo(courseId, chapterId);

            return View(model ?? new());
        }

        /// <summary>
        /// 講座情報を登録する
        /// </summary>
        /// <param name="model">講座表示Viewモデル</param>
        /// <returns></returns>
        public async Task<IActionResult> RegisterChapter([FromBody] ShowChapterViewModel model)
        {
            // コース講座(チャプター)を更新する
            var result = await this._adminCourseSvc.RegisterChapterInfo((this._loginUser?.UserId ?? Guid.Empty), model);

            return (result) ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }


        /// <summary>
        /// 大・中・小分類コードから問題カタログを取得する
        /// </summary>
        /// <param name="majorCd">大分類コード</param>
        /// <param name="middleCd">中分類コード</param>
        /// <param name="minorCd">小分類コード</param>
        /// <returns></returns>
        public async Task<IActionResult> GetQuestionCatalogList(string majorCd, string middleCd, string minorCd)
        {
            var lst = await this._adminCourseSvc.SearchQuestionCatalog(majorCd, middleCd, minorCd);

            return Ok(new { data = lst });
        }

        /// <summary>
        /// 動画ファイルのアップロード
        /// </summary>
        /// <param name="video">動画ファイル</param>
        /// <returns></returns>
        [RequestSizeLimit(68157440)]
        public async Task<IActionResult> UploadVideo(List<IFormFile> video)
        {
            var result = string.Empty;
            System.Diagnostics.Debug.WriteLine(this._env.WebRootPath);
            var tempFileName = Path.GetRandomFileName();
            var tempFilePath = $@"{this._env.WebRootPath}{ConstService.PathInfo.PATH_FOLDER_TEMP}\{tempFileName}";
            var m3u8FileName = tempFileName.Replace(".", "");
            var tempFolderPath = $@"{this._env.WebRootPath}{ConstService.PathInfo.PATH_FOLDER_TEMP}\{m3u8FileName}";
            try
            {
                // 一時ファイル作成
                using var st = System.IO.File.Create(tempFilePath);
                await video[0].CopyToAsync(st);
                st.Close();

                // HLSファイル作成
                Directory.CreateDirectory($@"{this._env.WebRootPath}{ConstService.PathInfo.PATH_FOLDER_TEMP}\{m3u8FileName}");
                var tsFilePath = $@"{tempFolderPath}\{m3u8FileName}_%3d.ts";
                var m3u8FilePath = $@"{tempFolderPath}\{m3u8FileName}.m3u8";
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = $"{this._env.WebRootPath}{ConstService.PathInfo.PATH_FFMPEG_EXE}",
                        Arguments = $"-i {tempFilePath} -c:v copy -c:a copy -f hls -hls_time 30 -hls_playlist_type vod -hls_segment_filename \"{tsFilePath}\" {m3u8FilePath}",
                        UseShellExecute = false,
                    }
                };

                if (p.Start())
                {
                    p.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
            }
            finally
            {
                // 一時ファイル削除
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }
            }

            return (result == "") ? Ok(new { FolderPath = tempFolderPath })
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        /// <summary>
        /// 問題カタログ識別子から解答グループリストを作成する
        /// </summary>
        /// <param name="questionId">問題カタログ識別子</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetAnswers(string questionId)
        {
            var qAnsData = await this._adminCourseSvc.SearchAnswerGroup(Guid.Parse(questionId));

            return Ok(new { data = qAnsData });
        }

        /// <summary>
        /// 優先参照先SelectList整形
        /// </summary>
        /// <returns></returns>
        private async Task<List<SelectListItem>> GetSelectList()
        {
            // 優先参照先
            var sysPrimaryReferenceCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_PRIMARY);
            var primaryReferenceList = sysPrimaryReferenceCode.Select(code => new SelectListItem
            {
                Value = code.Code,
                Text = code.Value,
            }).ToList();


            return (primaryReferenceList);
        }

        /// <summary>
        /// コースマスタ更新
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <param name="primaryReference">優先参照先区分</param>
        /// <param name="publicFlg">公開フラグ</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateMCourse(string courseId, string primaryReference, bool publicFlg)
        {
            try
            {
                var resut = await this._adminCourseSvc.HandlRegisterCourseInfo(courseId, primaryReference, publicFlg);

                if (resut)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);
                return BadRequest();
            }

        }

        /// <summary>
        /// 削除フラグ更新
        /// </summary>
        /// <param name="courseId">コース識別子</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> DeleteCourse(string courseId)
        {
            try
            {
                var isExistsUserCourse = await this._adminCourseSvc.IsExistUserCourse(Guid.Parse(courseId));
                if(isExistsUserCourse) {
                    return BadRequest(new { errData = new { errorMessage = "既に受講者が存在している為、削除できません" } });
                }

                var resut = await this._adminCourseSvc.UpdateDeleteFlg(courseId, _loginUser!.UserId);

                if (resut)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { errData = new { errorMessage = "削除に失敗しました" } });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }

        /// <summary>
        ///  コース別登録講座一覧の学習順を修正
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateChapterOrderNo(string fixedTableCourseChapterData, string courseId)
        {
            try
            {
                var isExistsUserCourse = await this._adminCourseSvc.IsExistUserCourse(Guid.Parse(courseId));
                if (isExistsUserCourse)
                {
                    return BadRequest(new { errData = new { errorMessage = "既に受講者が存在している為、学習順を更新できません" } });
                }

                // ログインユーザ
                Guid loginUserId = this._loginUser?.UserId ?? Guid.Empty;

                // 講座情報更新
                bool result = await this._adminCourseSvc.UpdateChapterOrderNo(fixedTableCourseChapterData, loginUserId);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { errData = new { errorMessage = "学習順を更新できませんでした" } });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }
    }
}
