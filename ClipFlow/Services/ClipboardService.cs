using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using System.Threading.Tasks;
using ClipFlow.Interfaces;

namespace ClipFlow.Services;

public class ClipboardService : IClipboardService
{
    private IClipboard? GetClipboard()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow?.Clipboard;
        }
        return null;
    }
    
    public async Task<string?> GetTextAsync()
    {
        var clipboard = GetClipboard();
        return clipboard != null ? await clipboard.GetTextAsync() : null;
    }
    
    public async Task SetTextAsync(string text)
    {
        var clipboard = GetClipboard();
        if (clipboard != null)
        {
            await clipboard.SetTextAsync(text);
        }
    }
}