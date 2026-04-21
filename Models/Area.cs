using SQLite;

namespace HirveeProjekti.Models
{
    [Table("alue")]
    public class Area
    {
        [PrimaryKey, AutoIncrement]
        [Column("alue_id")]
        public int AlueId { get; set; }

        [Column("nimi")]
        public string? Nimi { get; set; }
    }
}