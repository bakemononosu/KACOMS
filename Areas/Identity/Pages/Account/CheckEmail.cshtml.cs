using Azure.Core;
using ElsWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace ElsWebApp.Areas.Identity.Pages.Account
{
    public class CheckEmailModel : PageModel
    {
        private Dictionary<int, string> TitleMessages = new Dictionary<int, string>()
        {
            { 0, "アカウント新規登録" },
            { 1, "パスワード変更" }
        };

        public string Title { get; set; } = string.Empty;

        [BindProperty]
        public int RequestType { get; set; }

        [BindProperty]
        public InputModel? Input { get; set; }

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "メールアドレスを入力して下さい")]
            [EmailAddress(ErrorMessage = "メールアドレスを認識できません")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;
        }

        /// <summary>
        /// 申請画面で入力されたメールアドレスにメールを送信する
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            System.Diagnostics.Debug.WriteLine(TempData["Role"]);
            Title = TitleMessages[RequestType];
            if (!ModelState.IsValid)
            {
                // 入力エラーがある場合
                return Page();
            }

            // トークンの作成
            var json = "{\"email\":\"%EMAIL%\", \"limit\":\"%LIMIT%\", \"role\":\"%ROLE%\" }";
            json = json.Replace("%EMAIL%", Input!.Email);
            json = json.Replace("%LIMIT%", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            json = json.Replace("%ROLE%", TempData["Role"]? .ToString()?? ConstService.SystemCode.SYSCODE_USR_USERS);
            var enc = CommonService.EncryptString(json);

            // URLリンクの作成
            var url = string.Empty;
            if (RequestType == 0)
            {
                url = $"{Request.Scheme}://{Request.Host}/Identity/Account/Register/{enc}";
            }
            else
            {
                url = $"{Request.Scheme}://{Request.Host}/Identity/Account/ResetPassword/{enc}";
            }


            // メール送信
            await CommonService.SendMail(Input.Email, Input.Email.Split('@')[0], 
                Title, $"以下のURLにアクセスし、{Title}を行ってください。\n{url}\n ※尚、上記URLの有効期限は7日間です。");
            return RedirectToPage("./Announce", new { type = "ConfirmMail", message = Title });
        }

        /// <summary>
        /// 申請画面を表示する
        /// </summary>
        /// <param name="request">申請種類</param>
        /// <param name="role">利用者区分</param>
        public void OnGet(int request, string role)
        {
            TempData["Role"] = role;
            RequestType = request; 
            Title = TitleMessages[RequestType];
        }
    }
}
