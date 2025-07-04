using ElsWebApp.Services;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ElsWebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class LoginController(
        SignInManager<IdentityUser> sInMng,
        IUserService userSvc
        ) : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager = sInMng;
        private readonly IUserService _userService = userSvc;

        /// <summary>
        /// ログイン(管理者)画面表示
        /// </summary>
        /// <returns></returns>
        public IActionResult Admin()
        {
            return Redirect("../../../../Identity/Account/Login?admin=1");
        }

        /// <summary>
        /// ログイン(受講者)画面表示
        /// </summary>
        /// <returns></returns>
        public IActionResult Student()
        {
            return Redirect("../../../../Identity/Account/Login?admin=0");
        }

        /// <summary>
        /// ログアウト
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            var loginId = _signInManager.UserManager.GetUserName(User)?? "";
            var mUser = await this._userService.GetUserByLoginId(loginId);
            if (mUser != null)
            {
                ViewData["Role"] = mUser.UserRole;
            }
            await this._signInManager!.SignOutAsync();
            return View("/Views/Shared/Logout.cshtml");
        }
    }
}
