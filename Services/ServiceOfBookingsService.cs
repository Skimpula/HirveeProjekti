using HirveeProjekti.Models;
using SQLite;

namespace HirveeProjekti.Services
{
    public class ServiceOfBookingsService
    {
        private readonly SQLiteConnection _db;

        public ServiceOfBookingsService()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "vn.db");
            _db = new SQLiteConnection(dbPath);
        }

        public List<ServiceOfBookings> GetAll()
        {
            return _db.Table<ServiceOfBookings>().ToList();
        }

        public List<ServiceOfBookings> GetByVarausId(int varausId)
        {
            return _db.Table<ServiceOfBookings>()
                .Where(x => x.VarausId == varausId)
                .ToList();
        }

        public void Add(ServiceOfBookings item)
        {
            var existing = _db.Table<ServiceOfBookings>()
                .FirstOrDefault(x => x.VarausId == item.VarausId && x.PalveluId == item.PalveluId);

            if (existing != null)
            {
                existing.Lkm += item.Lkm;
                _db.Update(existing);
            }
            else
            {
                _db.Insert(item);
            }
        }

        public void UpdateLkm(int varausId, int palveluId, int newLkm)
        {
            var item = _db.Table<ServiceOfBookings>()
                .FirstOrDefault(x => x.VarausId == varausId && x.PalveluId == palveluId);
            if (item != null)
            {
                item.Lkm = newLkm;
                _db.Update(item);
            }
        }

        public void Delete(int varausId, int palveluId)
        {
            var item = _db.Table<ServiceOfBookings>()
                .FirstOrDefault(x => x.VarausId == varausId && x.PalveluId == palveluId);
            if (item != null)
                _db.Delete(item);
        }

        public void DeleteByVarausId(int varausId)
        {
            var items = _db.Table<ServiceOfBookings>()
                .Where(x => x.VarausId == varausId)
                .ToList();
            foreach (var item in items)
                _db.Delete(item);
        }
    }
}
