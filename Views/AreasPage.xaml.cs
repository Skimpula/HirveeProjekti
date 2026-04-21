using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using HirveeProjekti.Models;
using HirveeProjekti.Services;

namespace HirveeProjekti.Views
{
    public partial class AreasPage : ContentPage
    {
        private AreaService _areaService;
        public ObservableCollection<Area> Areas { get; set; } = new ObservableCollection<Area>();

        public AreasPage()
        {
            InitializeComponent();
            _areaService = new AreaService();
            LoadAreas();
            AreasCollectionView.ItemsSource = Areas;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Reload areas when page appears
            LoadAreas();
        }

        private void LoadAreas()
        {
            try
            {
                Areas.Clear();
                var areas = _areaService.GetAllAreas();
                foreach (var area in areas)
                {
                    Areas.Add(area);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to load areas: {ex.Message}", "OK");
            }
        }

        private async void AddArea_Clicked(object sender, EventArgs e)
        {
            string areaName = await DisplayPromptAsync("Add Area", "Enter area name:");
            
            if (!string.IsNullOrWhiteSpace(areaName))
            {
                var newArea = new Area { Nimi = areaName };
                _areaService.AddArea(newArea);
                LoadAreas();
                await DisplayAlert("Success", "Area added successfully", "OK");
            }
        }

        private async void EditArea_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var area = button?.BindingContext as Area;
            if (area != null)
            {
                string newName = await DisplayPromptAsync("Edit Area", "Enter new area name:", initialValue: area.Nimi);
                
                if (!string.IsNullOrWhiteSpace(newName) && newName != area.Nimi)
                {
                    area.Nimi = newName;
                    _areaService.UpdateArea(area);
                    LoadAreas();
                    await DisplayAlert("Success", "Area updated successfully", "OK");
                }
            }
        }

        private async void DeleteArea_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var area = button?.BindingContext as Area;
            if (area != null)
            {
                bool confirm = await DisplayAlert("Confirm Delete", $"Delete {area.Nimi}?", "Yes", "No");
                if (confirm)
                {
                    _areaService.DeleteArea(area);
                    LoadAreas();
                    await DisplayAlert("Success", "Area deleted successfully", "OK");
                }
            }
        }
    }
}