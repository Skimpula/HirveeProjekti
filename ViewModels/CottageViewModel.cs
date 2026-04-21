using System.Collections.ObjectModel;
using System.Windows.Input;
using HirveeProjekti.Models;
using HirveeProjekti.Services;
using Microsoft.Maui.Controls;

namespace HirveeProjekti.ViewModels
{
    public class CottageViewModel : BindableObject
    {
        private readonly CottageService _cottageService = new CottageService();

        public ObservableCollection<Cottage> Cottages { get; } = new();

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

        public ICommand AddCottageCommand { get; }
        public ICommand DeleteCottageCommand { get; }

        public CottageViewModel()
        {
            AddCottageCommand = new Command(AddCottage);
            DeleteCottageCommand = new Command<Cottage>(DeleteCottage);

            LoadCottages();
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
            LoadCottages();
            System.Diagnostics.Debug.WriteLine($"Cottages refreshed - Total: {Cottages.Count}");
        }
    }
}