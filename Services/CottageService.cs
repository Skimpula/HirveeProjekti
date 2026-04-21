using System.Collections.Generic;
using System.Linq;
using HirveeProjekti.Models;
using SQLite;

namespace HirveeProjekti.Services
{
    public class CottageService
    {
        private SQLiteConnection _db;

        public CottageService()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "vn.db");

            _db = new SQLiteConnection(dbPath);
            
            // Initialize database on first use
            var initializer = new DatabaseInitializer(_db);
            initializer.Initialize();
        }

        // Get all cottages from database with area information
        public List<Cottage> GetAllCottages()
        {
            try
            {
                _db.CreateTable<Cottage>();
                
                var cottages = _db.Query<Cottage>("SELECT * FROM mokki ORDER BY mokki_id");

                return cottages;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cottages: {ex.Message}");
                return new List<Cottage>();
            }
        }

        // Add cottage to database
        public void AddCottage(Cottage cottage)
        {
            try
            {
                _db.Insert(cottage);
                System.Diagnostics.Debug.WriteLine("Cottage added successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding cottage: {ex.Message}");
            }
        }

        // Delete cottage from database
        public void DeleteCottage(Cottage cottage)
        {
            try
            {
                _db.Delete(cottage);
                System.Diagnostics.Debug.WriteLine("Cottage deleted successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting cottage: {ex.Message}");
            }
        }
    }
}