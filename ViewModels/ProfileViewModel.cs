using System.Windows.Input;
using Arvestus_project_TARpv24.Services;

namespace Arvestus_project_TARpv24.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly SessionService _sessionService;
        private string _username;
        private string _password;
        private string _currentStatus = "Olete sisse loginud kui Külaline";

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
            get => _currentStatus;
            set => SetProperty(ref _currentStatus, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand LogoutCommand { get; }

        public ProfileViewModel(DatabaseService databaseService, SessionService sessionService)
        {
            _databaseService = databaseService;
            _sessionService = sessionService;

            LoginCommand = new Command(async () => await LoginAsync());
            RegisterCommand = new Command(async () => await RegisterAsync());
            LogoutCommand = new Command(Logout);
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Viga", "Palun täitke kaikki väljad", "OK");
                return;
            }

            var user = await _databaseService.LoginAsync(Username, Password);
            if (user != null)
            {
                _sessionService.CurrentUser = user;
                CurrentStatus = $"Kasutaja: {user.Username} ({user.Role})";
                Username = string.Empty;
                Password = string.Empty;
            }
            else
            {
                await Shell.Current.DisplayAlert("Viga", "Vale kasutajanimi või parool", "OK");
            }
        }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Viga", "Palun täitke kõik väljad", "OK");
                return;
            }

            var success = await _databaseService.RegisterUserAsync(Username, Password);
            if (success)
            {
                await Shell.Current.DisplayAlert("Edu", "Kasutaja on edukalt registreeritud", "OK");
                await LoginAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Viga", "See kasutajanimi on juba võetud", "OK");
            }
        }

        private void Logout()
        {
            _sessionService.CurrentUser = null;
            CurrentStatus = "Olete sisse loginud kui Külaline";
        }
    }
}