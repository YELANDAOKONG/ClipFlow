using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ClipFlow.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipFlow.Services;

public class ConfigureService : ObservableObject
{
    public static readonly string ApplicationName = "ClipFlow";

    private ApplicationConfigure _configure = new();
    private ApplicationData _data = new();

    public string ApplicationDataDirectory { get; }
    public string ApplicationConfigurePath { get; }
    public string ApplicationDataPath { get; }

    public ApplicationConfigure Configure
    {
        get => _configure;
        private set => SetProperty(ref _configure, value);
    }

    public ApplicationData Data
    {
        get => _data;
        private set => SetProperty(ref _data, value);
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = null
    };

    public ConfigureService(string? dataDirectory)
    {
        ApplicationDataDirectory = dataDirectory ?? GetRootDirectory();
        ApplicationConfigurePath = Path.Combine(ApplicationDataDirectory, "Configure.json");
        ApplicationDataPath = Path.Combine(ApplicationDataDirectory, "Data.json");
    }
    
    
    public void Load()
    {
        LoadConfigure();
        LoadData();
    }

    public void LoadConfigure()
    {
        if (!File.Exists(ApplicationConfigurePath)) return;

        try
        {
            using var stream = File.OpenRead(ApplicationConfigurePath);
            var result = JsonSerializer.Deserialize<ApplicationConfigure>(stream, _jsonOptions);
            
            if (result != null)
            {
                Configure = result;
            }
        }
        catch (JsonException)
        {
            // Handle corrupted JSON
        }
    }

    public void LoadData()
    {
        if (!File.Exists(ApplicationDataPath)) return;

        try
        {
            using var stream = File.OpenRead(ApplicationDataPath);
            var result = JsonSerializer.Deserialize<ApplicationData>(stream, _jsonOptions);

            if (result != null)
            {
                Data = result;
            }
        }
        catch (JsonException)
        {
            // Handle corrupted JSON
        }
    }

    public async Task LoadAsync()
    {
        await LoadConfigureAsync();
        await LoadDataAsync();
    }

    public async Task SaveAsync()
    {
        await SaveConfigureAsync();
        await SaveDataAsync();
    }

    public async Task LoadConfigureAsync()
    {
        if (!File.Exists(ApplicationConfigurePath)) return;

        try
        {
            using var stream = File.OpenRead(ApplicationConfigurePath);
            var result = await JsonSerializer.DeserializeAsync<ApplicationConfigure>(stream, _jsonOptions);
            
            if (result != null)
            {
                Configure = result;
            }
        }
        catch (JsonException) { }
    }

    public async Task LoadDataAsync()
    {
        if (!File.Exists(ApplicationDataPath)) return;

        try
        {
            using var stream = File.OpenRead(ApplicationDataPath);
            var result = await JsonSerializer.DeserializeAsync<ApplicationData>(stream, _jsonOptions);

            if (result != null)
            {
                Data = result;
            }
        }
        catch (JsonException) { }
    }

    public async Task SaveConfigureAsync()
    {
        EnsureDirectoryExists(ApplicationConfigurePath);
        
        using var stream = File.Create(ApplicationConfigurePath);
        await JsonSerializer.SerializeAsync(stream, Configure, _jsonOptions);
    }

    public async Task SaveDataAsync()
    {
        EnsureDirectoryExists(ApplicationDataPath);

        using var stream = File.Create(ApplicationDataPath);
        await JsonSerializer.SerializeAsync(stream, Data, _jsonOptions);
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static string GetRootDirectory()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(appDataPath, ApplicationName);
        
        if (!Directory.Exists(folder)) 
            Directory.CreateDirectory(folder);
            
        return folder;
    }

    public static string GetRootTempDirectory()
    {
        var tempFolder = Path.Combine(Path.GetTempPath(), ApplicationName);
        
        if (!Directory.Exists(tempFolder)) 
            Directory.CreateDirectory(tempFolder);
            
        return tempFolder;
    }
}
