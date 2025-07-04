using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("MUser")]
    public class MUser
    {
        [Key]
        [Column("UserId")]
        public Guid UserId { get; set; }

        [Column("LoginId", TypeName = "varchar(255)")]
        public string LoginId { get; set; } = string.Empty;

        [Column("UserName", TypeName = "nvarchar(32)")]
        public string UserName { get; set; } = string.Empty;

        [Column("CompanyName", TypeName = "nvarchar(128)")]
        public string CompanyName { get; set; } = string.Empty;

        [Column("DepartmentName", TypeName = "nvarchar(128)")]
        public string DepartmentName { get; set; } = string.Empty;

        [Column("Email", TypeName = "varchar(128)")]
        public string Email { get; set; } = string.Empty;

        [Column("EmployeeNo", TypeName = "varchar(16)")]
        public string EmployeeNo { get; set; } = string.Empty;

        [Column("Remarks1", TypeName = "nvarchar(64)")]
        public string Remarks1 { get; set; } = string.Empty;

        [Column("Remarks2", TypeName = "nvarchar(64)")]
        public string Remarks2 { get; set; } = string.Empty;

        [Column("UserRole", TypeName = "varchar(1)")]
        public string UserRole { get; set; } = string.Empty;

        [Column("AvailableFlg", TypeName = "bit")]
        public bool AvailableFlg { get; set; } = false;

        [Column("TempRegisterId", TypeName = "varchar(40)")]
        public string TempRegisterId { get; set; } = string.Empty;

        [Column("DeletedFlg", TypeName = "bit")]
        public bool DeletedFlg { get; set; } = false;

        [Column("UpdatedAt", TypeName = "DateTime")]
        public DateTime? UpdatedAt { get; set; }

        [Column("UpdatedBy")]
        public Guid? UpdatedBy { get; set; }

        [Column("CreatedAt", TypeName = "DateTime")]
        public DateTime? CreatedAt { get; set; }

        [Column("CreatedBy")]
        public Guid? CreatedBy { get; set; }
    }
}
