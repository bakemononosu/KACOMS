/// <summary>
/// システムで共通的に利用する機能を纏めたファイル
/// </summary>
using Azure.Core;
using ElsWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace ElsWebApp.Services
{
    /// <summary>
    /// 共通サービスクラス
    /// </summary>
    public class CommonService
    {
        private const string apiKey = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
        private const string fromAddress = "info-ec@edutechconnect.co.jp";

        public static string MakeQuetionNo(string major, string middle, string miner, string seq) => $"{major}-{middle}-{miner}-{seq}";

        public static string GetValueByCode(List<CodeValuePair> pairList, string code) => pairList.Where(x => x.Code == code).Select(x => x.Value).FirstOrDefault() ?? "";

        public static void CriticalError<T>(ILogger<T> logger, Exception ex) => logger.LogCritical("Message:{message}\nTrace:{trace}", ex.Message, ex.StackTrace);

        /// <summary>
        /// CodeValuePairリストをSelectListItemリストに変換する
        /// </summary>
        /// <param name="pairList">CodeValuePairリスト</param>
        /// <param name="includeCode">変換後のTextにコード値を含めるか否か</param>
        /// <returns></returns>
        public static List<SelectListItem> ConvCVPairListToSIList(List<CodeValuePair> pairList, bool includeCode = false) => pairList.Select(x => new SelectListItem
        {
            Value = x.Code,
            Text = (includeCode) ? $"{x.Code}-{x.Value}" : x.Value
        }).ToList();

        /// <summary>
        /// ハッシュ(SHA1)値を生成する
        /// </summary>
        /// <param name="source">ハッシュ値生成の元となる文字列</param>
        /// <returns>ハッシュ値(16進文字列)</returns>
        public static string CreateHashSHA1(string source)
        {
            var byteString = Encoding.UTF8.GetBytes(source);

            var algo = SHA1.Create();
            var value = algo.ComputeHash(byteString);
            algo.Clear();

            // バイト配列を16進数文字列に変換
            StringBuilder sb = new ();
            foreach (byte b in value)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 指定されたメールアドレスにメールを送信する。
        /// </summary>
        /// <param name="addr">宛先メールアドレス</param>
        /// <param name="name">宛先名</param>
        /// <param name="title">メールタイトル</param>
        /// <param name="mailText">メール内容</param>
        /// <returns></returns>
        public static async Task<bool> SendMail(string addr, string name, string title, string mailText)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress($"{fromAddress}", "eラーニング");
            var subject = title;
            var to = new EmailAddress(addr, name);
            var plainTextContent = mailText;
            var htmlContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

            return true; 
        }

        /// <summary>Key</summary>
        private const string KEY = "DqCCYjcXfBA8EQVV6c4s5LDBrHHYyJSL";
        /// <summary>Initialization Vector</summary>
        private const string IV = "hbqB94EqyJR7eYxa";

        /// <summary>
        /// 文字列の暗号化
        /// </summary>
        /// <param name="planeText">平文</param>
        /// <returns>暗号文</returns>
        public static string EncryptString(string planeText)
        {
            byte[] after;
            using (Aes aes = Aes.Create())
            {
                using ICryptoTransform encripter = aes.CreateEncryptor(Encoding.ASCII.GetBytes(KEY), Encoding.UTF8.GetBytes(IV));

                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encripter, CryptoStreamMode.Write))
                {
                    using var sw = new StreamWriter(cs);
                    sw.Write(planeText);
                }
                after = ms.ToArray();
            }

            return Convert.ToHexString(after);
        }

        /// <summary>
        /// 暗号文の復号化
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public static string DecryptString(string ciphertext)
        {
            var planeText = string.Empty;
            using (Aes aes = Aes.Create())
            {
                using ICryptoTransform decrypter = aes.CreateDecryptor(Encoding.ASCII.GetBytes(KEY), Encoding.ASCII.GetBytes(IV));

                using var ms = new MemoryStream(Convert.FromHexString(ciphertext));
                var cs = new CryptoStream(ms, decrypter, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                planeText = sr.ReadToEnd();
            }

            return planeText;
        }

        /// <summary>
        /// Videoコンテンツを削除する
        /// </summary>
        /// <param name="contentsFilePath">Videoコンテンツファイル(.m3u8)へのパス</param>
        public static void DeleteVideoContents(string contentsFilePath)
        {
            var contentsFolderPath = Path.GetDirectoryName(contentsFilePath);
            if (Directory.Exists(contentsFolderPath))
            {
                Directory.Delete(contentsFolderPath, true);
            }
        }

        /// <summary>
        /// クライアント端末がiOSかどうかチェックする
        /// </summary>
        /// <param name="userAgent">ユーザエージェント</param>
        /// <returns></returns>
        public static bool CheckiOS(string userAgent)
        {
            var pattern = @"iP(hone|(o|a)d)";

            return Regex.IsMatch(userAgent, pattern);
        }
    }
}
