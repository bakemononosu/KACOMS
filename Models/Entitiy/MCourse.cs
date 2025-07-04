using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("MCourse")]
    public class MCourse
    {
        [Key]
        [Column("CourseId")]
        public Guid CourseId { get; set; }

        [Column("CourseName", TypeName = "nvarchar(64)")]
        public string CourseName { get; set; } = string.Empty;

        [Column("CourseExplaination", TypeName = "nvarchar(256)")]
        public string CourseExplaination { get; set; } = string.Empty;

        [Column("BegineDateTime")]
        public DateTime BegineDateTime { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0);

        [Column("EndDateTime")]
        public DateTime EndDateTime { get; set; } = new DateTime(9999, 12, 31, 23, 59, 59);

        [Column("LearningTime")]
        public int LearningTime { get; set; } = 0;

        [Column("PublicFlg", TypeName = "bit")]
        public bool PublicFlg { get; set; } = true;

        [Column("PrimaryReference", TypeName = "varchar(1)")]
        public string PrimaryReference { get; set; } = "0";

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
