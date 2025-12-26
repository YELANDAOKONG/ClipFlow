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

public partial class FileListViewModel : ViewModelBase
{
    private readonly IClipboardService _clipboardService;
    private readonly ConfigureService _configService;
    private readonly LocalizationService _loc;

    [ObservableProperty]
    private string _fileListText = string.Empty;

    [ObservableProperty]
    private string _statusMessage;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isSuccess;

    public ApplicationConfigure Config => _configService.Configure;

    public FileListViewModel(
        IClipboardService clipboardService,
        ConfigureService configService,
        LocalizationService localizationService)
    {
        _clipboardService = clipboardService;
        _configService = configService;
        _loc = localizationService;
        _statusMessage = _loc["status.drag_list"];
    }

    [RelayCommand]
    private async Task HandleDropAsync(IReadOnlyList<IStorageItem> items)
    {
        if (items is null || items.Count == 0) return;

        IsBusy = true;
        try
        {
            var fileItems = items.OfType<IStorageFile>().ToList();
            foreach (var file in fileItems)
            {
                var path = file.Path.LocalPath;
                if (Path.GetExtension(path).Equals(".fl", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await file.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    FileListText = await reader.ReadToEndAsync();
                    StatusMessage = _loc["status.ready"];
                }
                else
                {
                    var separator = string.IsNullOrEmpty(FileListText) ? "" : Environment.NewLine;
                    FileListText += separator + path;
                }
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ProcessFileListAsync()
    {
        if (string.IsNullOrWhiteSpace(FileListText))
        {
            StatusMessage = _loc["status.list_empty"];
            return;
        }

        IsBusy = true;
        IsSuccess = false;
        int filesCopied = 0;

        try
        {
            var lines = FileListText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            var markdownBlocks = new List<string>();

            foreach (var line in lines)
            {
                var path = CleanPath(line);
                if (string.IsNullOrWhiteSpace(path) || path.StartsWith('#') || !File.Exists(path)) 
                    continue;

                try
                {
                    var content = await File.ReadAllTextAsync(path);
                    markdownBlocks.Add(FormatAsMarkdown(content, path));
                    filesCopied++;
                }
                catch (Exception ex)
                {
                    StatusMessage = string.Format(_loc["status.error"], $"{Path.GetFileName(path)}: {ex.Message}");
                }
            }

            if (markdownBlocks.Count > 0)
            {
                await CopyToClipboardAsync(string.Join("\n", markdownBlocks));
                StatusMessage = string.Format(_loc["status.success"], filesCopied);
                ShowSuccessIndicator();
            }
            else
            {
                StatusMessage = _loc["status.no_files"];
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ClearList()
    {
        FileListText = string.Empty;
        StatusMessage = _loc["status.cleared"];
    }

    private string CleanPath(string path)
    {
        path = path.Trim();
        if (path.StartsWith("file:/"))
        {
            try
            {
                return Uri.UnescapeDataString(new Uri(path).LocalPath);
            }
            catch
            {
                return Uri.UnescapeDataString(path.Replace("file:/", "").Replace("file:///", ""));
            }
        }
        return path;
    }

    // Shared logic duplicate - in a real app, move to a helper service or base class
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

    private async void ShowSuccessIndicator()
    {
        IsSuccess = true;
        await Task.Delay(2000);
        IsSuccess = false;
    }
}
