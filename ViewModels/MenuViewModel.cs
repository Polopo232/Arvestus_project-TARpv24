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
        private readonly SessionService _sessionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly SoundService _soundService;
        private List<Dish> _allDishes = new List<Dish>();
        private string _searchText;

        public SessionService Session => _sessionService;
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
        public ICommand AddDishCommand { get; }
        public ICommand SelectDishCommand { get; }

        public MenuViewModel(DatabaseService databaseService, SessionService sessionService, IServiceProvider serviceProvider, SoundService soundService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            _serviceProvider = serviceProvider;
            _soundService = soundService;

            LoadDishesCommand = new Command(async () => await LoadDishesAsync());
            AddDishCommand = new Command(async () => await NavigateToAddDishAsync());
            SelectDishCommand = new Command<Dish>(async (dish) => await OpenDishDetailAsync(dish));
        }

        public async Task LoadDishesAsync()
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

        private async Task NavigateToAddDishAsync()
        {
            var addDishPage = _serviceProvider.GetRequiredService<AddDishPage>();
            await Shell.Current.Navigation.PushAsync(addDishPage);
        }

        private async Task OpenDishDetailAsync(Dish dish)
        {
            if (dish == null) return;
            await _soundService.PlayClickSoundAsync();
            var viewModel = _serviceProvider.GetRequiredService<DishDetailViewModel>();
            await viewModel.InitializeAsync(dish);
            var page = _serviceProvider.GetRequiredService<DishDetailPage>();
            await Shell.Current.Navigation.PushAsync(page);
        }
    }
}