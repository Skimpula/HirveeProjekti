using HirveeProjekti.ViewModels;

namespace HirveeProjekti.Views
{
    public partial class CottagesPage : ContentPage
    {
        private CottageViewModel _viewModel;

        public CottagesPage()
        {
            InitializeComponent();
            _viewModel = new CottageViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Reload cottages when page appears
            _viewModel.RefreshCottages();
        }
    }
}