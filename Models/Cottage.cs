using SQLite;

namespace HirveeProjekti.Models
{
    [Table("mokki")]
    public class Cottage
    {
        [PrimaryKey, AutoIncrement]
        [Column("mokki_id")]
        public int MokkiId { get; set; }

        [Column("alue_id")]
        public int AlueId { get; set; }

        [Column("postinro")]
        public string? Postinro { get; set; }

        [Column("mokkinimi")]
        public string? Mokkinimi { get; set; }

        [Column("katuosoite")]
        public string? Katuosoite { get; set; }

        [Column("hinta")]
        public double Hinta { get; set; }

        [Column("kuvaus")]
        public string? Kuvaus { get; set; }

        [Column("henkilomaara")]
        public int Henkilomaara { get; set; }

        [Column("varustelu")]
        public string? Varustelu { get; set; }

        [Ignore]
        public string? AreaName { get; set; }
    }
}