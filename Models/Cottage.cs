namespace HirveeProjekti.Models
{
    public class Cottage
    {
        public int MokkiId { get; set; }
        public int AlueId { get; set; }
        public string Mokkinimi { get; set; }
        public string Katuosoite { get; set; }
        public double Hinta { get; set; }
        public int Henkilomaara { get; set; }
        public string Varustelu { get; set; }
        public string AreaName { get; set; } // Lisätty UI:ta varten
    }
}