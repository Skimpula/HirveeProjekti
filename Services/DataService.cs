using SQLite;
using System.IO;

namespace HirveeProjekti.Services
{
    public class DataService
    {
        private SQLiteConnection _db;

        public DataService()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "vn.db");

            _db = new SQLiteConnection(dbPath);

        }

        public SQLiteConnection GetConnection()
        {
            return _db;
        }
    }
}