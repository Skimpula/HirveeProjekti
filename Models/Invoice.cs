using SQLite;

namespace HirveeProjekti.Models
{
    //lasku
    [Table("lasku")]
    public class Invoice
    {
        [PrimaryKey, AutoIncrement]
        [Column("lasku_id")]
        public int LaskuId { get; set; }
        
        [Column("varaus_id")]
        public int VarausId { get; set; }
        
        [Column("summa")]
        public double Summa { get; set; }
        
        [Column("alv")]
        public double Alv { get; set; }
        
        [Column("maksettu")]
        public bool Maksettu { get; set; }
    }
}
