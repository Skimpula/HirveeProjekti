using HirveeProjekti.ViewModels;

namespace HirveeProjekti.Views
{
    public partial class CustomersPage : ContentPage
    {
        private CustomerViewModel _viewModel;

        public CustomersPage()
        {
            InitializeComponent();
            _viewModel = new CustomerViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Reload customers when page appears
            _viewModel.RefreshCustomers();
        }
    }
}