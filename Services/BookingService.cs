using System.Collections.Generic;
using System.Linq;
using HirveeProjekti.Models;
using SQLite;

namespace HirveeProjekti.Services
{
    public class BookingService
    {
        private SQLiteConnection _db;
        private CustomerService _customerService;
        private CottageService _cottageService;
        private static bool _initialized = false;

        public BookingService()
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
            
            _customerService = new CustomerService();
            _cottageService = new CottageService();
        }

        // Get all bookings from database with related data
        public List<Booking> GetAllBookings()
        {
            try
            {
                var bookings = _db.Table<Booking>().ToList();
                System.Diagnostics.Debug.WriteLine($"Loaded {bookings.Count} bookings from database");

                // Load related customer and cottage data
                foreach (var booking in bookings)
                {
                    booking.Asiakas = _customerService.GetAllCustomers()
                        .FirstOrDefault(c => c.AsiakasId == booking.AsiakasId);
                    booking.Mokki = _cottageService.GetAllCottages()
                        .FirstOrDefault(c => c.MokkiId == booking.MokkiId);
                }

                return bookings;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading bookings: {ex.Message}");
                return new List<Booking>();
            }
        }

        // Add booking to database
        public void AddBooking(Booking booking)
        {
            try
            {
                _db.Insert(booking);
                System.Diagnostics.Debug.WriteLine("Booking added successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding booking: {ex.Message}");
            }
        }

        // Delete booking from database
        public void DeleteBooking(Booking booking)
        {
            try
            {
                _db.Delete(booking);
                System.Diagnostics.Debug.WriteLine("Booking deleted successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting booking: {ex.Message}");
            }
        }
    }
}