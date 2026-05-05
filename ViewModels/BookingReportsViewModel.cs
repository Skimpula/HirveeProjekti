using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HirveeProjekti.Models;
using HirveeProjekti.Services;
using Microsoft.Maui.Controls;

namespace HirveeProjekti.ViewModels
{
    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public bool IsBooked { get; set; }
        public string? BookingInfo { get; set; }
        public Color BackgroundColor => IsBooked
            ? Color.FromArgb("#E53935")
            : Color.FromArgb("#43A047");
        public Color TextColor => Color.FromArgb("#FFFFFF");
        public string StatusText => IsBooked ? "Varattu" : "Vapaa";
    }

    // Palveluraportille oma rivi-luokka
    public class ServiceReportRow
    {
        public string PalveluNimi { get; set; } = "";
        public string AlueNimi { get; set; } = "";
        public int VarausId { get; set; }
        public string AsiakasNimi { get; set; } = "";
        public int Lkm { get; set; }
        public double Hinta { get; set; }
        public double YhteensaEur => Lkm * Hinta;
        public string YhteensaTeksti => $"{YhteensaEur:F2} €";
    }

    public class BookingReportsViewModel : BindableObject
    {
        private readonly BookingService _bookingService;
        private readonly CottageService _cottageService;
        private readonly ServiceService _serviceService;
        private readonly AreaService _areaService;

        // ── Varauslista aikavälillä (aluesuodatuksella) ───────────────
        public ObservableCollection<Booking> FilteredBookings { get; } = new();

        private DateTime _filterStart = DateTime.Today;
        public DateTime FilterStart
        {
            get => _filterStart;
            set { _filterStart = value; OnPropertyChanged(); }
        }

        private DateTime _filterEnd = DateTime.Today.AddMonths(1);
        public DateTime FilterEnd
        {
            get => _filterEnd;
            set { _filterEnd = value; OnPropertyChanged(); }
        }

        // Aluesuodatin majoittumisraporttiin
        private List<Area> _filterAreas = new();
        public List<Area> FilterAreas
        {
            get => _filterAreas;
            set { _filterAreas = value; OnPropertyChanged(); }
        }

        private Area? _selectedFilterArea;
        public Area? SelectedFilterArea
        {
            get => _selectedFilterArea;
            set { _selectedFilterArea = value; OnPropertyChanged(); }
        }

        private string _bookingCountText = "";
        public string BookingCountText
        {
            get => _bookingCountText;
            set { _bookingCountText = value; OnPropertyChanged(); }
        }

        private double _bookingTotalRevenue;
        public double BookingTotalRevenue
        {
            get => _bookingTotalRevenue;
            set { _bookingTotalRevenue = value; OnPropertyChanged(); OnPropertyChanged(nameof(BookingRevenueTeksti)); }
        }
        public string BookingRevenueTeksti => $"Kokonaistulo: {BookingTotalRevenue:F2} €";

        // ── Mökkikalenteri ───────────────────────────────────────────
        private List<Cottage> _cottages = new();
        public List<Cottage> Cottages
        {
            get => _cottages;
            set { _cottages = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CalendarDay> CottageDays { get; } = new();

        private Cottage? _selectedCottage;
        public Cottage? SelectedCottage
        {
            get => _selectedCottage;
            set { _selectedCottage = value; OnPropertyChanged(); RefreshCottageCalendar(); }
        }

        private DateTime _cottageCalendarStart = DateTime.Today;
        public DateTime CottageCalendarStart
        {
            get => _cottageCalendarStart;
            set { _cottageCalendarStart = value; OnPropertyChanged(); }
        }

        private DateTime _cottageCalendarEnd = DateTime.Today.AddMonths(1);
        public DateTime CottageCalendarEnd
        {
            get => _cottageCalendarEnd;
            set { _cottageCalendarEnd = value; OnPropertyChanged(); }
        }

        // ── Palvelukalenteri ─────────────────────────────────────────
        private List<Service> _services = new();
        public List<Service> Services
        {
            get => _services;
            set { _services = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CalendarDay> ServiceDays { get; } = new();

        private Service? _selectedService;
        public Service? SelectedService
        {
            get => _selectedService;
            set { _selectedService = value; OnPropertyChanged(); RefreshServiceCalendar(); }
        }

        private DateTime _serviceCalendarStart = DateTime.Today;
        public DateTime ServiceCalendarStart
        {
            get => _serviceCalendarStart;
            set { _serviceCalendarStart = value; OnPropertyChanged(); }
        }

        private DateTime _serviceCalendarEnd = DateTime.Today.AddMonths(1);
        public DateTime ServiceCalendarEnd
        {
            get => _serviceCalendarEnd;
            set { _serviceCalendarEnd = value; OnPropertyChanged(); }
        }

        // ── Palvelujen käyttöraportti ────────────────────────────────
        public ObservableCollection<ServiceReportRow> ServiceReport { get; } = new();

        private DateTime _serviceReportStart = DateTime.Today;
        public DateTime ServiceReportStart
        {
            get => _serviceReportStart;
            set { _serviceReportStart = value; OnPropertyChanged(); }
        }

        private DateTime _serviceReportEnd = DateTime.Today.AddMonths(1);
        public DateTime ServiceReportEnd
        {
            get => _serviceReportEnd;
            set { _serviceReportEnd = value; OnPropertyChanged(); }
        }

        private List<Area> _serviceReportAreas = new();
        public List<Area> ServiceReportAreas
        {
            get => _serviceReportAreas;
            set { _serviceReportAreas = value; OnPropertyChanged(); }
        }

        private Area? _selectedServiceReportArea;
        public Area? SelectedServiceReportArea
        {
            get => _selectedServiceReportArea;
            set { _selectedServiceReportArea = value; OnPropertyChanged(); }
        }

        private string _serviceReportSummary = "";
        public string ServiceReportSummary
        {
            get => _serviceReportSummary;
            set { _serviceReportSummary = value; OnPropertyChanged(); }
        }

        // ── Komennot ─────────────────────────────────────────────────
        public ICommand FilterBookingsCommand { get; }
        public ICommand RefreshCottageCalendarCommand { get; }
        public ICommand RefreshServiceCalendarCommand { get; }
        public ICommand GenerateServiceReportCommand { get; }

        private List<Booking> _allBookings = new();
        private List<Area> _allAreas = new();
        private List<Service> _allServices = new();

        public BookingReportsViewModel()
        {
            _bookingService = new BookingService();
            _cottageService = new CottageService();
            _serviceService = new ServiceService();
            _areaService = new AreaService();

            FilterBookingsCommand = new Command(FilterBookings);
            RefreshCottageCalendarCommand = new Command(RefreshCottageCalendar);
            RefreshServiceCalendarCommand = new Command(RefreshServiceCalendar);
            GenerateServiceReportCommand = new Command(GenerateServiceReport);

            LoadData();
        }

        private void LoadData()
        {
            _allBookings = _bookingService.GetAllBookings();
            _allAreas = _areaService.GetAllAreas();
            _allServices = _serviceService.GetAllServices();

            // "Kaikki alueet" -vaihtoehto listoihin
            var kaikki = new Area { AlueId = 0, Nimi = "— Kaikki alueet —" };
            var areasWithAll = new List<Area> { kaikki };
            areasWithAll.AddRange(_allAreas);

            FilterAreas = areasWithAll;
            SelectedFilterArea = kaikki;

            ServiceReportAreas = new List<Area>(areasWithAll);
            SelectedServiceReportArea = kaikki;

            Cottages = _cottageService.GetAllCottages();
            Services = new List<Service>(_allServices);

            FilterBookings();
        }

        public void Refresh()
        {
            LoadData();
            if (SelectedCottage != null) RefreshCottageCalendar();
            if (SelectedService != null) RefreshServiceCalendar();
        }

        // ── Majoittumisraportti aikajaksolla + alue ──────────────────
        private void FilterBookings()
        {
            var allCottages = _cottageService.GetAllCottages();

            var filtered = _allBookings
                .Where(b => b.VarattuAlkuPvm < FilterEnd && b.VarattuLoppuPvm > FilterStart)
                .ToList();

            // Aluesuodatus: jos valittu alue (AlueId > 0), suodatetaan
            if (SelectedFilterArea != null && SelectedFilterArea.AlueId > 0)
            {
                var cottageIdsInArea = allCottages
                    .Where(c => c.AlueId == SelectedFilterArea.AlueId)
                    .Select(c => c.MokkiId)
                    .ToHashSet();
                filtered = filtered.Where(b => cottageIdsInArea.Contains(b.MokkiId)).ToList();
            }

            filtered = filtered.OrderBy(b => b.VarattuAlkuPvm).ToList();

            FilteredBookings.Clear();
            foreach (var b in filtered)
                FilteredBookings.Add(b);

            BookingCountText = $"{filtered.Count} varausta löydetty";

            // Laske kokonaistulo: mökin hinta × yöt
            double total = 0;
            foreach (var b in filtered)
            {
                var cottage = b.Mokki ?? allCottages.FirstOrDefault(c => c.MokkiId == b.MokkiId);
                if (cottage != null)
                {
                    int nights = (b.VarattuLoppuPvm - b.VarattuAlkuPvm).Days;
                    if (nights < 1) nights = 1;
                    total += cottage.Hinta * nights;
                }
            }
            BookingTotalRevenue = total;
        }

        // ── Mökkikalenteri ───────────────────────────────────────────
        private void RefreshCottageCalendar()
        {
            CottageDays.Clear();
            if (SelectedCottage == null) return;

            var bookingsForCottage = _allBookings
                .Where(b => b.MokkiId == SelectedCottage.MokkiId)
                .ToList();

            for (var d = CottageCalendarStart; d <= CottageCalendarEnd; d = d.AddDays(1))
            {
                var booking = bookingsForCottage
                    .FirstOrDefault(b => d >= b.VarattuAlkuPvm && d < b.VarattuLoppuPvm);

                CottageDays.Add(new CalendarDay
                {
                    Date = d,
                    IsBooked = booking != null,
                    BookingInfo = booking != null
                        ? $"Varaus #{booking.VarausId}: {booking.Asiakas?.FullName}"
                        : null
                });
            }
        }

        // ── Palvelukalenteri ─────────────────────────────────────────
        private void RefreshServiceCalendar()
        {
            ServiceDays.Clear();
            if (SelectedService == null) return;

            for (var d = ServiceCalendarStart; d <= ServiceCalendarEnd; d = d.AddDays(1))
            {
                var booking = _allBookings
                    .FirstOrDefault(b => d >= b.VarattuAlkuPvm && d < b.VarattuLoppuPvm);

                ServiceDays.Add(new CalendarDay
                {
                    Date = d,
                    IsBooked = booking != null,
                    BookingInfo = booking != null ? $"Varaus #{booking.VarausId}" : null
                });
            }
        }

        // ── Ostettujen palvelujen raportti ───────────────────────────
        private void GenerateServiceReport()
        {
            ServiceReport.Clear();

            // Filtteröi varaukset aikavälin mukaan
            var bookingsInRange = _allBookings
                .Where(b => b.VarattuAlkuPvm < ServiceReportEnd && b.VarattuLoppuPvm > ServiceReportStart)
                .ToList();

            // Aluesuodatus palveluille
            var servicesToShow = _allServices.ToList();
            if (SelectedServiceReportArea != null && SelectedServiceReportArea.AlueId > 0)
            {
                servicesToShow = servicesToShow
                    .Where(s => s.AlueId == SelectedServiceReportArea.AlueId)
                    .ToList();
            }

            // Koska ServiceOfBookings ei ole DB:ssä, simuloidaan: jokainen varaus
            // sisältää satunnaisesti palveluita esimerkkidatana.
            // Kun ServiceOfBookings kytketään SQLite:hen, tämä logiikka korvataan.
            var rows = new List<ServiceReportRow>();
            foreach (var booking in bookingsInRange)
            {
                foreach (var service in servicesToShow)
                {
                    // Tässä pitäisi hakea ServiceOfBookings-taulu.
                    // Placeholder: näytetään palvelut alueelta, jolla mökki sijaitsee.
                    var cottage = booking.Mokki ?? _cottageService.GetAllCottages()
                        .FirstOrDefault(c => c.MokkiId == booking.MokkiId);
                    if (cottage != null && cottage.AlueId == service.AlueId)
                    {
                        rows.Add(new ServiceReportRow
                        {
                            PalveluNimi = service.Nimi ?? "",
                            AlueNimi = _allAreas.FirstOrDefault(a => a.AlueId == service.AlueId)?.Nimi ?? "",
                            VarausId = booking.VarausId,
                            AsiakasNimi = booking.Asiakas?.FullName ?? $"Asiakas #{booking.AsiakasId}",
                            Lkm = 1,
                            Hinta = service.Hinta
                        });
                    }
                }
            }

            foreach (var row in rows.OrderBy(r => r.PalveluNimi))
                ServiceReport.Add(row);

            double kokonais = rows.Sum(r => r.YhteensaEur);
            ServiceReportSummary = $"{rows.Count} palvelua | Yhteensä: {kokonais:F2} €";
        }
    }
}
