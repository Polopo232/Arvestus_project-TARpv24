using System.Collections.ObjectModel;
using System.Windows.Input;
using Arvestus_project_TARpv24.Models;
using Arvestus_project_TARpv24.Services;

namespace Arvestus_project_TARpv24.ViewModels
{
    public class DishDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly SessionService _sessionService;
        private Dish _dish;
        private int _newScore = 5;
        private string _newComment;

        public Dish Dish
        {
            get => _dish;
            set => SetProperty(ref _dish, value);
        }

        public int NewScore
        {
            get => _newScore;
            set => SetProperty(ref _newScore, value);
        }

        public string NewComment
        {
            get => _newComment;
            set => SetProperty(ref _newComment, value);
        }

        public SessionService Session => _sessionService;
        public ObservableCollection<Rating> Ratings { get; set; } = new ObservableCollection<Rating>();

        public ICommand AddRatingCommand { get; }

        public DishDetailViewModel(DatabaseService databaseService, SessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;

            AddRatingCommand = new Command(async () => await AddRatingAsync());
        }

        public async Task InitializeAsync(Dish dish)
        {
            if (dish == null)
            {
                System.Diagnostics.Debug.WriteLine("DishDetailViewModel.InitializeAsync: dish is null");
                return;
            }
            _dish = dish;
            OnPropertyChanged(nameof(Dish));
            await LoadRatingsAsync();
        }

        public async Task LoadRatingsAsync()
        {
            if (_dish == null) return;
            try
            {
                var list = await _databaseService.GetRatingsForDishAsync(_dish.Id);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Ratings.Clear();
                    foreach (var rating in list)
                    {
                        Ratings.Add(rating);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private async Task AddRatingAsync()
        {
            if (_dish == null)
            {
                await Shell.Current.DisplayAlert("Viga", "Toit pole valitud", "OK");
                return;
            }

            if (NewScore < 1 || NewScore > 5)
            {
                await Shell.Current.DisplayAlert("Viga", "Hinnang peab olema vahemikus 1 kuni 5", "OK");
                return;
            }

            try
            {
                var rating = new Rating
                {
                    DishId = _dish.Id,
                    Score = NewScore,
                    Comment = string.IsNullOrWhiteSpace(NewComment) ? "Без коментария" : NewComment,
                    Username = _sessionService.CurrentUser?.Username ?? "Külaline"
                };

                await _databaseService.SaveRatingAsync(rating);
                NewComment = string.Empty;
                await LoadRatingsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
