using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("UserExam")]
    public class UserExam
    {
        [Key]
        [Column("UserExamId")]
        public Guid UserExamId { get; set; }
    
        public Guid UserChapterId { get; set;}
        [ForeignKey("UserChapterId")]
        public UserChapter? UserChapter { get; set; }

        [Column("NthTime", TypeName = "tinyint")]
        public byte NthTime { get; set; } = 0;

        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public QuestionCatalog? QuestionCatalog { get; set; }

        [Column("DisplayOrder", TypeName = "tinyint")]
        public byte DisplayOrder { get; set; } = 0;

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
