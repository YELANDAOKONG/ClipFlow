using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using ClipFlow.ViewModels;
using System.Collections.Generic;
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
        e.DragEffects = DragDropEffects.Copy;
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