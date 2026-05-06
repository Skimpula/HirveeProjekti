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

        _viewModel.RefreshBookings();   
        _viewModel.RefreshCalendar();   
        BuildCalendar();                
    }

    private void BuildCalendar()
    {
        var grid = this.FindByName<Grid>("CalendarGrid");
        if (grid == null) return;

        grid.Children.Clear();
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();

   
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 120 });

        foreach (var date in _viewModel.CalendarDates)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 60 });
        }

     
        grid.RowDefinitions.Add(new RowDefinition { Height = 40 });

       
        int col = 1;
        foreach (var date in _viewModel.CalendarDates)
        {
            grid.Add(new Label
            {
                Text = date.ToString("dd.MM"),
                FontSize = 10,
                HorizontalTextAlignment = TextAlignment.Center
            }, col, 0);

            col++;
        }

        int row = 1;

        foreach (var cottage in _viewModel.CalendarCottages)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = 40 });

            
            grid.Add(new Label
            {
                Text = cottage.Mokkinimi,
                TextColor = Colors.Black
            }, 0, row);

            int dateCol = 1;

            foreach (var date in _viewModel.CalendarDates)
            {
                
                var booking =
                    _viewModel.CalendarBookings.ContainsKey(date) &&
                    _viewModel.CalendarBookings[date].ContainsKey(cottage.MokkiId)
                    ? _viewModel.CalendarBookings[date][cottage.MokkiId]
                    : null;

                var cell = new Frame
                {
                    BackgroundColor = booking != null
                        ? Color.FromArgb("#EF5350")
                        : Color.FromArgb("#66BB6A"),

                    CornerRadius = 6,
                    Margin = 2,
                    Padding = 0,
                    HasShadow = false,

                    Content = new Label
                    {
                        Text = booking != null ? "X" : "",
                        FontSize = 11,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.White
                    }
                };

                grid.Add(cell, dateCol, row);
                dateCol++;
            }

            row++;
        }
    }
}