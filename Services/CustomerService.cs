using System.Collections.Generic;
using System.Linq;
using HirveeProjekti.Models;
using SQLite;

namespace HirveeProjekti.Services
{
    public class CustomerService
    {
        private SQLiteConnection _db;
        private static bool _initialized = false;

        public CustomerService()
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

        // Get all customers from database
        public List<Customer> GetAllCustomers()
        {
            try
            {
                var customers = _db.Table<Customer>().ToList();
                System.Diagnostics.Debug.WriteLine($"Loaded {customers.Count} customers from database");
                return customers;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading customers: {ex.Message}");
                return new List<Customer>();
            }
        }

        // Add customer to database
        public void AddCustomer(Customer customer)
        {
            try
            {
                _db.Insert(customer);
                System.Diagnostics.Debug.WriteLine("Customer added successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding customer: {ex.Message}");
            }
        }

        // Delete customer from database
        public void DeleteCustomer(Customer customer)
        {
            try
            {
                _db.Delete(customer);
                System.Diagnostics.Debug.WriteLine("Customer deleted successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting customer: {ex.Message}");
            }
        }
    }
}