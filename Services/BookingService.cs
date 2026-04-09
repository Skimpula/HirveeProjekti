using System.Collections.Generic;
using HirveeProjekti.Models;

namespace HirveeProjekti.Services
{
    public class BookingService
    {
        private List<Booking> _bookings = new List<Booking>();

        // Palauttaa kaikki varaukset
        public List<Booking> GetAllBookings()
        {
            return _bookings;
        }

        // Lisää varaus
        public void AddBooking(Booking booking)
        {
            _bookings.Add(booking);
        }

        // Poista varaus
        public void DeleteBooking(Booking booking)
        {
            _bookings.Remove(booking);
        }
    }
}