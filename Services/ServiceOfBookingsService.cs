using HirveeProjekti.Models;
namespace HirveeProjekti.Services
{
    public class ServiceOfBookingsService
    {
        private readonly List<ServiceOfBookings> items = new List<ServiceOfBookings>();

        // Palauttaa kaikki varauksen palvelut
        public List<ServiceOfBookings> GetAll()
        {
            return items;
        }

        // Palauttaa yhden varauksen palvelut
        public List<ServiceOfBookings> GetByVarausId(int varausId)
        {
            return items
                .Where(x => x.VarausId == varausId)
                .ToList();
        }

        // Lisää palvelu varaukseen
        // Jos sama palvelu on jo lisätty, kasvatetaan määrää
        public void Add(ServiceOfBookings item)
        {
            var existing = items.FirstOrDefault(x =>
                x.VarausId == item.VarausId &&
                x.PalveluId == item.PalveluId);

            if (existing != null)
            {
                existing.Lkm += item.Lkm;
            }
            else
            {
                items.Add(item);
            }
        }

        // Päivitä palvelun määrä varauksessa
        public void UpdateLkm(int varausId, int palveluId, int newLkm)
        {
            var item = items.FirstOrDefault(x =>
                x.VarausId == varausId &&
                x.PalveluId == palveluId);

            if (item != null)
            {
                item.Lkm = newLkm;
            }
        }

        // Poista yksi palvelu varauksesta
        public void Delete(int varausId, int palveluId)
        {
            var item = items.FirstOrDefault(x =>
                x.VarausId == varausId &&
                x.PalveluId == palveluId);

            if (item != null)
            {
                items.Remove(item);
            }
        }

        // Poista kaikki palvelut varauksesta 
        public void DeleteByVarausId(int varausId)
        {
            items.RemoveAll(x => x.VarausId == varausId);
        }
    }
}
