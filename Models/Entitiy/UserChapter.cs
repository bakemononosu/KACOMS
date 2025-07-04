using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("UserChapter")]
    public class UserChapter
    {
        [Key]
        [Column("UserChapterId")]
        public Guid UserChapterId { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public MUser? MUser { get; set; }

        [Column("CourseId")]
        public Guid CourseId { get; set; }

        public Guid ChapterId { get; set; }
        [ForeignKey("ChapterId")]
        public MChapter? MChapter { get; set; }

        [Column("Status")]
        public string Status { get; set; } = "0";

        [Column("StartDatetime")]
        public DateTime? StartDatetime { get; set; }

        [Column("EndDatetime")]
        public DateTime? EndDatetime { get; set; }

        [Column("BookmarkSeconds")]
        public int? BookmarkSeconds { get; set; }

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
