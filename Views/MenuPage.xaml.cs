using Arvestus_project_TARpv24.ViewModels;

namespace Arvestus_project_TARpv24.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage(MenuViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}