using HirveeProjekti.Models;
using HirveeProjekti.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace HirveeProjekti.ViewModels
{
    public class BookingViewModel : BindableObject
    {
        private readonly BookingService _bookingService;
        private readonly CustomerService _customerService;
        private readonly CottageService _cottageService;

        // Collections for pickers
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();
        public ObservableCollection<Cottage> Cottages { get; set; } = new ObservableCollection<Cottage>();
        public ObservableCollection<Booking> Bookings { get; set; } = new ObservableCollection<Booking>();

        // Selected items from form
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

        // Error handling
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand AddBookingCommand { get; }
        public ICommand RemoveBookingCommand { get; }

        public BookingViewModel()
        {
            _bookingService = new BookingService();
            _customerService = new CustomerService();
            _cottageService = new CottageService();

            // Load initial data
            LoadCustomers();
            LoadCottages();
            LoadBookings();

            // Initialize commands
            AddBookingCommand = new Command(AddBooking);
            RemoveBookingCommand = new Command<Booking>(RemoveBooking);
        }

        private void LoadCustomers()
        {
            var list = _customerService.GetAllCustomers();
            Customers.Clear();
            foreach (var customer in list)
            {
                Customers.Add(customer);
            }
        }

        private void LoadCottages()
        {
            var list = _cottageService.GetAllCottages();
            Cottages.Clear();
            foreach (var cottage in list)
            {
                Cottages.Add(cottage);
            }
        }

        private void LoadBookings()
        {
            var list = _bookingService.GetAllBookings();
            Bookings.Clear();
            foreach (var booking in list)
            {
                Bookings.Add(booking);
            }
        }

        private void AddBooking()
        {
            ClearError();

            if (SelectedCustomer == null)
            {
                ShowError("Valitse asiakas");
                return;
            }

            if (SelectedCottage == null)
            {
                ShowError("Valitse mökki");
                return;
            }

            if (EndDate <= StartDate)
            {
                ShowError("Loppupäivä pitää olla alkupäivän jälkeen");
                return;
            }

            var newBooking = new Booking
            {
                AsiakasId = SelectedCustomer.AsiakasId,
                MokkiId = SelectedCottage.MokkiId,
                VarattuAlkuPvm = StartDate,
                VarattuLoppuPvm = EndDate,
                VarattuPvm = DateTime.Now,
                VahvistusPvm = DateTime.Now,
                Asiakas = SelectedCustomer,
                Mokki = SelectedCottage
            };

            _bookingService.AddBooking(newBooking);
            LoadBookings();

            // Reset form
            SelectedCustomer = null;
            SelectedCottage = null;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddDays(1);
        }

        private void RemoveBooking(Booking booking)
        {
            if (booking == null) return;

            _bookingService.DeleteBooking(booking);
            Bookings.Remove(booking);
        }

        private void ShowError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        private void ClearError()
        {
            ErrorMessage = string.Empty;
            HasError = false;
        }

        public void RefreshBookings()
        {
            LoadCustomers();
            LoadCottages();
            LoadBookings();
            System.Diagnostics.Debug.WriteLine($"Bookings refreshed - Total: {Bookings.Count}");
        }
    }
}

