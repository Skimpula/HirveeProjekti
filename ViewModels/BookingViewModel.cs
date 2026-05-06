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


        public ObservableCollection<Customer> Customers { get; set; } = new();
        public ObservableCollection<Cottage> Cottages { get; set; } = new();
        public ObservableCollection<Booking> Bookings { get; set; } = new();


        public Customer SelectedCustomer { get; set; }
        public Cottage SelectedCottage { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);

        public ICommand AddBookingCommand { get; }

        public BookingViewModel()
        {
            _bookingService = new BookingService();
            _customerService = new CustomerService();
            _cottageService = new CottageService();

            AddBookingCommand = new Command(AddBooking);

            RefreshBookings();
        }


        public void RefreshBookings()
        {
            LoadCustomers();
            LoadCottages();
            LoadBookings();

            System.Diagnostics.Debug.WriteLine($"Bookings: {Bookings.Count}");
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
                return;

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

            RefreshBookings();
        }
    }
}
