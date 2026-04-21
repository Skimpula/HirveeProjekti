using System.Collections.Generic;
using System.Linq;
using HirveeProjekti.Models;
using SQLite;

namespace HirveeProjekti.Services
{
    public class AreaService
    {
        private SQLiteConnection _db;
        private static bool _initialized = false;

        public AreaService()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "vn.db");

            _db = new SQLiteConnection(dbPath);
            
            // Initialize database only once
            if (!_initialized)
            {
                var initializer = new DatabaseInitializer(_db);
                initializer.Initialize();
                _initialized = true;
                System.Diagnostics.Debug.WriteLine($"Database initialized at: {dbPath}");
            }
        }

        // Get all areas from database
        public List<Area> GetAllAreas()
        {
            try
            {
                var areas = _db.Table<Area>().ToList();
                System.Diagnostics.Debug.WriteLine($"Loaded {areas.Count} areas from database");
                return areas;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading areas: {ex.Message}");
                return new List<Area>();
            }
        }

        // Add area to database
        public void AddArea(Area area)
        {
            try
            {
                _db.Insert(area);
                System.Diagnostics.Debug.WriteLine("Area added successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding area: {ex.Message}");
            }
        }

        // Delete area from database
        public void DeleteArea(Area area)
        {
            try
            {
                _db.Delete(area);
                System.Diagnostics.Debug.WriteLine("Area deleted successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting area: {ex.Message}");
            }
        }

        // Update area in database
        public void UpdateArea(Area area)
        {
            try
            {
                _db.Update(area);
                System.Diagnostics.Debug.WriteLine("Area updated successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating area: {ex.Message}");
            }
        }
    }
}
