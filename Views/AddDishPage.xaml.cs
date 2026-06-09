using Arvestus_project_TARpv24.ViewModels;

namespace Arvestus_project_TARpv24.Views
{
    public partial class AddDishPage : ContentPage
    {
        public AddDishPage(AddDishViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}