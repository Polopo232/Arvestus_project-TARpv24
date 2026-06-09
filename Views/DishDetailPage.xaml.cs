using Arvestus_project_TARpv24.Models;
using Arvestus_project_TARpv24.Services;
using Arvestus_project_TARpv24.ViewModels;

namespace Arvestus_project_TARpv24.Views
{
    public partial class DishDetailPage : ContentPage
    {
        public DishDetailPage(Dish dish, DatabaseService databaseService, SessionService sessionService)
        {
            InitializeComponent();
            BindingContext = new DishDetailViewModel(dish, databaseService, sessionService);
        }
    }
}