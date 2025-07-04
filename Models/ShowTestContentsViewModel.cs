namespace ElsWebApp.Models
{
    public class ShowTestContentsViewModel
    {
        public bool IsDisplayMode { get; set; } = false;
        public Guid UserChapterId { get; set; } = Guid.Empty;
        public Guid UserId { get; set; } = Guid.Empty;
        public Guid CourseId { get; set; } = Guid.Empty;
        public Guid ChapterId { get; set; } = Guid.Empty;
        public int Times { get; set; } = 0;
        public int QuestionCount { get; set; } = 0;
        public int CollectCount { get; set; } = 0;
        public int LimitLime { get; set; } = 0;
        public List<QuestionInfo> Questions { get; set; } = [];
        public string ErrorMessage { get; set; } = string.Empty;
        public AdjacentContentsInfo PrevChapter { get; set; } = new();
        public AdjacentContentsInfo NextChapter { get; set; } = new();
    }
    public class QuestionInfo
    {
        public Guid QId { get; set; } = Guid.Empty;
        public string QText { get; set; } = string.Empty;
        public string QImage { get; set; } = string.Empty;
        public List<AnswerInfo> Answers { get; set; } = [];
    }

    public class AnswerInfo
    {
        public Guid AId { get; set; } = Guid.Empty;
        public string AText { get; set; } = string.Empty;
        public string AImage { get; set; } = string.Empty;
        public string EText { get; set; } = string.Empty;
        public bool AValue { get; set; } = false;
        public string Status { get; set; } = string.Empty;
    }

    public class AdjacentContentsInfo
    {
        public Guid ChapterId {  get; set; } = Guid.Empty;
        public string ContentsType {  get; set; } = string.Empty;
        public bool IsDelete { get; set; } = false;
    }
}
