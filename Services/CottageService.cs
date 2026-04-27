using System.Collections.Generic;
using System.Linq;
using HirveeProjekti.Models;
using SQLite;

namespace HirveeProjekti.Services
{
    public class CottageService
    {
        private SQLiteConnection _db;
        private readonly AreaService _areaService = new AreaService();
        private static bool _initialized = false;

        public CottageService()
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
            }
        }

        // Get all cottages from database with area information
        public List<Cottage> GetAllCottages()
        {
            try
            {
                var cottages = _db.Table<Cottage>().ToList();
                var areas = _areaService.GetAllAreas();

                foreach (var cottage in cottages)
                {
                    cottage.AreaName = areas.FirstOrDefault(a => a.AlueId == cottage.AlueId)?.Nimi ?? "Tuntematon";
                }

                System.Diagnostics.Debug.WriteLine($"Loaded {cottages.Count} cottages from database");
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