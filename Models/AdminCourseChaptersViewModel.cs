using ElsWebApp.Models.Entitiy;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models
{
    public class AdminCourseChaptersViewModel
    {

        // コース識別子
        public string? CourseId { get; set; }

        // コース名
        public string? CourseName { get; set; }

        // コース説明
        public string? CourseExplaination { get; set; } 

        // 公開期間
        public DateTime? BegineDateTime { get; set; }

        // 公開期間
        public DateTime? EndDateTime { get; set; }

        // 公開フラグ
        public string? PublicFlg { get; set; }

        // 優先公開参照先区分
        public string? PrimaryReference { get; set; }


        // 優先公開参照先区分（公開フラグ）
        public string? PriFlagLabel { get; set; }

        // 優先公開参照先区分（公開期間）
        public string? PriPeriodLabel { get; set; }

        // 公開フラグ選択肢リスト
        public List<SelectListItem>? PublicFlgList { get; set; }

        // 学習コンテンツ区分選択肢リスト
        public List<SelectListItem>? ContentsList { get; set; }


        // 登録講座一覧
        public List<AdminCourseChapter>? ChapterList { get; set; }

        // ユーザコース登録有無
        public int? IsExistsUserCourse { get; set; }
    }

    public class AdminCourseChapter
    {
        // 講座識別子
        public Guid ChapterId { get; set; }

        // 順
        public byte OrderNo { get; set; }

        // 講座名
        public string? ChapterName { get; set; }

        // 学習コンテンツ
        public string? ContentsType { get; set; }

        // 学習時間
        public string? LearningTime { get; set; }
    }

    public class Person
    {
        public string? ChapterId { get; set; }
        public string? OrderNo { get; set; }
    }
}
