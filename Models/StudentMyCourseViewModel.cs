using ElsWebApp.Controllers;
using ElsWebApp.Models.Entitiy;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ElsWebApp.Models
{
    public class StudentMyCourseViewModel
    {

        // ユーザID
        public string? UserId { get; set; }

        // コースID
        public string? CourseId { get; set; }

        // コース名
        public string? CourseName { get; set; }

        // コース一覧
        public List<MyCourse>? MyCourseList { get; set; }

        // 講座一覧
        public List<MyChapter>? MyChapterList { get; set; }

        // コース講座受講
        public ShowVideoContent? ShowVideoContent { get; set; }

        // 動画再生可否フラグ
        public bool IsPlayNotAvailable { get; set; } = false;

        public bool IsIOs { get; set; } = false;

        public ConfirmExamination? ConfirmExaminationData { get; set; }
    }

    public class MyCourse
    {
        // コース名
        public string? CourseName { get; set; }
        // コースID
        public string? CourseId { get; set; }
        // 受講開始日
        public DateTime? StartDatetime { get; set; }
        // 受講終了日
        public DateTime? EndDatetime { get; set; }
        // 学習状況
        public string? Status { get; set; }
        // 進捗率
        public double? ProgressRate { get; set; }
        // ユーザーID
        public string? UserId { get; set; }
    }

    public class MyChapter
    {

        // 学習順序
        public byte? OrderNo { get; set; }
        // 講座ID
        public string? ChapterId { get; set; }
        // 講座名
        public string? ChapterName { get; set; }
        // 学習コンテンツ区分
        public string? ContentsType { get; set; }
        // 学習状況
        public string? Status { get; set; }
        // 学習状況（表示名）
        public string? StatusName { get; set; }
        // 受講開始
        public DateTime? StartDatetime { get; set; }
        // 受講終了
        public DateTime? EndDatetime { get; set; }
        // 削除フラグ
        public bool DeletedFlg { get; set; } = false;
    }

    public class ShowVideoContent
    {
        // ユーザID
        public string? UserId { get; set; }
        // コースID
        public string? CourseId { get; set; }
        // 講座ID
        public string? ChapterId { get; set; }
        // 最初のコンテンツ
        public bool IsFirstContent { get; set; } = false;
        // 最後のコンテンツ
        public bool IsLastContent { get; set; } = false;
        // 動画コンテンツ情報
        public MovieContents? MovieContents { get; set; }
        // 現在の学習順序
        public int CurrentOrderNo { get; set; } = 0;

    }

    public class ConfirmExamination
    {
        public string? UserId { get; set; }
        public string? ChapterName { get; set; }
        public string? CourseId { get; set; }
        public string? ChapterId { get; set; }

        //問題実施ボタン活性非活性制御用Flg
        public string? NoDataFlg { get; set; }

        //過去問題参照ボタン活性非活性制御用Flg
        public bool? ExaminationHistoryFlg { get; set; }
    }
}

