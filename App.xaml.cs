using Microsoft.Extensions.DependencyInjection;
using Arvestus_project_TARpv24.Services;

namespace Arvestus_project_TARpv24
{
    public partial class App : Application
    {
        // Resolving LocalizationService here instantiates the singleton (and sets its static
        // Instance) before any XAML loads, so {loc:Translate} bindings have a source.
        public App(LocalizationService localization)
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}