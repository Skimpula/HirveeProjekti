using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HirveeProjekti.Models;
using HirveeProjekti.Services;
using Microsoft.Maui.Controls;

namespace HirveeProjekti.ViewModels
{
    public class BookingViewModel : BindableObject
    {
        private readonly BookingService _bookingService;

        // Lista varauksista
        public ObservableCollection<Booking> Bookings { get; set; } = new ObservableCollection<Booking>();

        // Valitut tiedot lomakkeelta
        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
            }
        }

        private Cottage _selectedCottage;
        public Cottage SelectedCottage
        {
            get => _selectedCottage;
            set
            {
                _selectedCottage = value;
                OnPropertyChanged();
            }
        }

        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _endDate = DateTime.Today.AddDays(1);
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        // Komennot
        public ICommand AddBookingCommand { get; }
        public ICommand RemoveBookingCommand { get; }

        public BookingViewModel()
        {
            _bookingService = new BookingService(); // Käytetään valmista palvelua

            // Lataa varaukset palvelusta
            LoadBookings();

            // Komennot
            AddBookingCommand = new Command(AddBooking);
            RemoveBookingCommand = new Command<Booking>(RemoveBooking);
        }

        private void LoadBookings()
        {
            // Placeholder: haetaan varaukset palvelusta
            var list = _bookingService.GetAllBookings(); // Oletetaan, että tämä palauttaa List<Booking>
            Bookings.Clear();
            foreach (var b in list)
            {
                Bookings.Add(b);
            }
        }

        private void AddBooking()
        {
            if (SelectedCustomer == null || SelectedCottage == null)
                return; // Ei voi lisätä ilman asiakasta ja mökkiä

            var newBooking = new Booking
            {
                Asiakas = SelectedCustomer,
                Mokki = SelectedCottage,
                VarattuAlkuPvm = StartDate,
                VarattuLoppuPvm = EndDate,
                VarattuPvm = DateTime.Now
            };

            _bookingService.AddBooking(newBooking); // Tallennus palveluun
            Bookings.Add(newBooking);

            // Tyhjennä valinnat
            SelectedCustomer = null;
            SelectedCottage = null;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddDays(1);
        }

        private void RemoveBooking(Booking booking)
        {
            if (booking == null) return;

            _bookingService.DeleteBooking(booking); // Poisto palvelusta
            Bookings.Remove(booking);
        }
    }
}