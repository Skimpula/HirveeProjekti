using System.Collections.Generic;

namespace HirveeProjekti.Cottages
{
    public class Cottage
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public decimal PricePerDay { get; set; }

        public List<string> Amenities { get; set; } = new List<string>();
    }
}
