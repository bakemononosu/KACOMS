using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("MovieContents")]
    public class MovieContents
    {
        [Key]
        [Column("ContentsId")]
        public Guid ContentsId { get; set; }

        public Guid ChapterId { get; set; }
        [ForeignKey("ChapterId")]
        public MChapter? Chapter { get; set; }

        [Column("ContentsName", TypeName = "nvarchar(128)")]
        public string ContentsName {  get; set; } =string.Empty;

        [Column("ContentsPath",
            TypeName = "nvarchar(256)")]
        public string ContentsPath { get; set; } = string.Empty;

        [Column("PlaybackTime", TypeName = "smallint")]
        public short PlaybackTime { get; set; } = 0;

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
