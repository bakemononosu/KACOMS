using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace ElsWebApp.Models
{
    public class TaskStatusViewModel
    {
        public List<TaskStatus>? TaskStatusList { get; set; } // 進捗管理情報
        public string? CompanyName { get; set; } // ログインユーザの法人名
        public string? UserRole { get; set; }// ログインユーザの権限、ロール番号
        public List<SelectListItem>? CoursesList { get; set; } // コースリスト

        //↓ページネーション用--------------------------------------------
        public int AllDataCount { get; set; } // データ全数
        public int ShowLineForPage { get; set; } // 1ページの表示件数

        //↓検索欄情報保持用--------------------------------------------
        public string? UserEnteredName { get; set; }            // 氏名
        public string? UserEnteredCorporateName { get; set; }   // 法人名
        public string? UserEnteredCourse { get; set; }          // コース名
        public int MinRate { get; set; } = 0;                   // 進捗率(開始)
        public int MaxRate { get; set; } = 100;                 // 進捗率(終了)
    }

    public class TaskStatus
    {
        public Guid UserId { get; set; } // ユーザ識別子
        public string LoginId { get; set; } = string.Empty; // ログインID(eメールアドレス)
        public string? UserName { get; set; } // 受講者名
        public string? CourseName { get; set; } // コース名
        public string? ChapterName { get; set; } // 講座名
        public string? CourseId { get; set; } // コースID
        public DateTime? StartDatetime { get; set; } // 受講開始日時
        public DateTime? EndDatetime { get; set; } // 受講終了日時
        public double? ProgressRate { get; set; } // 進捗率
        public string? CompanyName { get; set; }  // 法人名
        public string? Status { get; set; } // 学習状況
    }
}
