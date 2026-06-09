using System.Windows.Input;
using Arvestus_project_TARpv24.Models;
using Arvestus_project_TARpv24.Services;

namespace Arvestus_project_TARpv24.ViewModels
{
    public class AddDishViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private string _name;
        private string _description;
        private string _category;
        private string _allergens;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public string Allergens
        {
            get => _allergens;
            set => SetProperty(ref _allergens, value);
        }

        public ICommand SaveDishCommand { get; }

        public AddDishViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            SaveDishCommand = new Command(async () => await SaveDishAsync());
        }

        private async Task SaveDishAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Category))
            {
                await Shell.Current.DisplayAlert("Viga", "Nimi ja Kategooria on kohustuslikud väljad", "OK");
                return;
            }

            try
            {
                var newDish = new Dish
                {
                    Name = Name,
                    Description = Description,
                    Category = Category,
                    Allergens = string.IsNullOrWhiteSpace(Allergens) ? "Puuduvad" : Allergens
                };

                await _databaseService.SaveDishAsync(newDish);

                Name = string.Empty;
                Description = string.Empty;
                Category = string.Empty;
                Allergens = string.Empty;

                await Shell.Current.DisplayAlert("Edu", "Toit on edukalt lisatud menüüsse", "OK");
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Viga", ex.Message, "OK");
            }
        }
    }
}