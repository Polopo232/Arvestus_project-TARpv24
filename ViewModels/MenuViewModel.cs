using System.Collections.ObjectModel;
using System.Windows.Input;
using Arvestus_project_TARpv24.Models;
using Arvestus_project_TARpv24.Services;
using Arvestus_project_TARpv24.Views;

namespace Arvestus_project_TARpv24.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private List<Dish> _allDishes = new List<Dish>();
        private string _searchText;

        public ObservableCollection<Dish> Dishes { get; set; } = new ObservableCollection<Dish>();

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterDishes();
            }
        }

        public ICommand LoadDishesCommand { get; }
        public ICommand AddTestDishCommand { get; }
        public ICommand SelectDishCommand { get; }

        public MenuViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            LoadDishesCommand = new Command(async () => await LoadDishesAsync());
            AddTestDishCommand = new Command(async () => await AddTestDishAsync());
            SelectDishCommand = new Command<Dish>(async (dish) => await OpenDishDetailAsync(dish));

            _ = LoadDishesAsync();
        }

        private async Task LoadDishesAsync()
        {
            try
            {
                _allDishes = await _databaseService.GetDishesAsync();
                FilterDishes();
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.DisplayAlert("Viga laadimisel", ex.Message, "OK");
                });
            }
        }

        private void FilterDishes()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Dishes.Clear();
                var filtered = string.IsNullOrWhiteSpace(SearchText)
                    ? _allDishes
                    : _allDishes.Where(d => d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                            d.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var dish in filtered)
                {
                    Dishes.Add(dish);
                }
            });
        }

        private async Task AddTestDishAsync()
        {
            try
            {
                var newDish = new Dish
                {
                    Name = "Päevasupp (Tomati)",
                    Description = "Maitsev kodune supp koore ja saiakuubikutega.",
                    Category = "Supid",
                    Allergens = "Laktoos, Gluteen"
                };

                await _databaseService.SaveDishAsync(newDish);
                await LoadDishesAsync();
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.DisplayAlert("Viga lisamisel", ex.Message, "OK");
                });
            }
        }

        private async Task OpenDishDetailAsync(Dish dish)
        {
            if (dish == null) return;
            await Shell.Current.Navigation.PushAsync(new DishDetailPage(dish, _databaseService));
        }
    }
}