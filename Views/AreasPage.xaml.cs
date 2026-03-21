using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using HirveeProjekti.Models;

namespace HirveeProjekti.Views
{
    public partial class AreasPage : ContentPage
    {
        public ObservableCollection<Area> Areas { get; set; } = new ObservableCollection<Area>();

        public AreasPage()
        {
            InitializeComponent();

            // Example data
            Areas.Add(new Area { Name = "Ruka", Description = "Ski resort area" });
            Areas.Add(new Area { Name = "Tahko", Description = "Family-friendly destination" });
            Areas.Add(new Area { Name = "Ylläs", Description = "Northern wilderness area" });

            AreasCollectionView.ItemsSource = Areas;
        }

        private async void AddArea_Clicked(object sender, EventArgs e)
        {
            // TODO: Open a dialog or new page to add a new area
            await DisplayAlert("Add Area", "Functionality to add a new area goes here.", "OK");
        }

        private async void EditArea_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var area = button?.BindingContext as Area;
            if (area != null)
            {
                // TODO: Open a dialog or page to edit area details
                await DisplayAlert("Edit Area", $"Edit {area.Name}", "OK");
            }
        }

        private async void DeleteArea_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var area = button?.BindingContext as Area;
            if (area != null)
            {
                bool confirm = await DisplayAlert("Confirm Delete", $"Delete {area.Name}?", "Yes", "No");
                if (confirm)
                {
                    Areas.Remove(area);
                }
            }
        }
    }
}