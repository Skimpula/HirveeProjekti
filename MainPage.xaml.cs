using System;
using Microsoft.Maui.Controls;
using HirveeProjekti.Views;

namespace HirveeProjekti
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void GoToAreas(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AreasPage());
        }

        private async void GoToCottages(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CottagesPage());
        }

        private async void GoToCustomers(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CustomersPage());
        }

        private async void GoToBookings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BookingsPage());
        }

        private async void GoToServices(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ServicesPage());
        }

        private async void GoToInvoices(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InvoicesPage());
        }

        private async void GoToReports(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BookingReportsPage());
        }
    }
}