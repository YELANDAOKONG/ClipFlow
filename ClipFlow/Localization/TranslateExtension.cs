using Avalonia.Data;
using Avalonia.Markup.Xaml;
using ClipFlow.Services;
using System;

namespace ClipFlow.Localization;

public class TranslateExtension : MarkupExtension
{
    public string Key { get; set; }

    public TranslateExtension(string key)
    {
        Key = key;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var service = App.GetService<LocalizationService>();

        return new Binding
        {
            Source = service,
            Path = $"[{Key}]",
            Mode = BindingMode.OneWay
        };
    }
}