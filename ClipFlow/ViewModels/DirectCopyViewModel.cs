using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ClipFlow.Interfaces;
using ClipFlow.Localization;
using ClipFlow.Models;
using ClipFlow.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClipFlow.ViewModels;

public partial class DirectCopyViewModel : ViewModelBase
{
    private readonly IClipboardService _clipboardService;
    private readonly ConfigureService _configService;
    private readonly LocalizationService _loc;

    [ObservableProperty]
    private string _statusMessage;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isSuccess;

    public ApplicationConfigure Config => _configService.Configure;
    public ApplicationData Data => _configService.Data;

    public DirectCopyViewModel(
        IClipboardService clipboardService, 
        ConfigureService configService,
        LocalizationService localizationService)
    {
        _clipboardService = clipboardService;
        _configService = configService;
        _loc = localizationService;
        _statusMessage = _loc["status.drag_drop"];
    }

    [RelayCommand]
    private async Task HandleDropAsync(IReadOnlyList<IStorageItem> items)
    {
        if (items is null || items.Count == 0) return;

        IsBusy = true;
        IsSuccess = false;
        int filesCopied = 0;

        try
        {
            var fileItems = items.OfType<IStorageFile>().ToList();
            if (fileItems.Count == 0)
            {
                StatusMessage = _loc["status.no_files"];
                return;
            }

            var markdownBlocks = new List<string>();
            var processedPaths = new HashSet<string>();

            foreach (var file in fileItems)
            {
                var filePath = file.Path.LocalPath;
                if (!processedPaths.Add(filePath)) continue;

                try
                {
                    using var stream = await file.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var content = await reader.ReadToEndAsync();
                    
                    if (string.IsNullOrEmpty(content)) continue;

                    markdownBlocks.Add(FormatAsMarkdown(content, filePath));
                    AddToRecentFiles(filePath);
                    filesCopied++;
                }
                catch (Exception ex)
                {
                    StatusMessage = string.Format(_loc["status.error"], ex.Message);
                }
            }

            if (markdownBlocks.Count > 0)
            {
                await CopyToClipboardAsync(string.Join("\n", markdownBlocks));
                StatusMessage = string.Format(_loc["status.success"], filesCopied);
                ShowSuccessIndicator();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CopyRecentFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            StatusMessage = string.Format(_loc["status.error"], "File not found");
            return;
        }

        IsBusy = true;
        try
        {
            var content = await File.ReadAllTextAsync(filePath);
            await CopyToClipboardAsync(FormatAsMarkdown(content, filePath));
            StatusMessage = string.Format(_loc["status.success"], 1);
            ShowSuccessIndicator();
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format(_loc["status.error"], ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ClearRecentFiles()
    {
        Data.RecentProjects.Clear();
        StatusMessage = _loc["status.cleared"];
    }

    private string FormatAsMarkdown(string content, string filePath)
    {
        var identifier = Config.IncludeFilePaths ? filePath : Path.GetExtension(filePath);
        var backticks = new string('`', Math.Max(3, Config.BacktickCount));
        return $"{backticks}{identifier}\n{content.TrimEnd()}\n{backticks}\n";
    }

    private async Task CopyToClipboardAsync(string text)
    {
        if (Config.AppendToClipboard)
        {
            var current = await _clipboardService.GetTextAsync();
            if (!string.IsNullOrEmpty(current))
            {
                text = current + "\n\n" + text;
            }
        }
        await _clipboardService.SetTextAsync(text);
    }

    private void AddToRecentFiles(string filePath)
    {
        var list = Data.RecentProjects;
        if (list.Contains(filePath)) list.Remove(filePath);
        list.Insert(0, filePath);
        while (list.Count > 10) list.RemoveAt(list.Count - 1);
    }

    private async void ShowSuccessIndicator()
    {
        IsSuccess = true;
        await Task.Delay(2000);
        IsSuccess = false;
    }
}
