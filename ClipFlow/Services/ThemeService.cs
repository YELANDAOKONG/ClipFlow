using Avalonia;
using Avalonia.Styling;
using ClipFlow.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipFlow.Services;

public partial class ThemeService : ObservableObject
{
    private readonly ConfigureService _configService;
    
    [ObservableProperty] 
    private ApplicationTheme _currentTheme;
    
    [ObservableProperty] 
    private WindowBackdropType _backdropType;
    
    [ObservableProperty] 
    private double _windowOpacity;

    public ThemeService(ConfigureService configService)
    {
        _configService = configService;

        LoadSettingsFromConfig();
        _configService.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ConfigureService.Configure))
            {
                LoadSettingsFromConfig();
            }
        };
    }

    private void LoadSettingsFromConfig()
    {
        var config = _configService.Configure;
        
        CurrentTheme = config.Theme;
        BackdropType = config.BackdropType;
        WindowOpacity = config.WindowOpacity;
        
        ApplyTheme(CurrentTheme);
    }

    partial void OnCurrentThemeChanged(ApplicationTheme value)
    {
        ApplyTheme(value);
        
        _configService.Configure.Theme = value;
    }
    
    partial void OnBackdropTypeChanged(WindowBackdropType value)
    {
        _configService.Configure.BackdropType = value;
    }

    partial void OnWindowOpacityChanged(double value)
    {
        _configService.Configure.WindowOpacity = value;
    }

    private void ApplyTheme(ApplicationTheme theme)
    {
        if (Application.Current is null) return;

        Application.Current.RequestedThemeVariant = theme switch
        {
            ApplicationTheme.Light => ThemeVariant.Light,
            ApplicationTheme.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default // System
        };
    }
}
