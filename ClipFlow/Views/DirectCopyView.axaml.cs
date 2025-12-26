using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using ClipFlow.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var items = await GetStorageItems(e);
            if (items.Any())
            {
                await vm.HandleDropCommand.ExecuteAsync(items);
            }
        }
        e.Handled = true;
    }

    private async Task<IEnumerable<IStorageItem>> GetStorageItems(DragEventArgs e)
    {
        var files = e.Data.GetFiles();
        if (files != null) return files;
        return [];
    }
}