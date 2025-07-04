using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElsWebApp.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json.Linq;
using NLog.Filters;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;

namespace ElsWebApp.Controllers
{
    /// <summary>
    /// 管理者用コントローラークラス
    /// </summary>
    [AutoValidateAntiforgeryToken]
    public class AdminStudentsController(
            ILogger<AdminStudentsController> logger,
            ElsWebAppDbContext context,
            IWebHostEnvironment env,
            SignInManager<IdentityUser> sInMng,
            IAdminStudentsService svcAds,
            IUserService svcUser,
            ISysCodeService svcSys
        ) : BaseController(logger, context, env, sInMng)
    {
        private IAdminStudentsService _adminStudentsService = svcAds;
        private readonly IUserService _userService = svcUser;
        private readonly ISysCodeService _sysCodeService = svcSys;
        private readonly List<(int lineNumber, string errorMessage)> errorList = [];

        /// <summary>
        /// 受講者管理
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> ShowStudents()
        {
            List<MUser> studentList = await this._adminStudentsService.GetStudentList();

            // 削除フラグでの絞り込み
            // 画面での検索表示時は削除済は表示しない
            studentList = studentList.Where(s => !s.DeletedFlg).ToList();

            // 初期表示は受講可否フラグ ＝ "1"(受講可)のみ
            studentList = studentList.Where(s => s.AvailableFlg == true).ToList();
            if ((this._loginUser?.UserRole ?? "") == ConstService.SystemCode.SYSCODE_AVA_YES)
            {
                // ログインユーザが帆人管理者の場合
                // 法人名にログインユーザの法人名を表示(入力不可)
                studentList = studentList.Where(x => x.CompanyName == (this._loginUser?.CompanyName ?? "")).ToList();
            }

            // 並べ替え（法人名、利用者氏名）
            studentList = [.. studentList.OrderBy(x => x.CompanyName).ThenBy(x => x.UserName)];

            // SelectList取得
            var (rolesList, availableList) = await GetSelectList(ConstService.SystemCode.SYSCODE_USR_USERS, "");

            var userInfoViewModel = new UserInfoViewModel
            {
                UserList = studentList,
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
                RolesList = rolesList,
                AvailableList = availableList,
                UserEnteredManagementGroup = ConstService.SystemCode.SYSCODE_USR_USERS,
                UserEnteredAvailability = ConstService.SystemCode.SYSCODE_AVA_YES
            };
            return View(userInfoViewModel);
        }

        /// <summary>
        /// 受講者検索
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> SearchStudents(
            string userName, string email, string companyName, string departmentName,
            string employeeNo, string remarks1, string remarks2, string userRole, string available
            )
        {
            companyName = this._loginUser!.UserRole == ConstService.SystemCode.SYSCODE_USR_CORPO ? this._loginUser!.CompanyName : companyName;

            List<MUser> studentList = await FilterSearchConditions(
                userName, email, companyName, departmentName, employeeNo, remarks1, remarks2, userRole, available
            );

            var selectLists = await GetSelectList(userRole, available);
            var userInfoViewModel = new UserInfoViewModel
            {
                UserList = studentList,
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
                RolesList = selectLists.rolesList,
                AvailableList = selectLists.availableList,
                UserEnteredName = userName,
                UserEnteredEmail = email,
                UserEnteredCorporateName = companyName,
                UserEnteredDepartment = departmentName,
                UserEnteredEmployeeNumber = employeeNo,
                UserEnteredNotes1 = remarks1,
                UserEnteredNotes2 = remarks2,
                UserEnteredManagementGroup = userRole,
                UserEnteredAvailability = available,
            };
            return View("ShowStudents", userInfoViewModel);
        }

        // 検索条件でのフィルタ
        private async Task<List<MUser>> FilterSearchConditions(
            string userName, string email, string companyName, string departmentName,
            string employeeNo, string remarks1, string remarks2, string userRole, string available, bool isCsvDownLoad = false
            )
        {
            List<MUser> studentList = await this._adminStudentsService.GetStudentList();

            // 削除フラグでの絞り込み
            // 画面での検索表示時は削除済は表示しない
            if(!isCsvDownLoad)
            {
                studentList = studentList.Where(s => !s.DeletedFlg).ToList();
            }

            // 氏名
            if (!string.IsNullOrEmpty(userName))
            {
                studentList = studentList.Where(s => s.UserName.StartsWith(userName)).ToList();
            }

            // e-mail
            if (!string.IsNullOrEmpty(email))
            {
                studentList = studentList.Where(s => s.Email.StartsWith(email)).ToList();
            }

            // 法人名
            if (!string.IsNullOrEmpty(companyName))
            {
                studentList = studentList.Where(s => s.CompanyName.StartsWith(companyName)).ToList();
            }

            // 所属部署名
            if (!string.IsNullOrEmpty(departmentName))
            {
                studentList = studentList.Where(s => s.DepartmentName!.StartsWith(departmentName)).ToList();
            }

            // 社員番号
            if (!string.IsNullOrEmpty(employeeNo))
            {
                studentList = studentList.Where(s => s.EmployeeNo!.StartsWith(employeeNo)).ToList();
            }

            // 備考1
            if (!string.IsNullOrEmpty(remarks1))
            {
                studentList = studentList.Where(s => s.Remarks1!.Contains(remarks1)).ToList();
            }

            // 備考2
            if (!string.IsNullOrEmpty(remarks2))
            {
                studentList = studentList.Where(s => s.Remarks2!.Contains(remarks2)).ToList();
            }

            // 管理グループ
            if (!string.IsNullOrEmpty(userRole))
            {
                studentList = studentList.Where(s => s.UserRole == userRole).ToList();
            }

            // 利用可否
            if (!string.IsNullOrEmpty(available))
            {
                bool availableFlg = available == ConstService.SystemCode.SYSCODE_AVA_YES ? true : false;
                studentList = studentList.Where(s => s.AvailableFlg == availableFlg).ToList();
            }

            if (studentList.Count > 0)
            {
                // 並べ替え（法人名、利用者氏名）
                studentList = [.. studentList.OrderBy(x => x.CompanyName).ThenBy(x => x.UserName)];
            }

            return studentList;
        }

        // 検索条件SelectList整形
        private async Task<(List<SelectListItem> rolesList, List<SelectListItem> availableList)> GetSelectList(string selectedrole = "", string selectedAvailable = "")
        {
            // 利用者区分
            var sysRolesCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_USER);
            var rolesList = sysRolesCode.Select(code => new SelectListItem
            {
                Value = code.Code,
                Text = code.Value,
                Selected = code.Code == selectedrole
            }).Where(x => x.Value == ConstService.SystemCode.SYSCODE_USR_USERS).ToList();

            // 受講可否フラグ
            var sysAvailableCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_AVAILABLE);
            var availableList = sysAvailableCode.Select(code => new SelectListItem
            {
                Value = code.Code,
                Text = code.Value,
                Selected = code.Code == selectedAvailable
            }).ToList();

            return (rolesList, availableList);
        }

        /// <summary>
        /// 受講可否フラグ更新
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ToggleAvailable(string userId, bool swichInput)
        {
            var result = false;
            try
            {
                var availableFlg = swichInput;
                result = await this._adminStudentsService.UpdateAvailableFlg(userId, availableFlg, _loginUser!.UserId); ;
            }
            catch (Exception ex)
            {
                logger.LogError("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);
            }

            return (result) ? Ok() : BadRequest();

        }

        /// <summary>
        /// 削除フラグ更新
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> DeleteStudent(string userId)
        {
            var result = false;
            try
            {
                result = await this._adminStudentsService.UpdateDeleteFlg(userId, _loginUser!.UserId);
            }
            catch (Exception ex)
            {
                logger.LogError("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);
            }

            return (result) ? Ok() : BadRequest();
        }

        /// <summary>
        /// CSVダウンロード用機能、絞り込み
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<List<MUser>> SearchActionCSV(
            string userName, string email, string companyName, string employeeNo,
            string remarks1, string remarks2, string departmentName, string userRole, string available)
        {
            List<MUser> studentList = await FilterSearchConditions(
                userName, email, companyName, departmentName, employeeNo, remarks1, remarks2, userRole, available, true
            );

            return studentList;
        }

        /// <summary>
        /// 個別新規登録
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("/AdminStudents/ShowIndividualNew")]
        public async Task<IActionResult> ShowIndividualNew(string enteredEmail, string enteredUserName, string enteredCompanyName, string enteredDepartmentName,
            string enteredEmployeeNo, string enteredRemarks1, string enteredRemarks2, string enteredAvailableFlg, string enteredUserRole)
        {
            var selectLists = await GetSelectList(ConstService.SystemCode.SYSCODE_USR_USERS, string.Empty);
            var loginUserRole = this._loginUser?.UserRole;
            var loginUserCompanyName = this._loginUser?.CompanyName;
            bool isLoginCorpUser = loginUserRole == ConstService.SystemCode.SYSCODE_USR_CORPO;

            var userInfoViewModel = new UserInfoViewModel
            {
                UserList = null,
                SearchDataForMUser = new MUser(),
                CompanyName = loginUserCompanyName,
                UserRole = loginUserRole,
                RolesList = selectLists.rolesList,
                AvailableList = selectLists.availableList,
                UserEnteredEmail = enteredEmail,
                UserEnteredName = enteredUserName,
                UserEnteredCorporateName = isLoginCorpUser ? loginUserCompanyName : enteredCompanyName,
                UserEnteredDepartment = enteredDepartmentName,
                UserEnteredEmployeeNumber = enteredEmployeeNo,
                UserEnteredNotes1 = enteredRemarks1,
                UserEnteredNotes2 = enteredRemarks2,
                UserEnteredManagementGroup = enteredUserRole,
                UserEnteredAvailability = enteredAvailableFlg
            };

            if (isLoginCorpUser && !string.IsNullOrEmpty(loginUserCompanyName))
            {
                userInfoViewModel.SearchDataForMUser.CompanyName = loginUserCompanyName;
            }
            return View("ShowIndividual", userInfoViewModel);
        }

        /// <summary>
        /// 個別更新
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("/AdminStudents/ShowIndividual")]
        public async Task<IActionResult> ShowIndividual(string userId, string enteredEmail, string enteredUserName, string enteredCompanyName, string enteredDepartmentName,
            string enteredEmployeeNo, string enteredRemarks1, string enteredRemarks2, string enteredAvailableFlg, string enteredUserRole)
        {
            var user = await this._adminStudentsService.GetUser(userId);
            var available = user.AvailableFlg ? ConstService.SystemCode.SYSCODE_AVA_YES : ConstService.SystemCode.SYSCODE_AVA_NO;
            var selectLists = await GetSelectList(user.UserRole, available);
            var loginUserRole = this._loginUser?.UserRole;
            var loginUserCompanyName = this._loginUser?.CompanyName;

            var userInfoViewModel = new UserInfoViewModel
            {
                UserList = null,
                SearchDataForMUser = user,
                CompanyName = loginUserCompanyName,
                UserRole = loginUserRole,
                RolesList = selectLists.rolesList,
                AvailableList = selectLists.availableList,
                UserEnteredEmail = enteredEmail,
                UserEnteredName = enteredUserName,
                UserEnteredCorporateName = loginUserRole == ConstService.SystemCode.SYSCODE_USR_CORPO ? loginUserCompanyName : enteredCompanyName,
                UserEnteredDepartment = enteredDepartmentName,
                UserEnteredEmployeeNumber = enteredEmployeeNo,
                UserEnteredNotes1 = enteredRemarks1,
                UserEnteredNotes2 = enteredRemarks2,
                UserEnteredManagementGroup = enteredUserRole,
                UserEnteredAvailability = enteredAvailableFlg
            };
            return View("ShowIndividual", userInfoViewModel);
        }

        /// <summary>
        /// 新規データ登録機能
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> InsertStudent(
            string email, string userName, string companyName, string departmentName,
            string employeeNo, string remarks1, string remarks2, bool availableFlg, string userRole
            )
        {
            // 既に登録済のユーザID（メールアドレス）
            var user = await this._userService.GetUserByLoginId(email);
            if (!string.IsNullOrEmpty(user.LoginId)) {
                return BadRequest(new { errData = new { errorMessage = "既に登録済のユーザID（メールアドレス）です" } });
            }

            var result = 0;
            try
            {
                var MUser = new MUser
                {
                    Email = email,
                    UserName = userName,
                    CompanyName = companyName,
                    DepartmentName = departmentName ?? string.Empty,
                    EmployeeNo = employeeNo ?? string.Empty,
                    Remarks1 = remarks1 ?? string.Empty,
                    Remarks2 = remarks2 ?? string.Empty,
                    AvailableFlg = availableFlg,
                    UserRole = userRole,
                    LoginId = email,
                    CreatedBy = _loginUser!.UserId
                };

                result = await this._userService.Insert(MUser);
            }
            catch (Exception ex)
            {
                logger.LogError("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);
            }

            return (result > 0) ? Ok() : BadRequest(new { errData = new[] { new { errorMessage = "更新に失敗しました" } } });
        }

        /// <summary>
        /// 既存データ更新機能
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateStudent(
            string email, string userName, string companyName, string departmentName, Guid userId,
            string employeeNo, string remarks1, string remarks2, bool passwordChangeRequest,
            bool availableFlg, string userRole
            )
        {
            var result = 0;
            try
            {
                var mUser = new MUser
                {
                    Email = email,
                    UserId = userId,
                    UserName = userName,
                    CompanyName = companyName,
                    DepartmentName = departmentName ?? string.Empty,
                    EmployeeNo = employeeNo ?? string.Empty,
                    Remarks1 = remarks1 ?? string.Empty,
                    Remarks2 = remarks2 ?? string.Empty,
                    AvailableFlg = availableFlg,
                    UserRole = userRole,
                    LoginId = email,
                    UpdatedBy = _loginUser!.UserId
                };

                result = await this._adminStudentsService.UpdateForAdminStudent(mUser);
                if (passwordChangeRequest)
                {
                    // トークンの作成
                    var json = "{\"email\":\"%EMAIL%\", \"limit\":\"%LIMIT%\", \"role\":\"%ROLE%\" }";
                    json = json.Replace("%EMAIL%", mUser.LoginId);
                    json = json.Replace("%LIMIT%", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    json = json.Replace("%ROLE%", userRole);
                    var enc = CommonService.EncryptString(json);
                    var title = "パスワード変更";
                    var url = $"{Request.Scheme}://{Request.Host}/Identity/Account/ResetPassword/{enc}";

                    //---------------------------------------------------------------------------チェックが入っていた場合仮登録識別子を更新↑
                    //await CommonService.SendMail(mUser.LoginId, userName,
                    //    title, $"以下のURLにアクセスし、{title}を行ってください。\n{url}\n ※尚、上記URLの有効期限は30分です。");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);
            }

            return (result > -1) ? Ok() : BadRequest();
        }


        /// <summary>
        /// 一括更新画面表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult TransitionBulkUpload()
        {
            ViewBag.ControllerName = "AdminStudents";
            return View("../Shared/BulkUpload");
        }

        /// <summary>
        /// アップロードファイルValidation
        /// </summary>
        /// <returns></returns>
        public IActionResult ValidateCsv(IFormFile fileData)
        {
            if (fileData == null || fileData.Length <= 0)
            {
                return BadRequest(new { errData = new[] { new { lineNumber = "", errorMessage = "CSVデータがありません。" } } });
            }

            try
            {
                using (var reader = new StreamReader(fileData.OpenReadStream(), Encoding.GetEncoding("Shift_JIS")))
                {
                    var loop = 0;
                    while (!reader.EndOfStream)
                    {
                        Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        var line = reader.ReadLine();
                        // ""で囲まれたものは分割しない
                        var values = reg.Split(line!);

                        if (values == null)
                        {
                            return BadRequest(new { errData = new[] { new { lineNumber = "", errorMessage = "CSVデータがありません。" } } });
                        }

                        // ヘッダー行は無視
                        if (loop == 0)
                        {
                            loop++;
                            continue;
                        }

                        // 項目数は各行チェック
                        // 本来は受講者管理では「利用者区分：9 （受講者）」のみ対象だが、項目がずれていると正しくチェック出来ない為、利用者区分のチェック前に実施
                        // エラーの場合は項目チェックスキップ
                        if (values.Length != ConstService.CsvUpload.CSV_NUMBER_OF_ENTRIES)
                        {
                            errorList.Add((loop, "項目数が規格と一致しません。"));
                            loop++;
                            continue;
                        }

                        // 項目チェック
                        // 受講者管理では「利用者区分：9 （受講者）」のみ対象
                        if (values[ConstService.CsvUpload.CSV_COL_NAME_USER_CLASSIFICATION] != ConstService.SystemCode.SYSCODE_USR_ADMIN
                                && values[ConstService.CsvUpload.CSV_COL_NAME_USER_CLASSIFICATION] != ConstService.SystemCode.SYSCODE_USR_CORPO)
                        {

                            Validate(values, loop);
                        }
                        loop++;
                    }
                }
                if (errorList.Count > 0)
                {
                    return BadRequest(new { errData = errorList.Select(e => new { e.lineNumber, e.errorMessage }) });
                }

                DateTime now = DateTime.Now;
                string formattedDateTime = now.ToString("yyyyMMddHHmmss");
                string userId = this._loginUser?.UserId.ToString() ?? "UserIdIsUndefined";
                string wwwrootPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "csv", "upload");
                var fileName = Path.Combine(wwwrootPath, $"user_{userId}_{formattedDateTime}.csv");

                // wwwrootディレクトリが存在しない場合は作成する
                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }

                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    // ファイルをwwwrootに保存
                    fileData.CopyTo(stream);
                }

                return Ok(new { errData = "", tempFileName = fileName });
            }
            catch (Exception ex)
            {
                logger.LogError("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);
                return BadRequest(new { errData = new[] { new { lineNumber = "", errorMessage = "CSVファイルチェック処置で予期せぬエラーが発生しました。" } } });
            }
        }

        // CSVファイルデータ確認
        private void Validate(string[] values, int index)
        {
            // アクション
            string[] action = ["", ConstService.CsvUpload.UPLOAD_ACT_INSERT, ConstService.CsvUpload.UPLOAD_ACT_UPDATA, ConstService.CsvUpload.UPLOAD_ACT_DELETE];
            if (!action.Contains(values[ConstService.CsvUpload.CSV_COL_NAME_ACTION]))
            {
                errorList.Add((index, "アクションが「空 / I / U / D」以外です。"));
            }

            // ユーザ識別子
            if ((values[ConstService.CsvUpload.CSV_COL_NAME_ACTION] == ConstService.CsvUpload.UPLOAD_ACT_UPDATA || values[ConstService.CsvUpload.CSV_COL_NAME_ACTION] == ConstService.CsvUpload.UPLOAD_ACT_DELETE)
                    && string.IsNullOrEmpty(values[ConstService.CsvUpload.CSV_COL_NAME_USER_IDENTIFIER]))
            {
                errorList.Add((index, "ユーザ識別子が未入力です。"));
            }

            // 利用者ID
            if (string.IsNullOrEmpty(values[ConstService.CsvUpload.CSV_COL_NAME_USER_ID]) || values[ConstService.CsvUpload.CSV_COL_NAME_USER_ID].Length > 255)
            {
                errorList.Add((index, "利用者IDが未入力または長すぎます。"));
            }
            else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(values[ConstService.CsvUpload.CSV_COL_NAME_USER_ID]);
                }
                catch
                {
                    errorList.Add((index, "利用者IDに利用できない文字が使われています。"));
                }
            }

            // 利用者名
            if (string.IsNullOrEmpty(values[ConstService.CsvUpload.CSV_COL_NAME_USER_NAME]) || values[ConstService.CsvUpload.CSV_COL_NAME_USER_NAME].Length > 32)
            {
                errorList.Add((index, "利用者名が未入力または長すぎます。"));
            }

            // 法人名
            if (string.IsNullOrEmpty(values[ConstService.CsvUpload.CSV_COL_NAME_CORPORATE_NAME]) || values[ConstService.CsvUpload.CSV_COL_NAME_CORPORATE_NAME].Length > 128)
            {
                errorList.Add((index, "法人名が未入力または長すぎます。"));
            }

            // 所属部署名
            if (values[ConstService.CsvUpload.CSV_COL_NAME_DEPARTMENT_NAME].Length > 128)
            {
                errorList.Add((index, "所属部署名が長すぎます。"));
            }

            // メールアドレス
            if (string.IsNullOrEmpty(values[ConstService.CsvUpload.CSV_COL_NAME_EMAIL_ADDRESS]) || values[ConstService.CsvUpload.CSV_COL_NAME_EMAIL_ADDRESS].Length > 255)
            {
                errorList.Add((index, "メールアドレスが未入力または長すぎます。"));
            }
            else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(values[ConstService.CsvUpload.CSV_COL_NAME_EMAIL_ADDRESS]);
                }
                catch
                {
                    errorList.Add((index, "メールアドレスに利用できない文字が使われています。"));
                }
            }

            // 社員番号
            if (values[ConstService.CsvUpload.CSV_COL_NAME_EMPLOYEE_NUMBER].Length > 16)
            {
                errorList.Add((index, "社員番号が長すぎます。"));
            }

            // 備考１
            if (values[ConstService.CsvUpload.CSV_COL_NAME_NOTE1].Length > 64)
            {
                errorList.Add((index, "備考１が長すぎます。"));
            }

            // 備考２
            if (values[ConstService.CsvUpload.CSV_COL_NAME_NOTE2].Length > 64)
            {
                errorList.Add((index, "備考２が長すぎます。"));
            }

            // 利用者区分
            string[] category = [ConstService.SystemCode.SYSCODE_USR_ADMIN, ConstService.SystemCode.SYSCODE_USR_CORPO, ConstService.SystemCode.SYSCODE_USR_USERS];
            if (!category.Contains(values[ConstService.CsvUpload.CSV_COL_NAME_USER_CLASSIFICATION]))
            {
                errorList.Add((index, "利用者区分が未入力、または無効な値です。"));
            }

            // 受講可否フラグ
            string[] available = [ConstService.SystemCode.SYSCODE_AVA_NO, ConstService.SystemCode.SYSCODE_AVA_YES];
            if (!available.Contains(values[ConstService.CsvUpload.CSV_COL_NAME_AVAILABLE_FLG]))
            {
                errorList.Add((index, "受講可否フラグが未入力、または無効な値です。"));
            }

            // 削除フラグ
            string[] deleted = [ConstService.SystemCode.SYSCODE_DEL_NO, ConstService.SystemCode.SYSCODE_DEL_YES];
            if (!deleted.Contains(values[ConstService.CsvUpload.CSV_COL_NAME_DELETED_FLG]))
            {
                errorList.Add((index, "削除フラグが未入力、または無効な値です。"));
            }
        }

        /// <summary>
        /// 一括更新
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BulkUpload(string tempFileName)
        {
            bool isError = false;
            Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            using (var reader = new StreamReader(tempFileName, Encoding.GetEncoding("shift_jis")))
            {
                var loop = 0;
                try
                {
                    while (!reader.EndOfStream)
                    {
                        // 行読込
                        var line = reader.ReadLine();
                        // ヘッダー行は無視
                        if (loop == 0)
                        {
                            loop++;
                            continue;
                        }

                        // 各行の処理を行う
                        if (line != null)
                        {
                            // ""で囲まれたものは分割しない
                            var splitVals = reg.Split(line!);

                            // 受講者管理では「利用者区分：9 （受講者）」のみ対象
                            if (splitVals[ConstService.CsvUpload.CSV_COL_NAME_USER_CLASSIFICATION] == ConstService.SystemCode.SYSCODE_USR_USERS)
                            {
                                // 行データをMUser型に成形
                                MUser data = ConvertLineDataToModel(splitVals);

                                string action = splitVals[ConstService.CsvUpload.CSV_COL_NAME_ACTION];
                                // ユーザの存在チェック
                                bool isExist = await CheckUserExist(data, action);
                                // 登録・更新・削除処理
                                var result = await SwitchRegist(data, action, loop, isExist);
                                if (result == 0)
                                {
                                    // エラーフラグをOn
                                    isError = true;
                                    break;

                                }
                            }
                        }
                        loop++;
                    }
                }
                catch (Exception ex)
                {
                    // 一時ファイルを削除
                    System.IO.File.Delete(tempFileName);

                    // エラーメッセージ
                    var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                    errorList.Add((loop, $"Message: {ex.Message} Trace: {ex.StackTrace}"));

                    logger.LogError("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

                    // エラーフラグをOn
                    isError = true;
                }

            }

            if (isError)
            {
                return BadRequest(new { errData = errorList.Select(e => new { e.lineNumber, e.errorMessage }) });
            }
            else
            {
                return Ok(new { errData = "" });
            }
        }

        // Muserコンバーター
        private static MUser ConvertLineDataToModel(string[] lineData)
        {
            bool isUpdate = false;
            if (!string.IsNullOrEmpty(lineData[ConstService.CsvUpload.CSV_COL_NAME_ACTION]) && lineData[ConstService.CsvUpload.CSV_COL_NAME_ACTION] != ConstService.CsvUpload.UPLOAD_ACT_INSERT)
            {
                isUpdate = true;
            }
            var userData = new MUser();
            if (lineData.Length == ConstService.CsvUpload.CSV_NUMBER_OF_ENTRIES)
            {
                if (isUpdate) userData.UserId = new Guid(lineData[ConstService.CsvUpload.CSV_COL_NAME_USER_IDENTIFIER]);
                userData.LoginId = lineData[ConstService.CsvUpload.CSV_COL_NAME_USER_ID];
                userData.UserName = lineData[ConstService.CsvUpload.CSV_COL_NAME_USER_NAME];
                userData.CompanyName = lineData[ConstService.CsvUpload.CSV_COL_NAME_CORPORATE_NAME];
                userData.DepartmentName = lineData[ConstService.CsvUpload.CSV_COL_NAME_DEPARTMENT_NAME];
                userData.Email = lineData[ConstService.CsvUpload.CSV_COL_NAME_EMAIL_ADDRESS];
                userData.EmployeeNo = lineData[ConstService.CsvUpload.CSV_COL_NAME_EMPLOYEE_NUMBER];
                userData.Remarks1 = lineData[ConstService.CsvUpload.CSV_COL_NAME_NOTE1];
                userData.Remarks2 = lineData[ConstService.CsvUpload.CSV_COL_NAME_NOTE2];
                userData.UserRole = lineData[ConstService.CsvUpload.CSV_COL_NAME_USER_CLASSIFICATION];
                userData.AvailableFlg = lineData[ConstService.CsvUpload.CSV_COL_NAME_AVAILABLE_FLG] == ConstService.SystemCode.SYSCODE_AVA_YES;
                userData.DeletedFlg = lineData[ConstService.CsvUpload.CSV_COL_NAME_DELETED_FLG] == ConstService.SystemCode.SYSCODE_DEL_YES;
            }

            return userData;
        }

        // 利用者IDの存在確認
        private async Task<bool> CheckUserExist(MUser data, string action)
        {
            bool isExist = false;
            if (action == ConstService.CsvUpload.UPLOAD_ACT_INSERT)
            {
                MUser user = await this._userService.GetUserByLoginId(data.LoginId);
                isExist = !string.IsNullOrEmpty(user.LoginId);
            }

            if (action == ConstService.CsvUpload.UPLOAD_ACT_UPDATA || action == ConstService.CsvUpload.UPLOAD_ACT_DELETE)
            {
                MUser user = await this._userService.SelectById(data.UserId.ToString());
                isExist = !string.IsNullOrEmpty(user.LoginId);
            }

            return isExist;
        }

        // 登録、更新、およびエラーメッセージの返却
        private async Task<int> SwitchRegist(MUser data, string action, int index, bool isExist)
        {
            try
            {
                var result = 0;
                switch (action)
                {
                    case ConstService.CsvUpload.UPLOAD_ACT_INSERT:
                        if (isExist)
                        {
                            errorList.Add((index, "既に存在するユーザです。"));
                            return 0;
                        }

                        data.CreatedBy = _loginUser!.UserId;
                        result = await this._userService.Insert(data);
                        if (result == 0)
                        {
                            errorList.Add((index, "新規登録処理に失敗しました。"));
                            return 0;
                        }
                        else
                        {
                            return result;
                        }
                    case ConstService.CsvUpload.UPLOAD_ACT_UPDATA:
                        if (!isExist)
                        {
                            errorList.Add((index, "存在しないユーザです。"));
                            return 0;
                        }

                        data.UpdatedBy = _loginUser!.UserId;
                        result = await this._adminStudentsService.UpdateForAdminStudent(data);
                        if (result > -1)
                        {
                            return 1;
                        }
                        else
                        {
                            errorList.Add((index, "更新処理に失敗しました。"));
                            return 0;
                        }
                    case ConstService.CsvUpload.UPLOAD_ACT_DELETE:
                        if (await this._adminStudentsService.UpdateDeleteFlg(data.UserId.ToString(), _loginUser!.UserId))
                        {
                            return 1;
                        }
                        else
                        {
                            errorList.Add((index, "削除処理に失敗しました。"));
                            return 0;
                        }
                    case "":
                        return 1;
                    default:
                        errorList.Add((index, "アクションが「空,I, U, D以外」"));
                        return 0;
                }
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                errorList.Add((index, combineErrMsg));

                logger.LogError("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);
                return 0;
            }
        }
    }
}
