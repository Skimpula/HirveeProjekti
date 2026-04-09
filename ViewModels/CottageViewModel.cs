using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HirveeProjekti.Models;
using HirveeProjekti.Services;
using Microsoft.Maui.Controls;

namespace HirveeProjekti.ViewModels
{
    public class CottageViewModel
    {
        public ObservableCollection<Cottage> Cottages { get; set; } = new();
        public ObservableCollection<Area> Areas { get; set; } = new();

        public Cottage SelectedCottage { get; set; }
        public Area SelectedArea { get; set; }

        public string NewCottageName { get; set; }
        public string NewCottageAddress { get; set; }
        public string NewCottagePrice { get; set; }
        public string NewCottageCapacity { get; set; }
        public string NewCottageEquipment { get; set; }

        public string ErrorMessage { get; set; }
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand AddCottageCommand { get; }
        public ICommand DeleteCottageCommand { get; }

        public CottageViewModel()
        {
            AddCottageCommand = new Command(AddCottage);
            DeleteCottageCommand = new Command<int>(DeleteCottage);

            // Lataa mökit ja alueet DB:stä
        }

        private void AddCottage() { /* toteutus */ }
        private void DeleteCottage(int id) { /* toteutus */ }
    }
}