namespace HirveeProjekti.Models
{
    public class Booking
    {
        public int VarausId { get; set; }
        public Customer Asiakas { get; set; }
        public Cottage Mokki { get; set; }
        public DateTime VarattuPvm { get; set; }
        public DateTime VarattuAlkuPvm { get; set; }
        public DateTime VarattuLoppuPvm { get; set; }
    }
}