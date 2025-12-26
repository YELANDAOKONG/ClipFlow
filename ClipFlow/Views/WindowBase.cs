using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ClipFlow.Models;
using ClipFlow.Services;

namespace ClipFlow.Views;

public class WindowBase : Window
{
    protected readonly ThemeService? ThemeService;

    public WindowBase()
    {
        if (Design.IsDesignMode) 
            return;

        ThemeService = App.GetService<ThemeService>();
        
        ApplyWindowEffects();
        
        Opened += OnWindowOpened;
        Closed += OnWindowClosed;
    }

    private void OnWindowOpened(object? sender, EventArgs e)
    {
        if (ThemeService == null) return;
        
        ThemeService.PropertyChanged += OnThemeServicePropertyChanged;
        ApplyWindowEffects();
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        if (ThemeService != null)
        {
            ThemeService.PropertyChanged -= OnThemeServicePropertyChanged;
        }
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
        if (ThemeService == null) return;

        Opacity = ThemeService.WindowOpacity;
        
        var type = ThemeService.BackdropType;

        if (type == WindowBackdropType.None)
        {
            TransparencyLevelHint = [WindowTransparencyLevel.None];
            // Background = null;
            // Background = Brushes.Transparent;
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
