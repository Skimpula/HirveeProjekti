namespace HirveeProjekti.Models
{
    public class SelectableService : BindableObject
    {
        private bool _isSelected;
        private int _quantity = 1;

        public Service Service { get; init; } = null!;

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public int Quantity
        {
            get => _quantity;
            set { _quantity = Math.Max(1, value); OnPropertyChanged(); }
        }

        public string DisplayName => $"{Service?.Nimi}  –  {Service?.Hinta:F2} €";
    }
}
