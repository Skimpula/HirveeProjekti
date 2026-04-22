using SQLite;

namespace HirveeProjekti.Models
{
    [Table("palvelu")]
    public class Service
    {
        [PrimaryKey, AutoIncrement]
        [Column("palvelu_id")]
        public int PalveluId { get; set; }

        [Column("alue_id")]
        public int AlueId { get; set; }

        [Column("nimi")]
        public string? Nimi { get; set; }

        [Column("kuvaus")]
        public string? Kuvaus { get; set; }

        [Column("hinta")]
        public double Hinta { get; set; }

        [Column("alv")]
        public double Alv { get; set; }
    }
}