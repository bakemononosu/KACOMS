using ElsWebApp.Controllers;
using ElsWebApp.Models.Entitiy;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ElsWebApp.Models
{
    public class StudentCoursesViewModel
    {

        // コース一覧
        public List<StudentCourse>? CourseList { get; set; }

        // コースID
        public string? CourseId { get; set; }

        // 講座一覧
        public List<Chapters>? ChapterList { get; set; }

    }
    
    
    public class StudentCourse
    {

        // 講座ID
        public Guid CourseId { get; set; }
        // 講座名
        public string? CourseName { get; set; }
        // 受講開始
        public DateTime? BegineDateTime { get; set; }
        // 受講終了
        public DateTime? EndDateTime { get; set; }
        // コース説明
        public string? CourseExplaination { get; set; }
        // 削除フラグ
        //public bool DeletedFlg { get; set; } = false;
        // 公開フラグ
        public bool PublicFlg { get; set; } = false;
        // 公開フラグ（表示名称）
        public string? PublicName { get; set; }

        // 利用可能フラグ（再受講できるか）
        public bool AvailableFlg { get; set; } = false;
        // 受講中フラグ
        public bool LearningFlag { get; set; } = false;

    }

    public class Chapters
    {

        // 学習順序
        public byte? OrderNo { get; set; }
        // 講座名
        public string? ChapterName { get; set; }
        // 学習コンテンツ区分
        public string? ContentsType { get; set; }
        // 動画再生時間
        public short? PlaybackTime { get; set; }
        // 制限時間
        public short? LimitTime { get; set; }
    }

}