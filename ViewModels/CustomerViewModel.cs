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

        // Lomakkeen kentät
        private string _etunimi;
        public string Etunimi
        {
            get => _etunimi;
            set
            {
                _etunimi = value;
                OnPropertyChanged();
            }
        }

        private string _sukunimi;
        public string Sukunimi
        {
            get => _sukunimi;
            set
            {
                _sukunimi = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _puhelinnro;
        public string Puhelinnro
        {
            get => _puhelinnro;
            set
            {
                _puhelinnro = value;
                OnPropertyChanged();
            }
        }

        // Komennot
        public ICommand AddCustomerCommand { get; }
        public ICommand RemoveCustomerCommand { get; }

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
            _customerService = new CustomerService(); // Oletetaan valmis palvelu

            // Lataa asiakkaat palvelusta
            LoadCustomers();

            AddCustomerCommand = new Command(AddCustomer);
            RemoveCustomerCommand = new Command<Customer>(RemoveCustomer);
        }

        private void LoadCustomers()
        {
            var list = _customerService.GetAllCustomers(); // Palauttaa List<Customer>
            Customers.Clear();
            foreach (var c in list)
            {
                Customers.Add(c);
            }
        }

        private void AddCustomer()
        {
            if (string.IsNullOrWhiteSpace(Etunimi) || string.IsNullOrWhiteSpace(Sukunimi))
                return;

            var newCustomer = new Customer
            {
                Etunimi = Etunimi,
                Sukunimi = Sukunimi,
                Email = Email,
                Puhelinnro = Puhelinnro
            };

            _customerService.AddCustomer(newCustomer);
            Customers.Add(newCustomer);

            // Tyhjennä lomake
            Etunimi = Sukunimi = Email = Puhelinnro = string.Empty;
        }

        private void RemoveCustomer(Customer customer)
        {
            if (customer == null) return;

            _customerService.DeleteCustomer(customer);
            Customers.Remove(customer);
        }
    }
}