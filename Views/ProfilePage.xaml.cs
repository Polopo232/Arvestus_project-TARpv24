using Arvestus_project_TARpv24.ViewModels;

namespace Arvestus_project_TARpv24.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage(ProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}