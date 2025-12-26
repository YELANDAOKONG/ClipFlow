using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipFlow.Models;

public partial class ApplicationConfigure : ObservableObject
{
    [ObservableProperty]
    private string _language = "en-US";

    [ObservableProperty]
    private ApplicationTheme _theme = ApplicationTheme.System;

    [ObservableProperty]
    private WindowBackdropType _backdropType = WindowBackdropType.Mica;

    [ObservableProperty]
    private double _windowOpacity = 0.95;

    // Copy functionality
    [ObservableProperty]
    private bool _includeFilePaths = true;

    [ObservableProperty]
    private bool _appendToClipboard = false;

    [ObservableProperty]
    private int _backtickCount = 3;
}