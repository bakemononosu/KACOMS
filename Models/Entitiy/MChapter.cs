using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("MChapter")]
    public class MChapter
    {
        [Key]
        [Column("ChapterId")]
        public Guid ChapterId { get; set; }

        [Column("ChapterName", TypeName = "nvarchar(64)")]
        public string ChapterName { get; set; } = string.Empty;

        [Column("ContentsType", TypeName = "varchar(1)")]
        public string ContentsType { get; set; } = "1";

        public Guid CourseId { get; set; }
        [ForeignKey("CourseId")]
        public MCourse? Course { get; set; }

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
