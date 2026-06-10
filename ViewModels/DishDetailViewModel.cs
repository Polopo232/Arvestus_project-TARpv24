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
        private readonly LocalizationService _localization;
        private readonly PhotoService _photoService;
        private Dish _dish;
        private int _newScore = 5;
        private string _newComment;
        private string _newImagePath;

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

        public string NewImagePath
        {
            get => _newImagePath;
            set => SetProperty(ref _newImagePath, value, onChanged: () => OnPropertyChanged(nameof(HasNewImage)));
        }

        public bool HasNewImage => !string.IsNullOrWhiteSpace(_newImagePath);

        public SessionService Session => _sessionService;
        public ObservableCollection<Rating> Ratings { get; set; } = new ObservableCollection<Rating>();

        public ICommand AddRatingCommand { get; }
        public ICommand PickPhotoCommand { get; }

        public DishDetailViewModel(DatabaseService databaseService, SessionService sessionService, LocalizationService localization, PhotoService photoService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            _localization = localization;
            _photoService = photoService;

            AddRatingCommand = new Command(async () => await AddRatingAsync());
            PickPhotoCommand = new Command(async () => await PickPhotoAsync());
        }

        private async Task PickPhotoAsync()
        {
            var path = await _photoService.PickPhotoAsync();
            if (path != null)
                NewImagePath = path;
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

                    _dish.RatingCount = list.Count;
                    _dish.AverageRating = list.Count > 0
                        ? Math.Round(list.Average(r => r.Score), 1)
                        : 0;
                    OnPropertyChanged(nameof(Dish));
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
                await Shell.Current.DisplayAlert(_localization["ErrorTitle"], _localization["DishNotSelected"], _localization["OkButton"]);
                return;
            }

            if (NewScore < 1 || NewScore > 5)
            {
                await Shell.Current.DisplayAlert(_localization["ErrorTitle"], _localization["RatingOutOfRange"], _localization["OkButton"]);
                return;
            }

            try
            {
                var rating = new Rating
                {
                    DishId = _dish.Id,
                    Score = NewScore,
                    Comment = string.IsNullOrWhiteSpace(NewComment) ? string.Empty : NewComment,
                    Username = _sessionService.CurrentUser?.Username ?? "Külaline",
                    ImagePath = NewImagePath
                };

                await _databaseService.SaveRatingAsync(rating);
                NewComment = string.Empty;
                NewImagePath = string.Empty;
                NewScore = 5;
                await LoadRatingsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
