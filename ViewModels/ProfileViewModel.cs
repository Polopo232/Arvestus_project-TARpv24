using System.Windows.Input;
using Arvestus_project_TARpv24.Services;

namespace Arvestus_project_TARpv24.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly SessionService _sessionService;
        private readonly LocalizationService _localization;
        private string _username;
        private string _password;

        public SessionService Session => _sessionService;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string CurrentStatus
        {
            get
            {
                var user = _sessionService.CurrentUser;
                return user == null
                    ? _localization["GuestStatus"]
                    : string.Format(_localization["UserStatus"], user.Username, user.Role);
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand LogoutCommand { get; }

        public ProfileViewModel(DatabaseService databaseService, SessionService sessionService, LocalizationService localization)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;
            _localization = localization;

            LoginCommand = new Command(async () => await LoginAsync());
            RegisterCommand = new Command(async () => await RegisterAsync());
            LogoutCommand = new Command(Logout);

            _localization.PropertyChanged += (_, __) => OnPropertyChanged(nameof(CurrentStatus));
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert(_localization["ErrorTitle"], _localization["FillAllFields"], _localization["OkButton"]);
                return;
            }

            var user = await _databaseService.LoginAsync(Username, Password);
            if (user != null)
            {
                _sessionService.CurrentUser = user;
                OnPropertyChanged(nameof(CurrentStatus));
                Username = string.Empty;
                Password = string.Empty;
            }
            else
            {
                await Shell.Current.DisplayAlert(_localization["ErrorTitle"], _localization["WrongCredentials"], _localization["OkButton"]);
            }
        }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert(_localization["ErrorTitle"], _localization["FillAllFields"], _localization["OkButton"]);
                return;
            }

            var success = await _databaseService.RegisterUserAsync(Username, Password);
            if (success)
            {
                await Shell.Current.DisplayAlert(_localization["SuccessTitle"], _localization["UserRegistered"], _localization["OkButton"]);
                await LoginAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert(_localization["ErrorTitle"], _localization["UsernameTaken"], _localization["OkButton"]);
            }
        }

        private void Logout()
        {
            _sessionService.CurrentUser = null;
            OnPropertyChanged(nameof(CurrentStatus));
        }
    }
}