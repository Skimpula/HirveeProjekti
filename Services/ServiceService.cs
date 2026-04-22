using System.Collections.Generic;
using System.Linq;
using HirveeProjekti.Models;
using SQLite;

namespace HirveeProjekti.Services
{
    public class ServiceService
    {
        private SQLiteConnection _db;
        private static bool _initialized = false;

        public ServiceService()
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

        // Get all services from database
        public List<Service> GetAllServices()
        {
            try
            {
                var services = _db.Table<Service>().ToList();
                System.Diagnostics.Debug.WriteLine($"Loaded {services.Count} services from database");
                return services;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading services: {ex.Message}");
                return new List<Service>();
            }
        }

        // Add service to database
        public void AddService(Service service)
        {
            try
            {
                _db.Insert(service);
                System.Diagnostics.Debug.WriteLine("Service added successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding service: {ex.Message}");
            }
        }

        // Delete service from database
        public void DeleteService(Service service)
        {
            try
            {
                _db.Delete(service);
                System.Diagnostics.Debug.WriteLine("Service deleted successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting service: {ex.Message}");
            }
        }

        // Update service in database
        public void UpdateService(Service service)
        {
            try
            {
                _db.Update(service);
                System.Diagnostics.Debug.WriteLine("Service updated successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating service: {ex.Message}");
            }
        }
    }
}
