using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("UserCourse")]
    public class UserCourse
    {
        [Key]
        [Column("UserCourseId")]
        public Guid UserCourseId { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public MUser? MUser { get; set; }

        public Guid CourseId { get; set; }
        [ForeignKey("CourseId")]
        public MCourse? MCourse { get; set; }

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
