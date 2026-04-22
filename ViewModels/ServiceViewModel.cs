using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HirveeProjekti.Models;
using HirveeProjekti.Services;
using Microsoft.Maui.Controls;

namespace HirveeProjekti.ViewModels
{
    public class ServiceViewModel : BindableObject
    {
        private readonly ServiceService _serviceService;

        public ObservableCollection<Service> Services { get; set; } = new ObservableCollection<Service>();

        private string _newServiceName = string.Empty;
        public string NewServiceName
        {
            get => _newServiceName;
            set
            {
                _newServiceName = value;
                OnPropertyChanged();
            }
        }

        private string _newServiceDescription = string.Empty;
        public string NewServiceDescription
        {
            get => _newServiceDescription;
            set
            {
                _newServiceDescription = value;
                OnPropertyChanged();
            }
        }

        private int _newServiceArea = 1;
        public int NewServiceArea
        {
            get => _newServiceArea;
            set
            {
                _newServiceArea = value;
                OnPropertyChanged();
            }
        }

        private string _newServicePrice = string.Empty;
        public string NewServicePrice
        {
            get => _newServicePrice;
            set
            {
                _newServicePrice = value;
                OnPropertyChanged();
            }
        }

        private string _newServiceVAT = string.Empty;
        public string NewServiceVAT
        {
            get => _newServiceVAT;
            set
            {
                _newServiceVAT = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public ICommand AddServiceCommand { get; }
        public ICommand DeleteServiceCommand { get; }

        public ServiceViewModel()
        {
            _serviceService = new ServiceService();
            LoadServices();

            AddServiceCommand = new Command(AddService);
            DeleteServiceCommand = new Command<Service>(DeleteService);
        }

        private void LoadServices()
        {
            try
            {
                Services.Clear();
                var services = _serviceService.GetAllServices();
                foreach (var service in services)
                {
                    Services.Add(service);
                }
                System.Diagnostics.Debug.WriteLine($"Loaded {Services.Count} services into ViewModel");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading services: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error loading services: {ex.Message}");
            }
        }

        private void AddService()
        {
            try
            {
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(NewServiceName) ||
                    string.IsNullOrWhiteSpace(NewServicePrice) ||
                    string.IsNullOrWhiteSpace(NewServiceVAT))
                {
                    ErrorMessage = "Nimi, hinta ja ALV ovat pakollisia.";
                    return;
                }

                if (!double.TryParse(NewServicePrice, out var price))
                {
                    ErrorMessage = "Hinta pitää olla numero.";
                    return;
                }

                if (!double.TryParse(NewServiceVAT, out var vat))
                {
                    ErrorMessage = "ALV pitää olla numero.";
                    return;
                }

                var newService = new Service
                {
                    AlueId = NewServiceArea,
                    Nimi = NewServiceName.Trim(),
                    Kuvaus = NewServiceDescription?.Trim() ?? "",
                    Hinta = price,
                    Alv = vat
                };

                _serviceService.AddService(newService);
                LoadServices();

                // Clear form
                NewServiceName = string.Empty;
                NewServiceDescription = string.Empty;
                NewServiceArea = 1;
                NewServicePrice = string.Empty;
                NewServiceVAT = string.Empty;

                System.Diagnostics.Debug.WriteLine("Service added successfully");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding service: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error adding service: {ex.Message}");
            }
        }

        private void DeleteService(Service service)
        {
            try
            {
                if (service == null) return;

                _serviceService.DeleteService(service);
                LoadServices();

                System.Diagnostics.Debug.WriteLine("Service deleted successfully");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting service: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error deleting service: {ex.Message}");
            }
        }

        public void RefreshServices()
        {
            LoadServices();
            System.Diagnostics.Debug.WriteLine($"Services refreshed - Total: {Services.Count}");
        }
    }
}