using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("QuestionCatalog")]
    public class QuestionCatalog
    {
        [Key]
        [Column("QuestionId")]
        public Guid QuestionId { get; set; }

        [Column("MajorCd", TypeName = "varchar(1)")]
        public string MajorCd { get; set; } = "0";

        [Column("MiddleCd", TypeName = "varchar(2)")]
        public string MiddleCd { get; set; } = "00";

        [Column("MinorCd", TypeName = "varchar(2)")]
        public string MinorCd { get; set; } = "00";

        [Column("SeqNo", TypeName = "varchar(5)")]
        public string SeqNo { get; set; } = "00000";

        [Column("QuestionTitle", TypeName = "nvarchar(256)")]
        public string QuestionTitle { get; set; } = string.Empty;

        [Column("QuestionText", TypeName = "nvarchar(1024)")]
        public string? QuestionText { get; set; }

        [Column("QuestionImageName", TypeName = "nvarchar(64)")]
        public string? QuestionImageName { get; set; }

        [Column("QuestionImageData", TypeName = "text")]
        public string? QuestionImageData { get; set; }

        [Column("QuestionType", TypeName = "varchar(1)")]
        public string QuestionType { get; set; } = "1";

        [Column("Level", TypeName = "varchar(1)")]
        public string Level { get; set; } = "5";

        [Column("Score", TypeName = "tinyint")]
        public byte Score { get; set; } = 0;

        [Column("FixedOrder", TypeName = "bit")]
        public bool FixedOrder { get; set; } = false;

        [Column("DeletedFlg", TypeName = "bit")]
        public bool DeletedFlg { get; set; } = false;

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("UpdatedBy")]
        public Guid? UpdatedBy { get; set; }

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("CreatedBy")]
        public Guid? CreatedBy { get; set; }
    }
}
