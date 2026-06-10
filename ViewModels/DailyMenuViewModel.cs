using System.Collections.ObjectModel;
using System.Windows.Input;
using Arvestus_project_TARpv24.Models;
using Arvestus_project_TARpv24.Services;
using Arvestus_project_TARpv24.Views;

namespace Arvestus_project_TARpv24.ViewModels
{
    public class DailyMenuViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly SessionService _sessionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly SoundService _soundService;
        private readonly LocalizationService _localization;
        private DateTime _selectedDate = DateTime.Today;
        private Dish _selectedDish;
        private bool _isSelectionMode;

        public SessionService Session => _sessionService;
        public ObservableCollection<Dish> DailyDishes { get; set; } = new ObservableCollection<Dish>();
        public ObservableCollection<Dish> AllDishes { get; set; } = new ObservableCollection<Dish>();

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                SetProperty(ref _selectedDate, value);
                _ = LoadDailyMenuAsync();
            }
        }

        public Dish SelectedDish
        {
            get => _selectedDish;
            set => SetProperty(ref _selectedDish, value);
        }

        public bool IsSelectionMode
        {
            get => _isSelectionMode;
            set => SetProperty(ref _isSelectionMode, value);
        }

        public ICommand LoadDailyMenuCommand { get; }
        public ICommand SelectDishCommand { get; }
        public ICommand AddSelectedDishToDailyCommand { get; }
        public ICommand RemoveFromDailyCommand { get; }
        public ICommand ToggleSelectionModeCommand { get; }

        public DailyMenuViewModel(DatabaseService databaseService, SessionService sessionService, IServiceProvider serviceProvider, SoundService soundService, LocalizationService localization)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            _serviceProvider = serviceProvider;
            _soundService = soundService;
            _localization = localization;

            LoadDailyMenuCommand = new Command(async () => await LoadDailyMenuAsync());
            SelectDishCommand = new Command<Dish>(async (dish) => await OpenDishDetailAsync(dish));
            AddSelectedDishToDailyCommand = new Command(async () => await AddSelectedDishToDailyAsync());
            RemoveFromDailyCommand = new Command<Dish>(async (dish) => await RemoveDishFromDailyAsync(dish));
            ToggleSelectionModeCommand = new Command(() => IsSelectionMode = !IsSelectionMode);

            _ = LoadDailyMenuAsync();
            _ = LoadAllDishesAsync();
        }

        public async Task LoadDailyMenuAsync()
        {
            try
            {
                var list = await _databaseService.GetDailyMenuDishesAsync(SelectedDate);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DailyDishes.Clear();
                    foreach (var dish in list)
                    {
                        DailyDishes.Add(dish);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public async Task LoadAllDishesAsync()
        {
            try
            {
                var list = await _databaseService.GetDishesAsync();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AllDishes.Clear();
                    foreach (var dish in list)
                    {
                        AllDishes.Add(dish);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private async Task AddSelectedDishToDailyAsync()
        {
            if (SelectedDish == null)
            {
                await Shell.Current.DisplayAlert(_localization["ErrorTitle"], _localization["SelectDishFirst"], _localization["OkButton"]);
                return;
            }

            try
            {
                var newDaily = new DailyMenu
                {
                    DishId = SelectedDish.Id,
                    Date = SelectedDate
                };
                await _databaseService.SaveDailyMenuAsync(newDaily);
                await _soundService.PlaySuccessSoundAsync();
                SelectedDish = null;
                await LoadDailyMenuAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private async Task RemoveDishFromDailyAsync(Dish dish)
        {
            if (dish == null) return;
            try
            {
                await _databaseService.DeleteDailyMenuAsync(dish.Id, SelectedDate);
                await _soundService.PlayClickSoundAsync();
                await LoadDailyMenuAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private async Task OpenDishDetailAsync(Dish dish)
        {
            if (dish == null) return;
            await _soundService.PlayClickSoundAsync();
            var page = _serviceProvider.GetRequiredService<DishDetailPage>();
            var viewModel = (DishDetailViewModel)page.BindingContext;
            await viewModel.InitializeAsync(dish);
            await Shell.Current.Navigation.PushAsync(page);
        }
    }
}
