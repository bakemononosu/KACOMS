using ElsWebApp.Services;

namespace ElsWebApp.Models
{
    public class DownloadCsvData
    {
        /// <summary>ユーザID</summary>
        public Guid UserId { get; set; } = Guid.Empty;
        /// <summary>受講者名</summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>メールアドレス</summary>
        public string EMail { get; set; } = string.Empty;
        /// <summary>コースID(講座ID)</summary>
        public Guid? CourseId { get; set; } = Guid.Empty;
        /// <summary>コース名(講座名)</summary>
        public string? CourseName { get; set; } = string.Empty;
        /// <summary>講座ID(セクションID)</summary>
        public Guid? ChapterId { get; set; } = Guid.Empty;
        /// <summary>講座名(セクション名)</summary>
        public string? ChapterName { get; set; } = string.Empty;
        /// <summary>コンテンツ種別</summary>
        public string? ContentsType { get; set; } = string.Empty;
        /// <summary>コンテンツ種別</summary>
        public string? ContentsTypeName { get; set; } = string.Empty;
        /// <summary>実施回数</summary>
        public byte? NthTime { get; set; } = 0;
        /// <summary>学習開始日時</summary>
        public DateTime? StartDateTime { get; set; }
        /// <summary>学習終了日時</summary>
        public DateTime? EndDateTime { get; set; }
        /// <summary>状態区分</summary>
        public string? Status { get; set; } = string.Empty;
        /// <summary>状態</summary>
        public string? StatusName { get; set; } = ConstService.SystemCode.SYSCODE_STS_WAITING;
        /// <summary>出題数</summary>
        public int? Total { get; set; } = 0;
        /// <summary>正解数</summary>
        public int? CollectAnswers { get; set; } = 0;
    }
}
