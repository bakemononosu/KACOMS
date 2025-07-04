using ElsWebApp.Controllers;
using ElsWebApp.Models.Entitiy;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models
{
    //public class CombinedViewModel
    public class AdminQuestionsViewModel
    {
        public List<QuestionCatalog>? QuestionList { get; set; } //問題内容_更新系、登録系で使用
        public List<CustomQuestionCatalog>? CustomQuestionCatalog { get; set; } //問題内容[一覧]
        public CustomQuestionCatalog? QuestionCatalogData { get; set; } //問題内容[詳細]

        public List<AnswerGroup>? AnswerGroup { get; set; } //解答内容
        public List<SelectListItem>? ClassMajor { get; set; }//大分類、コムボックス用
        public List<SelectListItem>? ClassMiddle { get; set; }//中分類、コムボックス用
        public List<SelectListItem>? ClassMinor { get; set; }//小分類、コムボックス用
        public List<SelectListItem>? ClassLevel { get; set; }//難易度、コムボックス用
        public List<SelectListItem>? ClassQuestionType { get; set; }//難易度、コムボックス用



        //↓検索欄情報保持用----------------------------------------------------------------------------------------------
        public string? UserEnteredchoiceClassMajor { get; set; }//検索条件
        public string? UserEnteredchoiceClassMiddle { get; set; }//検索条件
        public string? UserEnteredchoiceClassMinor { get; set; }//検索条件       
    }

    public class CustomQuestionCatalog
    {
        public Guid QuestionId { get; set; }
        public string MajorCd { get; set; } = "0";

        public string MiddleCd { get; set; } = "00";

        public string MinorCd { get; set; } = "00";

        public string SeqNo { get; set; } = "00000";

        public string QuestionTitle { get; set; } = string.Empty;

        public string? QuestionText { get; set; }

        public string? QuestionImageName { get; set; }

        public string? QuestionImageData { get; set; }

        public string QuestionType { get; set; } = "1";

        public string Level { get; set; } = "5";

        public byte Score { get; set; } = 0;

        public bool DeletedFlg { get; set; } = false;

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public int? UsedFlg { get; set; }//ぶら下がるユーザが存在するかどうかのフラグ/1=存在する_0=存在しない

        public bool FixedOrder { get; set; } = false;
    }
 }
