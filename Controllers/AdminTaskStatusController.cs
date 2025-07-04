using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Unicode;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ElsWebApp.Controllers
{
    /// <summary>
    /// 管理者用コントローラークラス
    /// </summary>
    [AutoValidateAntiforgeryToken]
    public class AdminTaskStatusController(
            ILogger<AdminTaskStatusController> logger,
            ElsWebAppDbContext context,
            IWebHostEnvironment env,
            SignInManager<IdentityUser> sInMng,
            IAdminTaskStatusService svcAdts,
            IElsService svcEls
        ) : BaseController(logger, context, env, sInMng)
    {
        private IElsService _elsService = svcEls;
        private IAdminTaskStatusService _adminTaskStatusService = svcAdts;

        /// <summary>
        /// 受講者毎の進捗率の表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("/AdminTaskStatus/ShowStudentsAnyCoursesStatus")]
        [Route("/AdminTaskStatus/ShowStudentsAnyCoursesStatus/{courseId}")]
        public async Task<IActionResult> ShowStudentsAnyCoursesStatus()
        {
            var taskStatusList = this._adminTaskStatusService.GetTaskStatus(string.Empty, false);
            if ((this._loginUser?.UserRole ?? "") == ConstService.SystemCode.SYSCODE_USR_CORPO)
            {
                // ログインユーザが法人管理者の場合
                // 法人名にログインユーザの法人名を表示(入力不可)
                taskStatusList = taskStatusList.Where(t => t.CompanyName == (this._loginUser?.CompanyName ?? "")).ToList();
            }

            var coursesList = await GetSelectList();

            var taskStatusViewModel = new TaskStatusViewModel
            {
                TaskStatusList = taskStatusList,
                CoursesList = coursesList,
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
            };

            return View(taskStatusViewModel);
        }

        /// <summary>
        /// 進捗情報の検索
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SearchTaskStatus(string companyName, string userName, string courseId, int minRate, int maxRate)
        {
            List<Models.TaskStatus> taskStatusList = [];

            // 進捗率の検索範囲設定
            (int, int) range = (Math.Min(minRate, maxRate), Math.Max(minRate, maxRate));

            if (!string.IsNullOrEmpty(courseId))
            {
                taskStatusList = this._adminTaskStatusService.GetTaskStatus(courseId, false);
                taskStatusList = taskStatusList.Where(t => string.Compare(t.CourseId, courseId, true) == 0).ToList();
            }
            else
            {
                taskStatusList = this._adminTaskStatusService.GetTaskStatus(string.Empty, false);
            }

            // 進捗率で絞り込み
            if ((range.Item1 > 0) || (range.Item2 < 100))
            {
                taskStatusList = taskStatusList.Where(t => (int)((t.ProgressRate?? 0.0) * 100) >= range.Item1 &&
                                                           (int)((t.ProgressRate?? 0.0) * 100) <= range.Item2).ToList();
            }

            // 受講者名
            if (!string.IsNullOrEmpty(userName))
            {
                taskStatusList = taskStatusList.Where(t => t.UserName != null && t.UserName.StartsWith(userName)).ToList();
            }
            // 法人名
            if (!string.IsNullOrEmpty(companyName))
            {
                taskStatusList = taskStatusList.Where(t => t.CompanyName != null && t.CompanyName.StartsWith(companyName)).ToList();
            }

            if ((this._loginUser?.UserRole ?? "") == ConstService.SystemCode.SYSCODE_USR_CORPO)
            {
                // ログインユーザが法人管理者の場合
                // 法人名にログインユーザの法人名を表示(入力不可)
                taskStatusList = taskStatusList.Where(x => x.CompanyName == (this._loginUser?.CompanyName ?? "")).ToList();
            }

            var coursesList = await GetSelectList(courseId);

            var taskStatusViewModel = new TaskStatusViewModel
            {
                TaskStatusList = taskStatusList,
                CoursesList = coursesList,
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
                UserEnteredCorporateName = companyName,
                UserEnteredName = userName,
                UserEnteredCourse = courseId,
                MinRate = range.Item1,
                MaxRate = range.Item2,
            };

            return View("ShowStudentsAnyCoursesStatus", taskStatusViewModel);
        }

        /// <summary>
        /// 受講者のコース進捗率の表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("/AdminTaskStatus/ShowStudentCoursesStatus")]
        public async Task<IActionResult> ShowStudentCoursesStatus(string userId, string enteredUserName, string enteredCompanyName, string enteredCourse)
        {
            var taskStatusList = this._adminTaskStatusService.GetTaskStatus(string.Empty, true);
            taskStatusList = taskStatusList.Where(x => x.UserId.ToString() == userId).ToList();

            var coursesList = await GetSelectList();
            var taskStatusViewModel = new TaskStatusViewModel
            {
                TaskStatusList = taskStatusList,
                CoursesList = coursesList,
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
                UserEnteredCorporateName = enteredCompanyName,
                UserEnteredName = enteredUserName,
                UserEnteredCourse = enteredCourse,
            };

            return View(taskStatusViewModel);
        }

        /// <summary>
        /// 受講者個別の講座毎進捗率の表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("/AdminTaskStatus/ShowStudentChaptersStatus")]
        public async Task<IActionResult> ShowStudentChaptersStatus(string userId, string enteredUserName, string enteredCompanyName, string enteredCourse)
        {
            var taskStatusList = this._adminTaskStatusService.GetChapterStatus(enteredCourse);
            taskStatusList = taskStatusList.Where(x => x.UserId.ToString() == userId).ToList();

            var coursesList = await GetSelectList(enteredCourse);
            var taskStatusViewModel = new TaskStatusViewModel
            {
                TaskStatusList = taskStatusList,
                CoursesList = coursesList,
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
                UserEnteredCorporateName = enteredCompanyName,
                UserEnteredName = enteredUserName,
                UserEnteredCourse = enteredCourse,
            };

            return View(taskStatusViewModel);
        }

        /// <summary>BOMマーカー</summary>
        private readonly byte[] BOM_MAKER = [0xEF, 0xBB, 0xBF];
        /// <summary>CSVヘッダー</summary>
        private const string CSV_HEADER = "受講者名,メールアドレス,講座名,セクション名,コンテンツ,開始日時,終了日時,状態";
        /// <summary>
        /// 受講者の講座の学習進捗をダウンロードする
        /// </summary>
        /// <param name="corpName">法人名</param>
        /// <param name="userName">ユーザ名</param>
        /// <param name="courseId">コースID</param>
        /// <param name="pRateS">進捗率範囲(開始)</param>
        /// <param name="pRateE">進捗率範囲(終了)</param>
        /// <returns></returns>
        public IActionResult DownloadCSV(
            string corpName,
            string userName,
            string courseId,
            string pRateS,
            string pRateE)
        {
            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms);
            var csvDataList = this._adminTaskStatusService.GetDownloadCsvData(corpName, userName, courseId);
            if (csvDataList.Count == 0)
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            // 進捗率の範囲を取得
            var maxRate = Math.Max(int.Parse(pRateS), int.Parse(pRateE));
            var minRate = Math.Min(int.Parse(pRateS), int.Parse(pRateE));

            // 初回データ取得
            var prevData = EditCommonData(csvDataList[0]);

            var scoreList = new List<string>();     // 採点結果リスト
            var maxRetrys = 0;                      // 再試験回数の最大値
            var saveRetrys = 0;                     // 再試験回数の最大値
            var count = 0;                          // トータル行数
            (int, int) progress = (0, 0);           // コース(講座)進捗率算出用変数：完了数, 講座(セクション)数
            (int, int) progresses = (0, 0);         // コース(講座)毎進捗率算出用変数：コース毎の進捗率の合計, コース(講座)数
            List<string> tempCsvDataList = [];      // ユーザ単位のCSVデータ 
            foreach (var data in csvDataList)
            {
                // キー割れ判定
                if ((Guid.Parse(prevData[0]) != data.UserId) ||
                    (Guid.Parse(prevData[1]) != data.CourseId) ||
                    (Guid.Parse(prevData[2]) != data.ChapterId))
                {
                    // CSVデータ一時保存
                    SaveTempCsvDataByStudent(tempCsvDataList, prevData, scoreList);

                    // コース(講座)、または受講者が変われば、CSVデータ出力する
                    WriteCsvData(
                        Guid.Parse(prevData[1]) != data.CourseId,
                        Guid.Parse(prevData[0]) != data.UserId, sw);

                    // 採点情報クリア
                    scoreList.Clear();

                    // 共通データを更新
                    prevData = EditCommonData(data);
                }

                // テストコンテンツの場合、採点情報を一時保存
                if ((data.ContentsType == ConstService.SystemCode.SYSCODE_CON_TEST) && (data.NthTime != null))
                {
                    scoreList.Add($"\"{data.CollectAnswers}／{data.Total}\"");

                    if (data.NthTime > 1)
                    {
                        // 再テスト2回目以降の場合
                        continue;
                    }
                }

                // 進捗率計算ようデータの蓄積
                progress.Item2++;               // 講座(セクション)数を加算
                if (data.Status == ConstService.SystemCode.SYSCODE_STS_COMPLETE)
                {
                    progress.Item1++;           // 講座(セクション)の完了数を加算
                }

            }
            // 未処理のCSVデータを処理
            // CSVデータ一時保存
            SaveTempCsvDataByStudent(tempCsvDataList, prevData, scoreList);
            // 無条件にCSVデータ出力する
            WriteCsvData(true, true, sw);
            sw.Close();

            // CSVヘッダ作成
            var headerData = $"{CSV_HEADER}";
            for (var idx = 0; idx < maxRetrys; idx++)
            {
                headerData = $"{headerData},{idx + 1}回目";
            }
            headerData = $"{headerData}\n";

            // CSVファイルイメージ作成
            var csvHead = Encoding.UTF8.GetBytes(headerData);
            var csvBody = ms.ToArray();
            var csvFile = new byte[3+csvHead.Length +csvBody.Length];

            Array.Copy(BOM_MAKER, csvFile, BOM_MAKER.Length);
            Array.Copy(csvHead, 0,csvFile, 3, csvHead.Length);
            Array.Copy(csvBody, 0, csvFile, (3 + csvHead.Length), csvBody.Length);
            ms.Close();

            return new FileContentResult(csvFile, "text/csv");

            // 動画/テストコンテンツで共通の情報を編集する
            string[] EditCommonData(DownloadCsvData d)
            {
                var items = new string[12];
                items[0] = $"{d.UserId}";                                   // キー情報   ：ユーザID
                items[1] = $"{d.CourseId}";                                 // キー情報   ：コース(講座)ID
                items[2] = $"{d.ChapterId}";                                // キー情報   ：講座(セクション)ID
                items[3] = $"{d.Status}";                                   // その他情報 ：学習状況CD
                items[4] = $"\"{d.UserName}\"";                             // CSV出力情報：ユーザ名
                items[5] = $"\"{d.EMail}\"";                                // CSV出力情報：メールアドレス
                items[6] = $"\"{d.CourseName}\"";                           // CSV出力情報：コース(講座)名
                items[7] = $"\"{d.ChapterName}\"";                          // CSV出力情報：講座(セクション)名
                items[8] = $"\"{d.ContentsTypeName}\"";                     // CSV出力情報：コンテンツ種別
                items[9] = $"\"{d.StartDateTime:yyyy/MM/dd HH:mm:ss}\"";    // CSV出力情報：開始日時
                items[10] = $"\"{d.EndDateTime:yyyy/MM/dd HH:mm:ss}\"";     // CSV出力情報：終了日時
                items[11] = $"\"{d.StatusName}\"";                          // CSV出力情報：学習状況

                return items;
            }

            // CSVデータをメモリストリームに書き込む
            bool WriteCsvData(bool changeCourse, bool changeUser, StreamWriter sw)
            {
                var outputCSV = false;

                // 再テスト回数を更新
                if (saveRetrys < scoreList.Count)
                {
                    saveRetrys = scoreList.Count;
                }

                if (changeCourse || changeUser)
                {
                    // コース(講座)、またはユーザが変わった場合
                    progresses.Item1 += (int)Math.Truncate(((double)progress.Item1 / progress.Item2) * 100.0);
                    progresses.Item2++;
                    progress = (0, 0);
                }

                if (changeUser)
                {
                    // 受講者が変わった場合
                    // 進捗率のチェック
                    var p = Math.Truncate(((double)progresses.Item1 / progresses.Item2));
                    if ((minRate <= p) && (maxRate >= p))
                    {
                        OutputTempCsvDataToMemoryStream(sw, tempCsvDataList);
                        if (saveRetrys > maxRetrys )
                        {
                            maxRetrys = saveRetrys;
                        }
                        outputCSV = true;
                    }

                    // コース(講座)毎進捗率算出用変数クリア
                    progresses = (0, 0);

                    // 一時保存CSVデータクリア
                    tempCsvDataList.Clear();

                    saveRetrys = 0;
                }

                return outputCSV;
            }

            // 受講者毎に進捗情報を一時保存する
            void SaveTempCsvDataByStudent(List<string> csvDataList, string[] common, List<string> score)
            {
                // 共通情報取得(キー部分は除く)
                var writeData = string.Join(',', common[4..]);

                if (scoreList.Count > 0)
                {
                    // 採点情報取得し共通情報と併せる
                    writeData = $"{writeData},{string.Join(',', [.. scoreList])}";
                }
                writeData = $"{writeData}\n";

                csvDataList.Add(writeData);
            }

            // 一時保存したCDVデータをメモリストリームへ出力する
            void OutputTempCsvDataToMemoryStream(StreamWriter ws, List<string> tempCsvList)
            {
                foreach (var data in tempCsvList)
                {
                    // メモリストリームへ書き込み
                    sw.Write(data);
                    count++;
                }
            }
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
