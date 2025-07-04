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
            { 0, "�A�J�E���g�V�K�o�^" },
            { 1, "�p�X���[�h�ύX" }
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
            [Required(ErrorMessage = "���[���A�h���X����͂��ĉ�����")]
            [EmailAddress(ErrorMessage = "���[���A�h���X��F���ł��܂���")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;
        }

        /// <summary>
        /// �\����ʂœ��͂��ꂽ���[���A�h���X�Ƀ��[���𑗐M����
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            System.Diagnostics.Debug.WriteLine(TempData["Role"]);
            Title = TitleMessages[RequestType];
            if (!ModelState.IsValid)
            {
                // ���̓G���[������ꍇ
                return Page();
            }

            // �g�[�N���̍쐬
            var json = "{\"email\":\"%EMAIL%\", \"limit\":\"%LIMIT%\", \"role\":\"%ROLE%\" }";
            json = json.Replace("%EMAIL%", Input!.Email);
            json = json.Replace("%LIMIT%", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            json = json.Replace("%ROLE%", TempData["Role"]? .ToString()?? ConstService.SystemCode.SYSCODE_USR_USERS);
            var enc = CommonService.EncryptString(json);

            // URL�����N�̍쐬
            var url = string.Empty;
            if (RequestType == 0)
            {
                url = $"{Request.Scheme}://{Request.Host}/Identity/Account/Register/{enc}";
            }
            else
            {
                url = $"{Request.Scheme}://{Request.Host}/Identity/Account/ResetPassword/{enc}";
            }


            // ���[�����M
            await CommonService.SendMail(Input.Email, Input.Email.Split('@')[0], 
                Title, $"�ȉ���URL�ɃA�N�Z�X���A{Title}���s���Ă��������B\n{url}\n �����A��LURL�̗L��������7���Ԃł��B");
            return RedirectToPage("./Announce", new { type = "ConfirmMail", message = Title });
        }

        /// <summary>
        /// �\����ʂ�\������
        /// </summary>
        /// <param name="request">�\�����</param>
        /// <param name="role">���p�ҋ敪</param>
        public void OnGet(int request, string role)
        {
            TempData["Role"] = role;
            RequestType = request; 
            Title = TitleMessages[RequestType];
        }
    }
}
