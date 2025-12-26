using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipFlow.Models;

public partial class ApplicationData : ObservableObject
{
    [ObservableProperty]
    private DateTime _lastRunTime = DateTime.UtcNow;
    
    [ObservableProperty]
    private ObservableCollection<string> _recentProjects = new();
}