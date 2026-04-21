using System.Collections.Generic;
using System.Linq;
using HirveeProjekti.Models;
using SQLite;

namespace HirveeProjekti.Services
{
    public class AreaService
    {
        private SQLiteConnection _db;

        public AreaService()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "vn.db");

            _db = new SQLiteConnection(dbPath);
            
            // Initialize database on first use
            var initializer = new DatabaseInitializer(_db);
            initializer.Initialize();
        }

        // Get all areas from database
        public List<Area> GetAllAreas()
        {
            try
            {
                _db.CreateTable<Area>();
                
                var areas = _db.Query<Area>("SELECT * FROM alue ORDER BY alue_id");

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
