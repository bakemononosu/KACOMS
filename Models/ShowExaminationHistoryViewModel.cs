namespace ElsWebApp.Models
{
    /// <summary>
    /// テスト実施履歴表示用モデル
    /// </summary>
    public class ShowExaminationHistoryViewModel
    {
        /// <summary>受講者講座識別子</summary>
        public Guid UserChapterId { get; set; } = Guid.Empty;
        /// <summary>ユーザ識別子</summary>
        public Guid UserId { get; set; } = Guid.Empty;
        /// <summary>コース識別子</summary>
        public Guid CourseId { get; set; } = Guid.Empty;
        /// <summary>講座識別子</summary>
        public Guid ChapterId { get; set; } = Guid.Empty;
        /// <summary>テスト実施履歴</summary>
        public List<ExaminationInfo> ExamHistory { get; set; } = [];
        /// <summary>前講座情報</summary>
        public AdjacentContentsInfo PrevChapter { get; set; } = new();
        /// <summary>次講座情報</summary>
        public AdjacentContentsInfo NextChapter { get; set; } = new();
    }

    /// <summary>
    /// テスト実施情報
    /// </summary>
    public class ExaminationInfo
    {
        public int Times { get; set; } = 0;
        public List<QuestionInfo> Questions { get; set; } = [];
        public int CollectCount { get; set; } = 0;
    }
}
