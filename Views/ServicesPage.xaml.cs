using HirveeProjekti.ViewModels;

namespace HirveeProjekti.Views
{
    public partial class ServicesPage : ContentPage
    {
        private ServiceViewModel _viewModel;

        public ServicesPage()
        {
            InitializeComponent();
            _viewModel = new ServiceViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Reload services when page appears
            _viewModel.RefreshServices();
        }
    }
}