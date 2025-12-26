using Avalonia.Controls;
using Avalonia.Input;
using ClipFlow.ViewModels;
using System.Linq;

namespace ClipFlow.Views;

public partial class DirectCopyView : UserControl
{
    public DirectCopyView()
    {
        InitializeComponent();
        AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DropEvent, Drop);
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
        if (DataContext is DirectCopyViewModel vm)
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