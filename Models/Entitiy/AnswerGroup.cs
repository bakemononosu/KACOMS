using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("AnswerGroup")]
    public class AnswerGroup
    {
        [Key]
        [Column("AnswerId")]
        public Guid AnswerId { get; set; }

        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public QuestionCatalog? Question { get; set; }

        [Column("AnswerText", TypeName = "nvarchar(1024)")]
        public string? AnswerText { get; set; }

        [Column("AnswerImageName", TypeName = "nvarchar(64)")]
        public string? AnswerImageName { get; set; }

        [Column("AnswerImageData", TypeName = "text")]
        public string? AnswerImageData { get; set; }

        [Column("ExplanationText", TypeName = "nvarchar(1024)")]
        public string ExplanationText { get; set; } = string.Empty;

        [Column("ExplanationImageName", TypeName = "nvarchar(64)")]
        public string? ExplanationImageName { get; set; } = string.Empty;

        [Column("ExplanationImageData", TypeName = "text")]
        public string? ExplanationImageData { get; set; } = string.Empty;

        [Column("ErrataFlg", TypeName = "bit")]
        public bool ErrataFlg { get; set; } = false;

        [Column("OrderNo", TypeName = "tinyint")]
        public byte OrderNo { get; set; }

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
