using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace ElsWebApp.Models
{
    public class TestStatusViewModel
    {
        public List<CourseTestStatus>? CourseTestStatusList { get; set; } // 得点情報
        public List<CourseStudentTestStatus>? CourseStudentTestStatusList { get; set; } // 得点情報
        public List<string>? ScoresHeader { get; set; } // 動的得点のヘッダー項目
        public string? CompanyName { get; set; } // ログインユーザの法人名
        public string? UserRole { get; set; }// ログインユーザの権限、ロール番号
        public List<SelectListItem>? CoursesList { get; set; } // コースリスト

        //↓検索欄情報保持用--------------------------------------------
        public string? UserEnteredCorporateName { get; set; }// 法人名
        public string? UserEnteredCourse { get; set; }// コース名

        //↓氏名押下時の対象受講者ID--------------------------------------------
        public string? SelectedUserID { get; set; }　// 受講者
    }

    public class CourseTestStatus
    {
        public string? UserName { get; set; }
        public Guid? UserId { get; set; }
        public string? CompanyName { get; set; }
        public string? CourseName { get; set; }
        public string? ChapterName { get; set; }
        public required Dictionary<string, string?> Scores { get; set; }
    }

    public class CourseStudentTestStatus
    {
        public string? UserName { get; set; }
        public Guid? UserId { get; set; }
        public string? CompanyName { get; set; }
        public string? CourseName { get; set; }
        public Guid? ChapterId { get; set; }
        public string? ChapterName { get; set; }
        public string? 得点 { get; set; }
        public string? 実施日時 { get; set; }
    }
}
