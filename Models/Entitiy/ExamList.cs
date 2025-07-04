using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    public class ExamList
    {
        [Key]
        [Column("ExamId")]
        public Guid ExamId { get; set; }

        public Guid ContentsId { get; set; }
        [ForeignKey("ContentsId")]
        public TestContents? TestContents { get; set; }

        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public QuestionCatalog? QuestionCatalog { get; set; }

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("CreatedBy")]
        public Guid? CreatedBy { get; set; }
    }
}
