using HirveeProjekti.Models;
using HirveeProjekti.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HirveeProjekti.ViewModels
{
    public class BookingViewModel : BindableObject
    {
        public ObservableCollection<DateTime> CalendarDates { get; set; } = new();
        public ObservableCollection<Cottage> CalendarCottages { get; set; } = new();
        public Dictionary<DateTime, Dictionary<int, Booking?>> CalendarBookings { get; set; }
            = new Dictionary<DateTime, Dictionary<int, Booking?>>();

        private readonly BookingService _bookingService;
        private readonly CustomerService _customerService;
        private readonly CottageService _cottageService;
        private readonly ServiceService _serviceService;
        private readonly ServiceOfBookingsService _serviceOfBookingsService;

        private List<Service> _allServices = new();

        public ObservableCollection<Customer> Customers { get; set; } = new();
        public ObservableCollection<Cottage> Cottages { get; set; } = new();
        public ObservableCollection<Booking> Bookings { get; set; } = new();
        public ObservableCollection<SelectableService> AvailableServices { get; set; } = new();

        private Customer? _selectedCustomer;
        public Customer? SelectedCustomer
        {
            get => _selectedCustomer;
            set { _selectedCustomer = value; OnPropertyChanged(); }
        }

        private Cottage? _selectedCottage;
        public Cottage? SelectedCottage
        {
            get => _selectedCottage;
            set
            {
                _selectedCottage = value;
                OnPropertyChanged();
                FilterServicesByCottage();
            }
        }

        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasError)); }
        }
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public ICommand AddBookingCommand { get; }
        public ICommand EditBookingCommand { get; }
        public ICommand RemoveBookingCommand { get; }

        public BookingViewModel()
        {
            _bookingService = new BookingService();
            _customerService = new CustomerService();
            _cottageService = new CottageService();
            _serviceService = new ServiceService();
            _serviceOfBookingsService = new ServiceOfBookingsService();

            AddBookingCommand = new Command(AddBooking);
            EditBookingCommand = new Command<Booking>(EditBooking);
            RemoveBookingCommand = new Command<Booking>(RemoveBooking);

            RefreshBookings();
        }

        public void RefreshBookings()
        {
            LoadCustomers();
            LoadCottages();
            LoadBookings();
            LoadServices();
        }

        private void LoadCustomers()
        {
            Customers.Clear();
            foreach (var c in _customerService.GetAllCustomers())
                Customers.Add(c);
        }

        private void LoadCottages()
        {
            Cottages.Clear();
            foreach (var c in _cottageService.GetAllCottages())
                Cottages.Add(c);
        }

        private void LoadBookings()
        {
            Bookings.Clear();
            foreach (var b in _bookingService.GetAllBookings())
                Bookings.Add(b);
            RefreshCalendar();
        }

        private void LoadServices()
        {
            _allServices = _serviceService.GetAllServices();
            FilterServicesByCottage();
        }

        private void FilterServicesByCottage()
        {
            AvailableServices.Clear();
            var services = _selectedCottage != null
                ? _allServices.Where(s => s.AlueId == _selectedCottage.AlueId)
                : _allServices;

            foreach (var s in services)
                AvailableServices.Add(new SelectableService { Service = s });
        }

        public void RefreshCalendar()
        {
            CalendarDates.Clear();
            CalendarCottages.Clear();
            CalendarBookings.Clear();

            var start = DateTime.Today;
            var end = DateTime.Today.AddDays(13);

            for (var d = start; d <= end; d = d.AddDays(1))
                CalendarDates.Add(d);

            foreach (var c in Cottages)
                CalendarCottages.Add(c);

            foreach (var date in CalendarDates)
            {
                CalendarBookings[date] = new Dictionary<int, Booking?>();
                foreach (var cottage in CalendarCottages)
                {
                    var booking = Bookings.FirstOrDefault(b =>
                        b.MokkiId == cottage.MokkiId &&
                        date >= b.VarattuAlkuPvm &&
                        date < b.VarattuLoppuPvm);
                    CalendarBookings[date][cottage.MokkiId] = booking;
                }
            }

            OnPropertyChanged(nameof(CalendarDates));
            OnPropertyChanged(nameof(CalendarCottages));
            OnPropertyChanged(nameof(CalendarBookings));
        }

        private void AddBooking()
        {
            if (SelectedCustomer == null || SelectedCottage == null)
            {
                ErrorMessage = "Valitse asiakas ja mökki.";
                return;
            }
            if (EndDate <= StartDate)
            {
                ErrorMessage = "Loppupäivän täytyy olla alkupäivän jälkeen.";
                return;
            }

            ErrorMessage = string.Empty;

            var booking = new Booking
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

            _bookingService.AddBooking(booking);

            foreach (var sel in AvailableServices.Where(s => s.IsSelected))
            {
                _serviceOfBookingsService.Add(new ServiceOfBookings
                {
                    VarausId = booking.VarausId,
                    PalveluId = sel.Service.PalveluId,
                    Lkm = sel.Quantity
                });
            }

            RefreshBookings();
        }

        private async void EditBooking(Booking booking)
        {
            if (booking == null) return;
            // Placeholder for edit functionality
        }

        private void RemoveBooking(Booking booking)
        {
            if (booking == null) return;
            _serviceOfBookingsService.DeleteByVarausId(booking.VarausId);
            _bookingService.DeleteBooking(booking);
            RefreshBookings();
        }
    }
}
