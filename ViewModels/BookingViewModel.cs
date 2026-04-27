using System.Collections.ObjectModel;     
using HirveeProjekti.Models;
using HirveeProjekti.Services;
using System;
using HirveeProjekti.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace HirveeProjekti.ViewModels
{
    public class BookingViewModel : BindableObject
    {
          public ObservableCollection<DateTime> CalendarDates { get; set; } = new ObservableCollection<DateTime>();
            public ObservableCollection<Cottage> CalendarCottages { get; set; } = new ObservableCollection<Cottage>();
            // Dictionary: [date][cottageId] = Booking or null
            public Dictionary<DateTime, Dictionary<int, Booking?>> CalendarBookings { get; set; } = new Dictionary<DateTime, Dictionary<int, Booking?>>();

        // Call this to refresh the calendar data
        public void RefreshCalendar(DateTime? start = null, DateTime? end = null)
        {
            // Show 14 days by default
            DateTime from = start ?? DateTime.Today;
            DateTime to = end ?? DateTime.Today.AddDays(13);
            CalendarDates.Clear();
            for (var d = from; d <= to; d = d.AddDays(1))
                CalendarDates.Add(d);

            CalendarCottages.Clear();
            foreach (var c in Cottages)
                CalendarCottages.Add(c);

            CalendarBookings.Clear();
            foreach (var date in CalendarDates)
            {
                CalendarBookings[date] = new Dictionary<int, Booking?>();
                foreach (var cottage in CalendarCottages)
                {
                    var booking = Bookings.FirstOrDefault(b => b.MokkiId == cottage.MokkiId && date >= b.VarattuAlkuPvm && date < b.VarattuLoppuPvm);
                    CalendarBookings[date][cottage.MokkiId] = booking;
                }
            }
            OnPropertyChanged(nameof(CalendarDates));
            OnPropertyChanged(nameof(CalendarCottages));
            OnPropertyChanged(nameof(CalendarBookings));
        }
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

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddButtonText));
            }
        }

        private Booking _editingBooking;
        public Booking EditingBooking
        {
            get => _editingBooking;
            set
            {
                _editingBooking = value;
                OnPropertyChanged();
            }
        }

        public string AddButtonText => IsEditMode ? "Päivitä varaus" : "Tee varaus";

        // Commands
        public ICommand AddBookingCommand { get; }
        public ICommand RemoveBookingCommand { get; }
        public ICommand EditBookingCommand { get; }
        public ICommand CancelEditCommand { get; }

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
            EditBookingCommand = new Command<Booking>(EditBooking);
            CancelEditCommand = new Command(CancelEdit);
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
            RefreshCalendar();
        }

        private bool HasBookingConflict(int mokkiId, DateTime startDate, DateTime endDate)
        {
            foreach (var booking in Bookings)
            {
                // Skip the booking we're currently editing
                if (IsEditMode && EditingBooking != null && booking.VarausId == EditingBooking.VarausId)
                {
                    continue;
                }

                // Check if this booking is for the same cottage
                if (booking.MokkiId == mokkiId)
                {
                    // Check if date ranges overlap
                    // Overlap occurs if: newStart < existingEnd AND newEnd > existingStart
                    if (startDate < booking.VarattuLoppuPvm && endDate > booking.VarattuAlkuPvm)
                    {
                        System.Diagnostics.Debug.WriteLine($"Booking conflict detected for cottage {mokkiId}");
                        return true;
                    }
                }
            }

            return false;
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

            // Check for booking conflicts
            if (HasBookingConflict(SelectedCottage.MokkiId, StartDate, EndDate))
            {
                ShowError("Mökki on jo varattu valituille päivämäärille");
                return;
            }

            if (IsEditMode && EditingBooking != null)
            {
                // Update existing booking
                EditingBooking.AsiakasId = SelectedCustomer.AsiakasId;
                EditingBooking.MokkiId = SelectedCottage.MokkiId;
                EditingBooking.VarattuAlkuPvm = StartDate;
                EditingBooking.VarattuLoppuPvm = EndDate;
                EditingBooking.Asiakas = SelectedCustomer;
                EditingBooking.Mokki = SelectedCottage;

                _bookingService.UpdateBooking(EditingBooking);
                System.Diagnostics.Debug.WriteLine($"Booking {EditingBooking.VarausId} updated");
            }
            else
            {
                // Create new booking
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
                System.Diagnostics.Debug.WriteLine("New booking created");
            }

            LoadBookings();
            CancelEdit();
        }

        private void EditBooking(Booking booking)
        {
            if (booking == null) return;

            IsEditMode = true;
            EditingBooking = booking;
            
            // Find the exact customer object from the Customers collection
            var customer = Customers.FirstOrDefault(c => c.AsiakasId == booking.AsiakasId);
            if (customer != null)
            {
                SelectedCustomer = customer;
            }
            
            // Find the exact cottage object from the Cottages collection
            var cottage = Cottages.FirstOrDefault(c => c.MokkiId == booking.MokkiId);
            if (cottage != null)
            {
                SelectedCottage = cottage;
            }
            
            StartDate = booking.VarattuAlkuPvm;
            EndDate = booking.VarattuLoppuPvm;

            System.Diagnostics.Debug.WriteLine($"Editing booking {booking.VarausId}");
        }

        private void CancelEdit()
        {
            IsEditMode = false;
            EditingBooking = null;
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

