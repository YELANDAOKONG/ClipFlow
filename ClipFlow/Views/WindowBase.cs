using System;
using Avalonia.Controls;
using Avalonia.Media;
using ClipFlow.Models;
using ClipFlow.Services;

namespace ClipFlow.Views;

public class WindowBase : Window
{
    protected readonly ThemeService ThemeService;

    public WindowBase(){
        ThemeService = App.GetService<ThemeService>();
        
        Opened += OnWindowOpened;
        Closed += OnWindowClosed;
    }

    private void OnWindowOpened(object? sender, EventArgs e)
    {
        ThemeService.PropertyChanged += OnThemeServicePropertyChanged;
        ApplyWindowEffects();
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        ThemeService.PropertyChanged -= OnThemeServicePropertyChanged;
    }

    private void OnThemeServicePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ThemeService.BackdropType) ||
            e.PropertyName == nameof(ThemeService.WindowOpacity))
        {
            ApplyWindowEffects();
        }
    }

    private void ApplyWindowEffects()
    {
        Opacity = ThemeService.WindowOpacity;
        var type = ThemeService.BackdropType;

        if (type == WindowBackdropType.None)
        {
            TransparencyLevelHint = [WindowTransparencyLevel.None];
            Background = null; 
        }
        else
        {
            Background = Brushes.Transparent;
            
            TransparencyLevelHint = type switch
            {
                WindowBackdropType.Transparent => [WindowTransparencyLevel.Transparent],
                WindowBackdropType.Blur => [WindowTransparencyLevel.Blur],
                WindowBackdropType.Acrylic => [WindowTransparencyLevel.AcrylicBlur],
                WindowBackdropType.Mica => [WindowTransparencyLevel.Mica],
                _ => [WindowTransparencyLevel.None]
            };
        }
    }
}
