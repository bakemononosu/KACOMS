using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsWebApp.Models.Entitiy
{
    [Table("MSysCode")]
    public class MSysCode
    {
        [Column("ClassId", TypeName = "varchar(2)")]
        public string ClassId { get; set; } = string.Empty;

        [Column("ClassCd", TypeName = "varchar(2)")]
        public string ClassCd { get; set; } = string.Empty;

        [Column("ClassName", TypeName = "nvarchar(64)")]
        public string ClassName { get; set; } = string.Empty;

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; }
    }
}
