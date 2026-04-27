using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HirveeProjekti.Models;
using HirveeProjekti.Services;
using Microsoft.Maui.Controls;

namespace HirveeProjekti.ViewModels
{
    public class CottageViewModel : BindableObject
    {
        private readonly CottageService _cottageService = new CottageService();
        private readonly AreaService _areaService = new AreaService();
        private readonly BookingService _bookingService = new BookingService();

        public ObservableCollection<Cottage> Cottages { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Area> SearchAreas { get; } = new();
        public ObservableCollection<Cottage> AvailableCottages { get; } = new();

        private Cottage? _selectedCottage;
        public Cottage? SelectedCottage
        {
            get => _selectedCottage;
            set
            {
                _selectedCottage = value;
                OnPropertyChanged();
            }
        }

        private Area? _selectedArea;
        public Area? SelectedArea
        {
            get => _selectedArea;
            set
            {
                _selectedArea = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NewCottageArea = value.AlueId;
                }
            }
        }

        private Area? _selectedSearchArea;
        public Area? SelectedSearchArea
        {
            get => _selectedSearchArea;
            set
            {
                _selectedSearchArea = value;
                OnPropertyChanged();
                SearchAvailableCottages();
            }
        }

        private DateTime _searchStartDate = DateTime.Today;
        public DateTime SearchStartDate
        {
            get => _searchStartDate;
            set
            {
                if (SetField(ref _searchStartDate, value))
                {
                    SearchAvailableCottages();
                }
            }
        }

        private DateTime _searchEndDate = DateTime.Today.AddDays(1);
        public DateTime SearchEndDate
        {
            get => _searchEndDate;
            set
            {
                if (SetField(ref _searchEndDate, value))
                {
                    SearchAvailableCottages();
                }
            }
        }

        private string _newCottageName = string.Empty;
        public string NewCottageName
        {
            get => _newCottageName;
            set => SetField(ref _newCottageName, value);
        }

        private string _newCottageAddress = string.Empty;
        public string NewCottageAddress
        {
            get => _newCottageAddress;
            set => SetField(ref _newCottageAddress, value);
        }

        private int _newCottageArea = 1;
        public int NewCottageArea
        {
            get => _newCottageArea;
            set => SetField(ref _newCottageArea, value);
        }

        private string _newCottagePrice = string.Empty;
        public string NewCottagePrice
        {
            get => _newCottagePrice;
            set => SetField(ref _newCottagePrice, value);
        }

        private string _newCottageCapacity = string.Empty;
        public string NewCottageCapacity
        {
            get => _newCottageCapacity;
            set => SetField(ref _newCottageCapacity, value);
        }

        private string _newCottageEquipment = string.Empty;
        public string NewCottageEquipment
        {
            get => _newCottageEquipment;
            set => SetField(ref _newCottageEquipment, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetField(ref _errorMessage, value))
                {
                    OnPropertyChanged(nameof(HasError));
                }
            }
        }

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        private string _searchMessage = string.Empty;
        public string SearchMessage
        {
            get => _searchMessage;
            set
            {
                if (SetField(ref _searchMessage, value))
                {
                    OnPropertyChanged(nameof(HasSearchMessage));
                }
            }
        }

        public bool HasSearchMessage => !string.IsNullOrWhiteSpace(SearchMessage);

        public ICommand AddCottageCommand { get; }
        public ICommand DeleteCottageCommand { get; }
        public ICommand SearchAvailableCottagesCommand { get; }

        public CottageViewModel()
        {
            AddCottageCommand = new Command(AddCottage);
            DeleteCottageCommand = new Command<Cottage>(DeleteCottage);
            SearchAvailableCottagesCommand = new Command(SearchAvailableCottages);

            LoadAreas();
            LoadCottages();
            LoadSearchAreas();
            SearchAvailableCottages();
        }

        private void LoadAreas()
        {
            try
            {
                Areas.Clear();
                var areas = _areaService.GetAllAreas();
                foreach (var area in areas)
                {
                    Areas.Add(area);
                }
                
                // Set default selected area to first one
                if (Areas.Count > 0)
                {
                    SelectedArea = Areas[0];
                }
                
                System.Diagnostics.Debug.WriteLine($"Loaded {Areas.Count} areas into ViewModel");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading areas: {ex.Message}");
            }
        }

        private void LoadSearchAreas()
        {
            SearchAreas.Clear();
            SearchAreas.Add(new Area { AlueId = 0, Nimi = "Kaikki alueet" });

            foreach (var area in Areas)
            {
                SearchAreas.Add(area);
            }

            SelectedSearchArea = SearchAreas.FirstOrDefault();
        }

        private void LoadCottages()
        {
            try
            {
                Cottages.Clear();

                var cottages = _cottageService.GetAllCottages();
                foreach (var cottage in cottages)
                {
                    Cottages.Add(cottage);
                }

                System.Diagnostics.Debug.WriteLine($"Loaded {Cottages.Count} cottages into ViewModel");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading cottages: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error loading cottages: {ex.Message}");
            }
        }

        private void SearchAvailableCottages()
        {
            try
            {
                SearchMessage = string.Empty;

                var start = SearchStartDate.Date;
                var end = SearchEndDate.Date;

                if (end <= start)
                {
                    SearchMessage = "Loppupäivän pitää olla alkupäivän jälkeen.";
                    AvailableCottages.Clear();
                    return;
                }

                var cottages = _cottageService.GetAllCottages();
                var bookings = _bookingService.GetAllBookings();

                if (SelectedSearchArea != null && SelectedSearchArea.AlueId > 0)
                {
                    cottages = cottages.Where(c => c.AlueId == SelectedSearchArea.AlueId).ToList();
                }

                var freeCottages = cottages
                    .Where(cottage => !bookings.Any(booking =>
                        booking.MokkiId == cottage.MokkiId &&
                        start < booking.VarattuLoppuPvm.Date &&
                        end > booking.VarattuAlkuPvm.Date))
                    .OrderBy(c => c.AreaName)
                    .ThenBy(c => c.Mokkinimi)
                    .ToList();

                AvailableCottages.Clear();
                foreach (var cottage in freeCottages)
                {
                    AvailableCottages.Add(cottage);
                }

                SearchMessage = freeCottages.Count == 0
                    ? $"Yhtään vapaata mökkiä ei löytynyt ajalle {start:dd.MM.yyyy} - {end:dd.MM.yyyy}."
                    : $"Löytyi {freeCottages.Count} vapaata mökkiä ajalle {start:dd.MM.yyyy} - {end:dd.MM.yyyy}.";
            }
            catch (Exception ex)
            {
                SearchMessage = $"Vapaiden mökkien haku epäonnistui: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error searching available cottages: {ex.Message}");
            }
        }

        private void AddCottage()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewCottageName) ||
                    string.IsNullOrWhiteSpace(NewCottageAddress) ||
                    string.IsNullOrWhiteSpace(NewCottagePrice) ||
                    string.IsNullOrWhiteSpace(NewCottageCapacity))
                {
                    ErrorMessage = "Täytä vähintään nimi, osoite, hinta ja henkilömäärä.";
                    return;
                }

                if (!double.TryParse(NewCottagePrice, out var price))
                {
                    ErrorMessage = "Hinta pitää olla numero.";
                    return;
                }

                if (!int.TryParse(NewCottageCapacity, out var capacity))
                {
                    ErrorMessage = "Henkilömäärä pitää olla kokonaisluku.";
                    return;
                }

                var cottage = new Cottage
                {
                    AlueId = NewCottageArea,
                    Mokkinimi = NewCottageName.Trim(),
                    Katuosoite = NewCottageAddress.Trim(),
                    Hinta = price,
                    Henkilomaara = capacity,
                    Varustelu = NewCottageEquipment.Trim()
                };

                _cottageService.AddCottage(cottage);
                LoadCottages();

                NewCottageName = string.Empty;
                NewCottageAddress = string.Empty;
                NewCottageArea = 1;
                NewCottagePrice = string.Empty;
                NewCottageCapacity = string.Empty;
                NewCottageEquipment = string.Empty;
                ErrorMessage = string.Empty;

                System.Diagnostics.Debug.WriteLine("Cottage added successfully");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding cottage: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error adding cottage: {ex.Message}");
            }
        }

        private void DeleteCottage(Cottage cottage)
        {
            try
            {
                _cottageService.DeleteCottage(cottage);
                LoadCottages();
                System.Diagnostics.Debug.WriteLine("Cottage deleted successfully");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting cottage: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error deleting cottage: {ex.Message}");
            }
        }

        private bool SetField<T>(ref T field, T value)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged();
            return true;
        }

        public void RefreshCottages()
        {
            LoadAreas();
            LoadCottages();
            LoadSearchAreas();
            SearchAvailableCottages();
            System.Diagnostics.Debug.WriteLine($"Cottages refreshed - Total: {Cottages.Count}");
        }
    }
}