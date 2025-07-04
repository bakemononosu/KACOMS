using ElsWebApp.Controllers;
using ElsWebApp.Models.Entitiy;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ElsWebApp.Models
{
    public class AdminCoursesViewModel
    {
        // コース一覧
        public List<AdminCourse>? AdminCourseList { get; set; }
        // 優先参照先リスト
        public required List<SelectListItem> PrimaryReferenceList { get; set; }
        // 検索文字列
        public string? SearchWord { get; set; }

    }

    public class AdminCourse
    {

        // 講座ID
        public Guid CourseId { get; set; }
        // 講座名
        public string? CourseName { get; set; }
        // 公開期間
        public string? PublicPeriod { get; set; }
        // 講座数
        public int? ChapterCnt { get; set; }
        // 学習時間
        public string? StudyTime { get; set; }
        // 公開フラグ
        public int? PublicFlg { get; set; }
        // 優先参照先
        public string? PrimaryReference { get; set; }
        // 公開中フラグ
        public int? IsPublic { get; set; }
        // 有効期間フラグ
        public int? IsInAvalablePeriod { get; set; }
        // ユーザコース登録有無
        public int? IsExistsUserCourse { get; set; }
    }
}
