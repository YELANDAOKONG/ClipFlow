using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using ClipFlow.Interfaces;
using ClipFlow.Localization;
using ClipFlow.Services;
using ClipFlow.ViewModels;
using ClipFlow.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ClipFlow;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
        
        var configService = GetService<ConfigureService>();
        configService.Load(); 
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = GetService<MainWindowViewModel>(),
            };
            
            desktop.Exit += async (sender, args) => 
            {
                await configService.SaveAsync();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Core Services
        services.AddSingleton(sp => new ConfigureService(null));
        services.AddSingleton<LocalizationService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<IClipboardService, ClipboardService>();
    
        // ViewModels
        services.AddTransient<DirectCopyViewModel>();
        services.AddTransient<FileListViewModel>();
        services.AddTransient<MainWindowViewModel>(); 
    }

    public static T GetService<T>() where T : class
    {
        var result = ServiceProvider?.GetService<T>();
        if (result == null)
        {
            throw new InvalidOperationException($"Service of type {typeof(T)} could not be found.");
        }
        return result;
    }
}