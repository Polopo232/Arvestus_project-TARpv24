using Arvestus_project_TARpv24.ViewModels;

namespace Arvestus_project_TARpv24.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}