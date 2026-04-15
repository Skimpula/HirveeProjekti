using System.Collections.Generic;
using System.Linq;

namespace HirveeProjekti.Cottages
{
    public class CottageData
    {
        private readonly List<Cottage> cottages = new List<Cottage>();

        public List<Cottage> GetAll()
        {
            return cottages;
        }

        public Cottage? GetById(int id)
        {
            return cottages.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Cottage cottage)
        {
            cottage.Id = cottages.Count + 1;
            cottages.Add(cottage);
        }

        public void Delete(int id)
        {
            var cottage = GetById(id);
            if (cottage != null)
            {
                cottages.Remove(cottage);
            }
        }

        public void Update(Cottage updated)
        {
            var cottage = GetById(updated.Id);

            if (cottage != null)
            {
                cottage.Name = updated.Name;
                cottage.Address = updated.Address;
                cottage.Description = updated.Description;
                cottage.Capacity = updated.Capacity;
                cottage.PricePerDay = updated.PricePerDay;
                cottage.Amenities = updated.Amenities;
            }
        }

        public List<Cottage> Search(string? area = null, int? minPersons = null, decimal? maxPrice = null)
        {
            var query = cottages.AsQueryable();

            if (!string.IsNullOrEmpty(area))
            {
                query = query.Where(c => c.Address.Contains(area));
            }

            if (minPersons.HasValue)
            {
                query = query.Where(c => c.Capacity >= minPersons.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.PricePerDay <= maxPrice.Value);
            }

            return query.ToList();
        }
    }
}
