using ClipFlow.Localization;
using ClipFlow.Models;
using ClipFlow.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClipFlow.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly LocalizationService _localizationService;
    private readonly ThemeService _themeService;
    private readonly ConfigureService _configService;

    public DirectCopyViewModel DirectCopyVm { get; }
    public FileListViewModel FileListVm { get; }
    
    public LocalizationService Localization => _localizationService;
    public ApplicationConfigure Config => _configService.Configure;

    // Expose Enums for Settings UI
    public List<ApplicationTheme> Themes { get; } = Enum.GetValues<ApplicationTheme>().ToList();
    public List<WindowBackdropType> BackdropTypes { get; } = Enum.GetValues<WindowBackdropType>().ToList();
    public List<string> Languages { get; } = ["en-US", "zh-CN"];

    public MainWindowViewModel(
        IServiceProvider sp, 
        LocalizationService localizationService,
        ThemeService themeService,
        ConfigureService configService)
    {
        _localizationService = localizationService;
        _themeService = themeService;
        _configService = configService;

        DirectCopyVm = sp.GetRequiredService<DirectCopyViewModel>();
        FileListVm = sp.GetRequiredService<FileListViewModel>();
    }
}