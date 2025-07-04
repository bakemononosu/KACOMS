// /Controllers/ZoomController.cs

using Microsoft.AspNetCore.Mvc;

namespace ElsWebApp.Controllers
{
    public class ZoomController : Controller
    {
        // "/Zoom/Index" にアクセスした際に、ボタンのあるページを表示します
        public IActionResult Index()
        {
            return View();
        }

        // ▼▼▼ このメソッドを追加 ▼▼▼
        // ミーティングに参加するためのメソッド
        public IActionResult JoinMeeting()
        {
            // ここで、参加させたいミーティングの情報を設定します。
            // 将来的にはデータベースなどから取得するようにします。
            const long meetingId = 1234567890; // ★参加したいミーティングID
            const string password = "pAsSwOrD";   // ★ミーティングのパスワード

            // ZoomのWebクライアントを直接開くためのURLを組み立てます
            string zoomJoinUrl = $"https://zoom.us/wc/join/{meetingId}?pwd={password}";

            // 組み立てたZoomのURLにユーザーをリダイレクトさせます
            return Redirect(zoomJoinUrl);
        }
        // ▲▲▲ ここまで ▲▲▲
    }
}