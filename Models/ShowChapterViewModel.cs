using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ElsWebApp.Models
{
    public class ShowChapterViewModel
    {
        public Guid CourseId { get; set; }
        public Guid ChapterId { get; set; }
        public Guid ContentsId { get; set; }
        public string ChapterName { get; set; } = string.Empty;
        public string ContentsType { get; set; } = ConstService.SystemCode.SYSCODE_CON_MOVIE;
        public int OrderNo { get; set; }
        public VideoContents VideoOld { get; set; } = new();
        public VideoContents VideoNew { get; set; } = new();
        public ExamContents Exam { get; set; } = new();
        public List<SelectListItem> MajorList { get; set; } = [];
        public List<SelectListItem> MiddleList { get; set; } = [];
        public List<SelectListItem> MinorList { get; set; } = [];
        public int IsExistsUserCourse { get; set; }
    }

    public class VideoContents
    {
        public string ContentsName { get; set; } = string.Empty;
        public string ContentsPath { get; set;} = string.Empty;
        public int PlaybackTime { get; set; } = 0;
    }

    public class ExamContents
    {
        public string ContentsName { get; set; } = string.Empty;
        public int Questions { get; set; } = 0;
        public int LimitTime { get; set; } = 0;
        public List<QuestionData> QuestionList { get; set; } = [];
    }

    public class QuestionData
    {
        public string QId { get; set; } = string.Empty;
        public string QNo { get; set; } = string.Empty;
        public string QTitle { get; set; } = string.Empty;
        public string QType { get; set; } = string.Empty;
        public string QLevel { get; set; } = string.Empty;
        public int QScore { get; set; } = 0;
    }

    public class QAnswerData
    {
        public string QNo { get; set; } = string.Empty;
        public string QTitle { get; set; } = string.Empty;
        public string QText { get; set; } = string.Empty;
        public string QImage { get; set; } = string.Empty;
        public List<AnswerData> AList { get; set; } = [];
    }

    public class AnswerData
    {
        public string AnswerId { get;set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
        public string AnswerImage { get; set; } = string.Empty;
        public string ExplanationText { get; set; } = string.Empty;
        public bool ErrataFlg { get; set; } = false;
    }
}
