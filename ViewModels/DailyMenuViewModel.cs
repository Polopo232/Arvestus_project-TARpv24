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
        private DateTime _selectedDate = DateTime.Today;
        private Dish _selectedDish;

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

        public ICommand LoadDailyMenuCommand { get; }
        public ICommand SelectDishCommand { get; }
        public ICommand AddSelectedDishToDailyCommand { get; }

        public DailyMenuViewModel(DatabaseService databaseService, SessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;

            LoadDailyMenuCommand = new Command(async () => await LoadDailyMenuAsync());
            SelectDishCommand = new Command<Dish>(async (dish) => await OpenDishDetailAsync(dish));
            AddSelectedDishToDailyCommand = new Command(async () => await AddSelectedDishToDailyAsync());

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
                await Shell.Current.DisplayAlert("Viga", "Palun valige toit nimekirjast", "OK");
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
                SelectedDish = null;
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
            await Shell.Current.Navigation.PushAsync(new DishDetailPage(dish, _databaseService, _sessionService));
        }
    }
}