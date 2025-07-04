using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("TestContents")]
    public class TestContents
    {
        [Key]
        [Column("ContentsId")]
        public Guid ContentsId { get; set; }

        public Guid ChapterId { get; set; }
        [ForeignKey("ChapterId")]
        public MChapter? Chapter { get; set; }

        [Column("ContentsName", TypeName = "nvarchar(128)")]
        public string ContentsName { get; set; } = string.Empty;

        [Column("Questions", TypeName = "tinyint")]
        public byte Questions { get; set; } = 0;

        [Column("LimitTime", TypeName = "smallint")]
        public short LimitTime { get; set; } = 0;

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
