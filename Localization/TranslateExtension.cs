using Arvestus_project_TARpv24.Services;

namespace Arvestus_project_TARpv24.Localization
{
    // Usage in XAML: Text="{loc:Translate MenuTab}"
    // Binds to the LocalizationService indexer so the text refreshes when the language changes.
    [ContentProperty(nameof(Key))]
    public class TranslateExtension : IMarkupExtension<BindingBase>
    {
        public string Key { get; set; } = string.Empty;

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{Key}]",
                Source = LocalizationService.Instance
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
            => ProvideValue(serviceProvider);
    }
}
