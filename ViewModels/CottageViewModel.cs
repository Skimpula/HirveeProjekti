using System.Collections.ObjectModel;
using System.Windows.Input;
using HirveeProjekti.Models;
using Microsoft.Maui.Controls;

namespace HirveeProjekti.ViewModels
{
    public class CottageViewModel : BindableObject
    {
        private readonly CottageData _cottageData = new CottageData();

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

        private string _newCottageArea = string.Empty;
        public string NewCottageArea
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
            DeleteCottageCommand = new Command<int>(DeleteCottage);

            SeedCottages();
            LoadCottages();
        }

        private void SeedCottages()
        {
            if (_cottageData.GetAll().Count > 0)
            {
                return;
            }

            _cottageData.Add(new Cottage
            {
                Mokkinimi = "Rantamaja",
                Katuosoite = "Järventie 12",
                AreaName = "Ruka",
                Hinta = 129.00,
                Henkilomaara = 4,
                Varustelu = "Sähkö, sauna, wifi"
            });

            _cottageData.Add(new Cottage
            {
                Mokkinimi = "Metsäkumpu",
                Katuosoite = "Honkatie 8",
                AreaName = "Tahko",
                Hinta = 149.00,
                Henkilomaara = 6,
                Varustelu = "Takka, vene, grilli"
            });
        }

        private void LoadCottages()
        {
            Cottages.Clear();

            foreach (var cottage in _cottageData.GetAll())
            {
                Cottages.Add(cottage);
            }
        }

        private void AddCottage()
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
                Mokkinimi = NewCottageName.Trim(),
                Katuosoite = NewCottageAddress.Trim(),
                AreaName = NewCottageArea.Trim(),
                Hinta = price,
                Henkilomaara = capacity,
                Varustelu = NewCottageEquipment.Trim()
            };

            _cottageData.Add(cottage);
            LoadCottages();

            NewCottageName = string.Empty;
            NewCottageAddress = string.Empty;
            NewCottageArea = string.Empty;
            NewCottagePrice = string.Empty;
            NewCottageCapacity = string.Empty;
            NewCottageEquipment = string.Empty;
            ErrorMessage = string.Empty;
        }

        private void DeleteCottage(int id)
        {
            _cottageData.Delete(id);
            LoadCottages();
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
    }
}