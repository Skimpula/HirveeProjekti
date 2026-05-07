using SQLite;

namespace HirveeProjekti.Models
{
    [Table("varauksen_palvelut")]
    public class ServiceOfBookings
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("varaus_id")]
        public int VarausId { get; set; }

        [Column("palvelu_id")]
        public int PalveluId { get; set; }

        [Column("lkm")]
        public int Lkm { get; set; }
    }
}

