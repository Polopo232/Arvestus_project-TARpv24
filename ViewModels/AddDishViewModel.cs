using System.Windows.Input;
using Arvestus_project_TARpv24.Models;
using Arvestus_project_TARpv24.Services;

namespace Arvestus_project_TARpv24.ViewModels
{
    public class AddDishViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly LocalizationService _localization;
        private readonly PhotoService _photoService;
        private string _name;
        private string _description;
        private string _category;
        private string _allergens;
        private string _imagePath;

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

        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value, onChanged: () => OnPropertyChanged(nameof(HasImage)));
        }

        public bool HasImage => !string.IsNullOrWhiteSpace(_imagePath);

        public ICommand SaveDishCommand { get; }
        public ICommand PickPhotoCommand { get; }

        public AddDishViewModel(DatabaseService databaseService, LocalizationService localization, PhotoService photoService)
        {
            _databaseService = databaseService;
            _localization = localization;
            _photoService = photoService;
            SaveDishCommand = new Command(async () => await SaveDishAsync());
            PickPhotoCommand = new Command(async () => await PickPhotoAsync());
        }

        private async Task PickPhotoAsync()
        {
            var path = await _photoService.PickPhotoAsync();
            if (path != null)
                ImagePath = path;
        }

        private async Task SaveDishAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Category))
            {
                await Shell.Current.DisplayAlert(_localization["ErrorTitle"], _localization["FillNameCategory"], _localization["OkButton"]);
                return;
            }

            try
            {
                var newDish = new Dish
                {
                    Name = Name,
                    Description = Description,
                    Category = Category,
                    Allergens = string.IsNullOrWhiteSpace(Allergens) ? "Puuduvad" : Allergens,
                    ImagePath = ImagePath
                };

                await _databaseService.SaveDishAsync(newDish);

                Name = string.Empty;
                Description = string.Empty;
                Category = string.Empty;
                Allergens = string.Empty;
                ImagePath = string.Empty;

                await Shell.Current.DisplayAlert(_localization["SuccessTitle"], _localization["DishSaved"], _localization["OkButton"]);
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(_localization["ErrorTitle"], ex.Message, _localization["OkButton"]);
            }
        }
    }
}