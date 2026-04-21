using SQLite;

namespace HirveeProjekti.Models
{
    [Table("asiakas")]
    public class Customer
    {
        [PrimaryKey, AutoIncrement]
        [Column("asiakas_id")]
        public int AsiakasId { get; set; }

        [Column("postinro")]
        public string? Postinro { get; set; }

        [Column("etunimi")]
        public string? Etunimi { get; set; }

        [Column("sukunimi")]
        public string? Sukunimi { get; set; }

        [Column("lahiosoite")]
        public string? Lahiosoite { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("puhelinnro")]
        public string? Puhelinnro { get; set; }

        [Ignore]
        public string FullName => $"{Etunimi} {Sukunimi}".Trim();
    }
}