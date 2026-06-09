using System.Collections.ObjectModel;
using System.Windows.Input;
using Arvestus_project_TARpv24.Models;
using Arvestus_project_TARpv24.Services;

namespace Arvestus_project_TARpv24.ViewModels
{
    public class DishDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private string _newComment;
        private int _newScore = 5;

        public Dish SelectedDish { get; }
        public ObservableCollection<Rating> Ratings { get; set; } = new ObservableCollection<Rating>();

        public string NewComment
        {
            get => _newComment;
            set => SetProperty(ref _newComment, value);
        }

        public int NewScore
        {
            get => _newScore;
            set => SetProperty(ref _newScore, value);
        }

        public ICommand LoadRatingsCommand { get; }
        public ICommand AddRatingCommand { get; }

        public DishDetailViewModel(Dish dish, DatabaseService databaseService)
        {
            SelectedDish = dish;
            _databaseService = databaseService;

            LoadRatingsCommand = new Command(async () => await LoadRatingsAsync());
            AddRatingCommand = new Command(async () => await AddRatingAsync());

            _ = LoadRatingsAsync();
        }

        public async Task LoadRatingsAsync()
        {
            try
            {
                var list = await _databaseService.GetRatingsForDishAsync(SelectedDish.Id);
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
            if (string.IsNullOrWhiteSpace(NewComment)) return;

            try
            {
                var rating = new Rating
                {
                    DishId = SelectedDish.Id,
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