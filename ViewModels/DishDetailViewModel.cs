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

        public DishDetailViewModel(Dish dish, DatabaseService databaseService, SessionService sessionService)
        {
            _dish = dish;
            _databaseService = databaseService;
            _sessionService = sessionService;

            AddRatingCommand = new Command(async () => await AddRatingAsync());

            _ = LoadRatingsAsync();
        }

        public async Task LoadRatingsAsync()
        {
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
                    Comment = NewComment
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