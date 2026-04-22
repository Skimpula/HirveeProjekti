using HirveeProjekti.ViewModels;

namespace HirveeProjekti.Views
{
    public partial class InvoicesPage : ContentPage
    {
        private InvoiceViewModel _viewModel;

        public InvoicesPage()
        {
            InitializeComponent();
            _viewModel = new InvoiceViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Reload invoices when page appears
            _viewModel.RefreshInvoices();
        }
    }
}