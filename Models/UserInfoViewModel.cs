using ElsWebApp.Controllers;
using ElsWebApp.Models.Entitiy;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ElsWebApp.Models
{
    //public class CombinedViewModel
    public class UserInfoViewModel
    {
        public List<MUser>? UserList { get; set; }
        public string? CompanyName { get; set; } // ログインユーザの法人名
        public string? UserRole { get; set; }// ログインユーザの権限、ロール番号
        public List<SelectListItem>? RolesList { get; set; } // 利用者区分リスト
        public List<SelectListItem>? AvailableList { get; set; } // 受講可否フラグリスト
        public string? StudentFlg { get; set; } // 受講者情報画面用フラグ、項目の非表示を制御

        //↓検索欄情報保持用----------------------------------------------------------------------------------------------
        public string? UserEnteredName { get; set; }　// 氏名
        public string? UserEnteredEmail { get; set; }// e-mail
        public string? UserEnteredCorporateName { get; set; }// 法人名
        public string? UserEnteredEmployeeNumber { get; set; }// 社員番号
        public string? UserEnteredNotes1 { get; set; }// 備考-1
        public string? UserEnteredNotes2 { get; set; }// 備考-2
        public string? UserEnteredManagementGroup { get; set; }// 管理グループ
        public string? UserEnteredAvailability { get; set; }// 利用可否
        public string? UserEnteredDepartment { get; set; }// 所属部署名        
        //------------------------------------------------------------------------------------------------------------------
        public MUser? SearchDataForMUser { get; set; }

    }
}
