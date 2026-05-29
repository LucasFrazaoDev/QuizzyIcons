using System;

public enum Language { English, Portuguese }

public static class LocalizationManager
{
    public static Language CurrentLanguage { get; private set; } = Language.English;
    public static event Action OnLanguageChanged;

    public static void SetLanguage(Language language)
    {
        if (CurrentLanguage == language) return;

        CurrentLanguage = language;
        OnLanguageChanged?.Invoke();
    }

    public static string Get(string keyEN, string keyPT)
    {
        return CurrentLanguage == Language.English ? keyEN : keyPT;
    }
}