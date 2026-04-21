using HirveeProjekti.ViewModels;

namespace HirveeProjekti.Views;

public partial class BookingsPage : ContentPage
{
    private BookingViewModel _viewModel;

    public BookingsPage()
    {
        InitializeComponent();
        _viewModel = new BookingViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Reload bookings when page appears
        _viewModel.RefreshBookings();
    }
}