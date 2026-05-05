using HirveeProjekti.ViewModels;

namespace HirveeProjekti.Views;

public partial class BookingReportsPage : ContentPage
{
    private BookingReportsViewModel _viewModel;

    public BookingReportsPage()
    {
        InitializeComponent();
        _viewModel = new BookingReportsViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Refresh();
    }
}
