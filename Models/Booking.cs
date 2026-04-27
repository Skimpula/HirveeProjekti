using SQLite;

namespace HirveeProjekti.Models
{
    [Table("varaus")]
    public class Booking
    {
        [PrimaryKey, AutoIncrement]
        [Column("varaus_id")]
        public int VarausId { get; set; }

        [Column("asiakas_id")]
        public int AsiakasId { get; set; }

        [Column("mokki_id")]
        public int MokkiId { get; set; }

        [Column("varattu_pvm")]
        public DateTime VarattuPvm { get; set; }

        [Column("vahvistus_pvm")]
        public DateTime VahvistusPvm { get; set; }

        [Column("varattu_alkupvm")]
        public DateTime VarattuAlkuPvm { get; set; }

        [Column("varattu_loppupvm")]
        public DateTime VarattuLoppuPvm { get; set; }

        // Navigation properties for UI
        [Ignore]
        public Customer? Asiakas { get; set; }

        [Ignore]
        public Cottage? Mokki { get; set; }

        [Ignore]
        public string InvoiceCreationDisplay
        {
            get
            {
                var cottageName = Mokki?.Mokkinimi ?? $"Mokki #{MokkiId}";
                return $"Varaus #{VarausId}: {cottageName} ({VarattuAlkuPvm:dd.MM.yyyy} - {VarattuLoppuPvm:dd.MM.yyyy})";
            }
        }
    }
}