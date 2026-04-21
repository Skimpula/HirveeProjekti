using HirveeProjekti.ViewModels;

namespace HirveeProjekti.Views;

public partial class BookingsPage : ContentPage
{
    public BookingsPage()
    {
        InitializeComponent();
        BindingContext = new BookingViewModel();
    }
}