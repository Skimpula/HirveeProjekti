using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HirveeProjekti.Models;
using HirveeProjekti.Services;
using Microsoft.Maui.Controls;

namespace HirveeProjekti.ViewModels
{
    public class CustomerViewModel : BindableObject
    {
        private readonly CustomerService _customerService;

        // Lista asiakkaista
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();
        private List<Customer> _allCustomers = new List<Customer>();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchCustomers();
            }
        }

        public ICommand SearchCommand { get; }

        // Lomakkeen kentät
        private string _newCustomerFirstName = string.Empty;
        public string NewCustomerFirstName
        {
            get => _newCustomerFirstName;
            set
            {
                _newCustomerFirstName = value;
                OnPropertyChanged();
            }
        }

        private string _newCustomerLastName = string.Empty;
        public string NewCustomerLastName
        {
            get => _newCustomerLastName;
            set
            {
                _newCustomerLastName = value;
                OnPropertyChanged();
            }
        }

        private string _newCustomerAddress = string.Empty;
        public string NewCustomerAddress
        {
            get => _newCustomerAddress;
            set
            {
                _newCustomerAddress = value;
                OnPropertyChanged();
            }
        }

        private string _newCustomerPostalCode = string.Empty;
        public string NewCustomerPostalCode
        {
            get => _newCustomerPostalCode;
            set
            {
                _newCustomerPostalCode = value;
                OnPropertyChanged();
            }
        }

        private string _newCustomerEmail = string.Empty;
        public string NewCustomerEmail
        {
            get => _newCustomerEmail;
            set
            {
                _newCustomerEmail = value;
                OnPropertyChanged();
            }
        }

        private string _newCustomerPhone = string.Empty;
        public string NewCustomerPhone
        {
            get => _newCustomerPhone;
            set
            {
                _newCustomerPhone = value;
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

        // Komennot
        public ICommand AddCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }

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


        public CustomerViewModel()
        {
            _customerService = new CustomerService();

            // Lataa asiakkaat palvelusta
            LoadCustomers();

            AddCustomerCommand = new Command(AddCustomer);
            DeleteCustomerCommand = new Command<Customer>(DeleteCustomer);
            SearchCommand = new Command(SearchCustomers);
        }

        private void LoadCustomers()
        {
            try
            {
                var list = _customerService.GetAllCustomers();
                _allCustomers = list;
                SearchCustomers();
                System.Diagnostics.Debug.WriteLine($"Loaded {Customers.Count} customers");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading customers: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error loading customers: {ex.Message}");
            }
        }

        private void SearchCustomers()
        {
            Customers.Clear();
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allCustomers
                : _allCustomers.Where(c =>
                    (!string.IsNullOrEmpty(c.Etunimi) && c.Etunimi.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                      (!string.IsNullOrEmpty(c.Sukunimi) && c.Sukunimi.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                      (!string.IsNullOrEmpty(c.FullName) && c.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                  ).ToList();
            foreach (var c in filtered)
            {
                Customers.Add(c);
            }
        }

        private void AddCustomer()
        {
            try
            {
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(NewCustomerFirstName) || string.IsNullOrWhiteSpace(NewCustomerLastName))
                {
                    ErrorMessage = "Etunimi ja sukunimi ovat pakollisia.";
                    return;
                }

                var newCustomer = new Customer
                {
                    Etunimi = NewCustomerFirstName.Trim(),
                    Sukunimi = NewCustomerLastName.Trim(),
                    Lahiosoite = NewCustomerAddress?.Trim() ?? "",
                    Postinro = NewCustomerPostalCode?.Trim() ?? "00000",
                    Email = NewCustomerEmail?.Trim() ?? "",
                    Puhelinnro = NewCustomerPhone?.Trim() ?? ""
                };

                _customerService.AddCustomer(newCustomer);
                LoadCustomers();

                // Tyhjennä lomake
                NewCustomerFirstName = string.Empty;
                NewCustomerLastName = string.Empty;
                NewCustomerAddress = string.Empty;
                NewCustomerPostalCode = string.Empty;
                NewCustomerEmail = string.Empty;
                NewCustomerPhone = string.Empty;

                System.Diagnostics.Debug.WriteLine("Customer added successfully");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding customer: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error adding customer: {ex.Message}");
            }
        }

        private void DeleteCustomer(Customer customer)
        {
            try
            {
                if (customer == null) return;

                _customerService.DeleteCustomer(customer);
                LoadCustomers();

                System.Diagnostics.Debug.WriteLine("Customer deleted successfully");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting customer: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error deleting customer: {ex.Message}");
            }
        }

        public void RefreshCustomers()
        {
            LoadCustomers();
            System.Diagnostics.Debug.WriteLine($"Customers refreshed - Total: {Customers.Count}");
        }
    }
}