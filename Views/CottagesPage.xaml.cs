using HirveeProjekti.ViewModels;

namespace HirveeProjekti.Views
{
    public partial class CottagesPage : ContentPage
    {
        public CottagesPage()
        {
            InitializeComponent();
            BindingContext = new CottageViewModel();
        }
    }
}