using System.Collections.Generic;
using System.Linq;

namespace HirveeProjekti.Models
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
            return cottages.FirstOrDefault(c => c.MokkiId == id);
        }

        public void Add(Cottage cottage)
        {
            cottage.MokkiId = cottages.Count == 0 ? 1 : cottages.Max(c => c.MokkiId) + 1;
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
            var cottage = GetById(updated.MokkiId);

            if (cottage != null)
            {
                cottage.Mokkinimi = updated.Mokkinimi;
                cottage.Katuosoite = updated.Katuosoite;
                cottage.Hinta = updated.Hinta;
                cottage.Henkilomaara = updated.Henkilomaara;
                cottage.Varustelu = updated.Varustelu;
                cottage.AreaName = updated.AreaName;
            }
        }

        public List<Cottage> Search(string? area = null, int? minPersons = null, double? maxPrice = null)
        {
            var query = cottages.AsQueryable();

            if (!string.IsNullOrEmpty(area))
            {
                query = query.Where(c =>
                    (!string.IsNullOrEmpty(c.AreaName) && c.AreaName.Contains(area)) ||
                    (!string.IsNullOrEmpty(c.Katuosoite) && c.Katuosoite.Contains(area)));
            }

            if (minPersons.HasValue)
            {
                query = query.Where(c => c.Henkilomaara >= minPersons.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Hinta <= maxPrice.Value);
            }

            return query.ToList();
        }
    }
}
