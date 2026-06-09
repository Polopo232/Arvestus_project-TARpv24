using Arvestus_project_TARpv24.ViewModels;

namespace Arvestus_project_TARpv24.Views
{
    public partial class DailyMenuPage : ContentPage
    {
        private readonly DailyMenuViewModel _viewModel;

        public DailyMenuPage(DailyMenuViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null)
            {
                await _viewModel.LoadDailyMenuAsync();
                await _viewModel.LoadAllDishesAsync();
            }
        }
    }
}