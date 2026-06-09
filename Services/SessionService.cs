using System.ComponentModel;
using System.Runtime.CompilerServices;
using Arvestus_project_TARpv24.Models;

namespace Arvestus_project_TARpv24.Services
{
    public class SessionService : INotifyPropertyChanged
    {
        private User _currentUser;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAdmin));
                OnPropertyChanged(nameof(IsLoggedIn));
                OnPropertyChanged(nameof(IsLoggedOut));
            }
        }

        public bool IsAdmin => CurrentUser?.Role == "Admin";
        public bool IsLoggedIn => CurrentUser != null;
        public bool IsLoggedOut => CurrentUser == null;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}