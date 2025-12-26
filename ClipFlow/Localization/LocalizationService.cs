using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia.Platform;
using ClipFlow.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipFlow.Localization;

public partial class LocalizationService : ObservableObject
{
    public const string TranslationsLocation = "avares://ClipFlow/Assets/Locales/";
    private const string DefaultLanguage = "en-US";
    
    private readonly Dictionary<string, Dictionary<string, string>> _translations = new();
    private readonly ConfigureService _configService;

    [ObservableProperty]
    private string _currentLanguage;

    public LocalizationService(ConfigureService configService)
    {
        _configService = configService;
        
        LoadTranslations();
        
        _currentLanguage = _configService.Configure.Language ?? DefaultLanguage;
        
        _configService.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ConfigureService.Configure))
            {
                SetLanguage(_configService.Configure.Language);
            }
        };
    }

    public string this[string key] => GetString(key);

    public void SetLanguage(string? languageCode)
    {
        if (languageCode == null)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(languageCode) || !_translations.ContainsKey(languageCode))
        {
            return;
        }

        CurrentLanguage = languageCode;
        _configService.Configure.Language = languageCode;
        OnPropertyChanged(string.Empty); 
    }

    public string GetString(string key)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;

        if (_translations.TryGetValue(CurrentLanguage, out var currentDict) && 
            currentDict.TryGetValue(key, out var value))
        {
            return value;
        }

        if (CurrentLanguage != DefaultLanguage && 
            _translations.TryGetValue(DefaultLanguage, out var defaultDict) && 
            defaultDict.TryGetValue(key, out var defaultValue))
        {
            return defaultValue;
        }

        return key;
    }

    private void LoadTranslations()
    {
        _translations.Clear();
        if (!_translations.ContainsKey(DefaultLanguage))
            _translations[DefaultLanguage] = new Dictionary<string, string>();

        try
        {
            var localesUri = new Uri(TranslationsLocation);
            var assets = AssetLoader.GetAssets(localesUri, null);

            foreach (var uri in assets)
            {
                var fileName = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
                if (string.IsNullOrEmpty(fileName)) continue;

                using var stream = AssetLoader.Open(uri);
                using var reader = new StreamReader(stream);
                var jsonContent = reader.ReadToEnd();

                var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                if (translations != null)
                {
                    _translations[fileName] = translations;
                }
            }
        }
        catch (Exception)
        {
            // Ignored
        }
    }
}
