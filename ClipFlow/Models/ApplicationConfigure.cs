using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipFlow.Models;

public partial class ApplicationConfigure : ObservableObject
{
    [ObservableProperty]
    private string _language = "en-US";

    [ObservableProperty]
    private ApplicationTheme _theme = ApplicationTheme.System;

    [ObservableProperty]
    private WindowBackdropType _backdropType = WindowBackdropType.Blur;

    [ObservableProperty]
    private double _windowOpacity = 1.0;
}