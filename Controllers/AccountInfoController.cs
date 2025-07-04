using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ElsWebApp.Services;
using System.Data;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyModel.Resolution;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Security.Policy;
using System;

namespace ElsWebApp.Controllers
{
    /// <summary>
    /// 管理者用コントローラークラス
    /// </summary> 
    [AutoValidateAntiforgeryToken]
    public class AccountInfoController(
            IUserService svcUser,
            ILogger<AccountInfoController> logger,
            ElsWebAppDbContext context,
            IWebHostEnvironment env,
            SignInManager<IdentityUser> sInMng,
            IAccountInfoService svcAI,
            ISysCodeService svcSys

        ) : BaseController(logger, context, env, sInMng)
    {
        private List<(int lineNumber, string errorMessage)> errorList = new List<(int lineNumber, string errorMessage)>();
        private readonly IUserService _userService = svcUser;
        private readonly IAccountInfoService _accountInfoService = svcAI;
        private readonly ISysCodeService _sysCodeService = svcSys;

        /// <summary>
        /// アカウント一覧
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> ShowAccounts()
        {

            var userList = await this._accountInfoService.GetAccountList();

            // 削除フラグでの絞り込み
            // 画面での検索表示時は削除済は表示しない
            userList = userList.Where(s => !s.DeletedFlg).ToList();

            var userId = this._loginUser!.UserRole.ToString();

            if (userId == ConstService.SystemCode.SYSCODE_USR_ADMIN)
            {
                userList = (List<MUser>)userList.Where(x => x.UserRole.Contains(ConstService.SystemCode.SYSCODE_USR_CORPO) || x.UserRole.Contains(ConstService.SystemCode.SYSCODE_USR_ADMIN)).ToList();
            }
            else if (userId == ConstService.SystemCode.SYSCODE_USR_CORPO)
            {
                userList = (List<MUser>)userList.Where(x => x.UserRole.Contains(ConstService.SystemCode.SYSCODE_USR_CORPO)).ToList();
            }
            else if (userId == ConstService.SystemCode.SYSCODE_USR_USERS)
            {
                userList = (List<MUser>)userList.Where(x => x.UserRole.Contains(ConstService.SystemCode.SYSCODE_USR_USERS)).ToList();
            }

            if ((this._loginUser?.UserRole ?? "") == ConstService.SystemCode.SYSCODE_USR_CORPO)
            {
                // 代表管理者は、自身の会社のユーザのみ
                userList = userList.Where(x => x.CompanyName == (this._loginUser?.CompanyName ?? "")).ToList();
                var combinedModelSYSCODE_USR_CORPO = new UserInfoViewModel
                {
                    UserList = userList,
                    CompanyName = this._loginUser?.CompanyName,
                    UserRole = this._loginUser?.UserRole,
                    UserEnteredCorporateName = this._loginUser?.CompanyName,
                };
                return View(combinedModelSYSCODE_USR_CORPO);
            }
            var combinedModel = new UserInfoViewModel
            {
                UserList = userList,
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,

            };

            return View(combinedModel);
        }

        [HttpPost] // HTTP POSTリクエストに応答するように指定
        [Authorize]
        public async Task<IActionResult> ShowAccountsPost()
        {
            var userList = await this._accountInfoService.GetAccountList();
            if ((this._loginUser?.UserRole ?? "") == ConstService.SystemCode.SYSCODE_USR_CORPO)
            {
                // 代表管理者は、自身の会社のユーザのみ
                userList = userList.Where(x => x.CompanyName == (this._loginUser?.CompanyName ?? "")).ToList();
            }
            return View(userList);
        }

        /// <summary>
        /// 個別アカウントの表示、新規の時
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        //[Route("/AccountInfo/ShowAdminAccount/")]
        //[Route("/AccountInfo/ShowAdminAccount/{userId?}")]
        public async Task<IActionResult> ShowAdminAccount(
    string UserEnteredEmail, string userId, string UserEnteredName, string UserEnteredCorporateName,
    string UserEnteredDepartment, string UserEnteredEmployeeNumber, string UserEnteredNotes1, string UserEnteredNotes2)
        {
            MUser? user = userId == "undefined" ? null : await this._accountInfoService.GetUser(userId);

            var selectLists = userId == "undefined"
                ? await GetSelectList(this._loginUser?.UserRole ?? "", ConstService.SystemCode.SYSCODE_AVA_YES)
                : await GetSelectList(user?.UserRole ?? "",
                user?.AvailableFlg == true ? ConstService.SystemCode.SYSCODE_AVA_YES : ConstService.SystemCode.SYSCODE_AVA_NO);

            var accountsInfoData = new UserInfoViewModel
            {
                UserEnteredName = UserEnteredName,
                UserEnteredEmail = UserEnteredEmail,
                UserEnteredCorporateName = UserEnteredCorporateName,
                UserEnteredDepartment = UserEnteredDepartment,
                UserEnteredEmployeeNumber = UserEnteredEmployeeNumber,
                UserEnteredNotes1 = UserEnteredNotes1,
                UserEnteredNotes2 = UserEnteredNotes2,
                RolesList = selectLists.rolesList,
                AvailableList = selectLists.availableList,
                UserRole = this._loginUser?.UserRole,
                //UserList = userId == "undefined" ? null : new List<MUser> { user! },
                CompanyName = userId == "undefined" ? null : this._loginUser?.CompanyName,
                SearchDataForMUser = userId == "undefined" ? null : new MUser
                {
                    UserId = user?.UserId ?? Guid.Empty,
                    LoginId = user?.LoginId ?? "",
                    UserName = user?.UserName ?? "",
                    CompanyName = user?.CompanyName ?? "",
                    DepartmentName = user?.DepartmentName ?? "",
                    Email = user?.Email ?? "",
                    EmployeeNo = user?.EmployeeNo ?? "",
                    Remarks1 = user?.Remarks1 ?? "",
                    Remarks2 = user?.Remarks2 ?? "",
                    UserRole = user?.UserRole ?? "",
                    AvailableFlg = user?.AvailableFlg ?? false,
                    TempRegisterId = user?.TempRegisterId ?? "",
                    DeletedFlg = user?.DeletedFlg ?? false,
                    UpdatedAt = user?.UpdatedAt,
                    UpdatedBy = user?.UpdatedBy ?? Guid.Empty,
                    CreatedAt = user?.CreatedAt,
                    CreatedBy = user?.CreatedBy ?? Guid.Empty,
                }
            };

            return View(accountsInfoData);
        }

        /// <summary>
        /// 受講者アカウントの表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> ShowStudentAccount()
        {
            var userId = this._loginUser!.UserId.ToString();
            var user = await this._accountInfoService.GetUser(userId);
            var userUserRole = user.UserRole;
            var userAvailableFlg = "";
            if (user.AvailableFlg)
            {
                userAvailableFlg = ConstService.SystemCode.SYSCODE_USR_CORPO;
            }
            else
            {
                userAvailableFlg = ConstService.SystemCode.SYSCODE_USR_ADMIN;
            }
            var selectLists = await GetSelectList(userUserRole, userAvailableFlg);

            var mUserModel = new MUser()
            {
                UserId = user.UserId,
                LoginId = user.LoginId,
                UserName = user.UserName,
                CompanyName = user.CompanyName,
                DepartmentName = user.DepartmentName,
                Email = user.Email,
                EmployeeNo = user.EmployeeNo,
                Remarks1 = user.Remarks1,
                Remarks2 = user.Remarks2,
                UserRole = user.UserRole,
                AvailableFlg = user.AvailableFlg,
                TempRegisterId = user.TempRegisterId,
                DeletedFlg = user.DeletedFlg,
                UpdatedAt = user.UpdatedAt,
                UpdatedBy = user.UpdatedBy,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
            };

            var userInfoViewModel = new UserInfoViewModel
            {
                UserList = new List<MUser> { user },
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
                RolesList = selectLists.rolesList,
                AvailableList = selectLists.availableList,
                SearchDataForMUser = mUserModel,
                StudentFlg = "true",
            };
            return View(userInfoViewModel);
        }

        /// <summary>
        /// 検索機能
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> SearchAction(
    string userName, string email, string companyName, string employeeNo,
    string remarks1, string remarks2, string departmentName, string format = "html")
        {
            List<MUser> userList = await this._accountInfoService.GetAccountList();

            var userId = this._loginUser!.UserRole.ToString();

            if (userId == ConstService.SystemCode.SYSCODE_USR_ADMIN)
            {
                userList = userList.Where(x => x.UserRole.Contains(ConstService.SystemCode.SYSCODE_USR_CORPO) || x.UserRole.Contains(ConstService.SystemCode.SYSCODE_USR_ADMIN)).ToList();
            }
            else if (userId == ConstService.SystemCode.SYSCODE_USR_CORPO)
            {
                userList = userList.Where(x => x.UserRole.Contains(ConstService.SystemCode.SYSCODE_USR_CORPO)).ToList();
            }
            else if (userId == ConstService.SystemCode.SYSCODE_USR_USERS)
            {
                userList = userList.Where(x => x.UserRole.Contains(ConstService.SystemCode.SYSCODE_USR_USERS)).ToList();
            }

            // 削除フラグでの絞り込み
            // 画面での検索表示時は削除済は表示しない
            if (format != "csv")
            {
                userList = userList.Where(s => !s.DeletedFlg).ToList();
            }

            // 氏名
            if (!string.IsNullOrEmpty(userName))
            {
                userList = userList.Where(s => s.UserName.StartsWith(userName)).ToList();
            }

            // e-mail
            if (!string.IsNullOrEmpty(email))
            {
                userList = userList.Where(s => s.Email.StartsWith(email)).ToList();
            }

            if (userId == ConstService.SystemCode.SYSCODE_USR_CORPO)
            {
                // 法人名
                userList = userList.Where(s => s.CompanyName.StartsWith(this._loginUser.CompanyName)).ToList();
            }
            else
            {
                // 法人名
                if (!string.IsNullOrEmpty(companyName))
                {
                    userList = userList.Where(s => s.CompanyName.StartsWith(companyName)).ToList();
                }
            }

            // 所属部署名
            if (!string.IsNullOrEmpty(departmentName))
            {
                userList = userList.Where(s => s.DepartmentName != null && s.DepartmentName.StartsWith(departmentName)).ToList();
            }

            // 社員番号
            if (!string.IsNullOrEmpty(employeeNo))
            {
                userList = userList.Where(s => s.EmployeeNo != null && s.EmployeeNo.StartsWith(employeeNo)).ToList();
            }

            // 備考1
            if (!string.IsNullOrEmpty(remarks1))
            {
                userList = userList.Where(s => s.Remarks1 != null && s.Remarks1.Contains(remarks1)).ToList();
            }

            // 備考2
            if (!string.IsNullOrEmpty(remarks2))
            {
                userList = userList.Where(s => s.Remarks2 != null && s.Remarks2.Contains(remarks2)).ToList();
            }

            var userInfoViewModel = new UserInfoViewModel
            {
                UserList = userList.ToList(),
                CompanyName = this._loginUser?.CompanyName,
                UserRole = this._loginUser?.UserRole,
                UserEnteredName = userName,
                UserEnteredEmail = email,
                UserEnteredCorporateName = companyName,
                UserEnteredEmployeeNumber = employeeNo,
                UserEnteredNotes1 = remarks1,
                UserEnteredNotes2 = remarks2,
                UserEnteredDepartment = departmentName,
            };

            if (format == "csv")
            {
                return Json(userInfoViewModel); // JSON形式で返す
            }
            else
            {
                return View("ShowAccounts", userInfoViewModel); // View形式で返す
            }
        }

        /// <summary>
        /// CSVダウンロード用機能、絞り込み
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<List<MUser>> SearchActionCSV(
    string userName, string email, string companyName, string employeeNo,
    string remarks1, string remarks2, string departmentName)
        {
            var result = await SearchAction(
                userName, email, companyName, employeeNo, remarks1, remarks2, departmentName, "csv"
            ) as JsonResult;

            if (result?.Value is UserInfoViewModel userInfo)
            {
                return userInfo.UserList!.ToList(); // 必要な形式に変換
            }

            return new List<MUser>(); // 何も返ってこなかった場合に空リストを返す
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
                result = await this._accountInfoService.UpdateAvailableFlg(userId, availableFlg, this._loginUser!.UserId);
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
            }

            return (result) ? Ok() : BadRequest();
        }

        /// <summary>
        /// 新規データ登録機能
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> InsertAccount(//InsertAccount
            string loginId, string userName, string companyName, string departmentName,
            string employeeNo, string remarks1, string remarks2, string passwordChangeRequest,
            bool availableFlg, string userRole
            )
        {
            var result = 0;
            try
            {
                var MUser = new MUser
                {
                    Email = loginId,
                    UserName = userName,
                    CompanyName = companyName,
                    DepartmentName = departmentName ?? string.Empty,
                    EmployeeNo = employeeNo ?? string.Empty,
                    Remarks1 = remarks1 ?? string.Empty,
                    Remarks2 = remarks2 ?? string.Empty,
                    AvailableFlg = availableFlg,
                    UserRole = userRole,
                    LoginId = loginId,
                    CreatedBy = this._loginUser?.UserId
                };

                result = await this._userService.Insert(MUser);
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
            }
            return (result > 0) ? Ok() : BadRequest();
        }

        /// <summary>
        /// 既存データ更新機能
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateAccount(
            string loginId, string userName, string companyName, string departmentName, Guid userId,
            string employeeNo, string remarks1, string remarks2, bool passwordChangeRequest,
            bool availableFlg, string userRole, string tempRegisterId, string studentFlg
            )
        {
            var result = 0;
            try
            {
                Guid fixUserId = studentFlg == "true" ? this._loginUser!.UserId : userId;
                var MUser = new MUser
                {
                    LoginId = loginId,
                    UserId = fixUserId,
                    UserName = userName,
                    CompanyName = companyName,
                    DepartmentName = departmentName ?? string.Empty,
                    EmployeeNo = employeeNo ?? string.Empty,
                    Remarks1 = remarks1 ?? string.Empty,
                    Remarks2 = remarks2 ?? string.Empty,
                    AvailableFlg = availableFlg,
                    UserRole = userRole,
                    UpdatedBy = this._loginUser?.UserId
                };

                //if (this._loginUser!.UserRole == ConstService.SystemCode.SYSCODE_USR_ADMIN && studentFlg != "true")
                //{
                //    MUser.CompanyName = companyName;
                //}
             
                result = await this._accountInfoService.Update(MUser, this._loginUser!.UserRole, studentFlg);
                if (passwordChangeRequest)
                {
                    // トークンの作成
                    var json = "{\"email\":\"%EMAIL%\", \"limit\":\"%LIMIT%\", \"role\":\"%ROLE%\" }";
                    json = json.Replace("%EMAIL%", loginId);
                    json = json.Replace("%LIMIT%", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    json = json.Replace("%ROLE%", userRole);
                    var enc = CommonService.EncryptString(json);
                    var title = "パスワード変更";
                    var url = $"{Request.Scheme}://{Request.Host}/Identity/Account/ResetPassword/{enc}";

                    //---------------------------------------------------------------------------チェックが入っていた場合仮登録識別子を更新↑
                    //await CommonService.SendMail(loginId, userName,
                    //    title, $"以下のURLにアクセスし、{title}を行ってください。\n{url}\n ※尚、上記URLの有効期限は7日間です。");
                }
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
                return BadRequest();
            }
            return (result >= 0) ? Ok() : BadRequest();
        }

        /// <summary>
        /// 削除フラグ更新
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> DeleteAccount(string userId)
        {
            var result = false;
            try
            {
                result = await this._accountInfoService.UpdateDeleteFlg(userId, this._loginUser!.UserId);
            }
            catch (Exception ex)
            {
                var combineErrMsg = $"Message: {ex.Message} Trace: {ex.StackTrace}";
                logger.LogError(combineErrMsg);
            }
            return (result) ? Ok() : BadRequest();
        }

        // 検索条件SelectList整形
        private async Task<(List<SelectListItem> rolesList, List<SelectListItem> availableList)> GetSelectList(string selectedrole = "", string selectedAvailable = "")
        {
            // 利用者区分
            var sysRolesCode = await _sysCodeService.GetClassCodeList(ConstService.SystemCode.SYSCODE_CLASS_USER);

            var filteredCodes = sysRolesCode;

            if (selectedrole != ConstService.SystemCode.SYSCODE_USR_USERS)
            {
                filteredCodes = filteredCodes
                    .Where(code => code.Code == ConstService.SystemCode.SYSCODE_USR_ADMIN ||
                                   code.Code == ConstService.SystemCode.SYSCODE_USR_CORPO)
                    .ToList();
            }
            else
            {
                filteredCodes = filteredCodes
                    .Where(code => code.Code == ConstService.SystemCode.SYSCODE_USR_USERS)
                    .ToList();
            }

            var rolesList = filteredCodes.Select(code => new SelectListItem
            {
                Value = code.Code,
                Text = code.Value,
                Selected = code.Code == selectedrole
            }).ToList();

            if (this._loginUser?.UserRole == ConstService.SystemCode.SYSCODE_USR_ADMIN)
            {
                rolesList = rolesList.Where(x => x.Value.Contains(ConstService.SystemCode.SYSCODE_USR_ADMIN) || x.Value.Contains(ConstService.SystemCode.SYSCODE_USR_CORPO)).ToList();
            }
            else if (this._loginUser?.UserRole == ConstService.SystemCode.SYSCODE_USR_CORPO)
            {
                rolesList = rolesList.Where(x => x.Value.Contains(ConstService.SystemCode.SYSCODE_USR_CORPO)).ToList();
            }
            else if (this._loginUser?.UserRole == ConstService.SystemCode.SYSCODE_USR_USERS)
            {
                rolesList = rolesList.Where(x => x.Value.Contains(ConstService.SystemCode.SYSCODE_USR_USERS)).ToList();
            }

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

        ////---------------------------------------------------------------------------------------------------------------↓CSV一括処理関係
        /// <summary>
        /// 一括更新画面表示
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult TransitionBulkUpload()
        {
            ViewBag.ControllerName = "AccountInfo";
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
                        // エラーの場合は項目チェックスキップ
                        if (values.Length != ConstService.CsvUpload.CSV_NUMBER_OF_ENTRIES)
                        {
                            errorList.Add((loop, "項目数が規格と一致しません。"));
                            loop++;
                            continue;
                        }

                        // 項目チェック
                        // アカウント管理では「利用者区分：9 （受講者）」以外対象
                        if (values[ConstService.CsvUpload.CSV_COL_NAME_USER_CLASSIFICATION] != ConstService.SystemCode.SYSCODE_USR_USERS)
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

                            // アカウント管理では「利用者区分：9 （受講者）」以外対象
                            if (splitVals[ConstService.CsvUpload.CSV_COL_NAME_USER_CLASSIFICATION] != ConstService.SystemCode.SYSCODE_USR_USERS)
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
                        data.CreatedBy = this._loginUser!.UserId;
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
                        data.UpdatedBy = this._loginUser!.UserId;
                        result = await this._accountInfoService.Update(data, this._loginUser!.UserRole);
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
                        if (await this._accountInfoService.UpdateDeleteFlg(data.UserId.ToString(), this._loginUser!.UserId))
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
        ////---------------------------------------------------------------------------------------------------------------↑CSV一括処理関係
    }
}


