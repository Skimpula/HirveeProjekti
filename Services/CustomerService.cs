using System.Collections.Generic;
using HirveeProjekti.Models;

namespace HirveeProjekti.Services
{
    public class CustomerService
    {
        // Yksinkertainen lista asiakkaista muistissa
        private List<Customer> _customers = new List<Customer>();

        public CustomerService()
        {
            // Esimerkkiasiakkaita alustukseksi
            _customers.Add(new Customer { AsiakasId = 1, Etunimi = "Matti", Sukunimi = "Meikäläinen", Email = "matti@example.com", Puhelinnro = "0401234567" });
            _customers.Add(new Customer { AsiakasId = 2, Etunimi = "Maija", Sukunimi = "Mallikas", Email = "maija@example.com", Puhelinnro = "0507654321" });
        }

        // Hae kaikki asiakkaat
        public List<Customer> GetAllCustomers()
        {
            return _customers;
        }

        // Lisää asiakas
        public void AddCustomer(Customer customer)
        {
            // Annetaan yksinkertainen automaattinen ID
            customer.AsiakasId = _customers.Count > 0 ? _customers[_customers.Count - 1].AsiakasId + 1 : 1;
            _customers.Add(customer);
        }

        // Poista asiakas
        public void DeleteCustomer(Customer customer)
        {
            _customers.Remove(customer);
        }
    }
}