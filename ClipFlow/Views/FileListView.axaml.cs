using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ClipFlow.ViewModels;
using System.Linq;

namespace ClipFlow.Views;

public partial class FileListView : UserControl
{
    public FileListView()
    {
        InitializeComponent();
        AddHandler(DragDrop.DragOverEvent, DragOver, RoutingStrategies.Tunnel);
        AddHandler(DragDrop.DropEvent, Drop, RoutingStrategies.Tunnel);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        var hasFiles = e.DataTransfer.TryGetFiles()?.Any() ?? false;
        if (hasFiles)
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private async void Drop(object? sender, DragEventArgs e)
    {
        if (DataContext is FileListViewModel vm)
        {
            var files = e.DataTransfer.TryGetFiles();
            if (files != null && files.Any())
            {
                await vm.HandleDropCommand.ExecuteAsync(files.ToList());
            }
        }
        e.Handled = true;
    }
}