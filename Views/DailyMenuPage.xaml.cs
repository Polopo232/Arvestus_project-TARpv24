using Arvestus_project_TARpv24.ViewModels;

namespace Arvestus_project_TARpv24.Views
{
    public partial class DailyMenuPage : ContentPage
    {
        public DailyMenuPage(DailyMenuViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}