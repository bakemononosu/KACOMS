using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("UserScore")]
    public class UserScore
    {
        [Key]
        [Column("UserScoreId")]
        public Guid UserScoreId { get; set; }

        public Guid UserChapterId { get; set; }
        [ForeignKey("UserChapterId")]
        public UserChapter? UserChapter { get; set; }

        [Column("NthTime", TypeName = "tinyint")]
        public byte NthTime { get; set; } = 0;

        public Guid QuestionId { get; set; }

        public Guid AnswerId { get; set; }
        [ForeignKey("AnswerId")]
        public AnswerGroup? AnswerGroup { get; set; }

        [Column("DisplayOrder", TypeName = "tinyint")]
        public byte DisplayOrder { get; set; } = 0;

        [Column("AnswerValue", TypeName = "bit")]
        public bool AnswerValue { get; set; } = false;

        [Column("Result", TypeName = "tinyint")]
        public byte Result { get; set; } = 0;

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
