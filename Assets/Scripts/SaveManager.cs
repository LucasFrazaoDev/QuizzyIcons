using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string M_HIGHSCORE = "HighScore";
    private const string K_MUSIC_VOLUME = "MusicVolume";
    private const string K_SFX_VOLUME = "SfxVolume";

    public void SaveHighScore(int score)
    {
        if (LoadHighScore() > score) return;

        PlayerPrefs.SetInt(M_HIGHSCORE, score);
        PlayerPrefs.Save();
    }

    public int LoadHighScore()
    {
        // Verify if exists a previous highscore
        return PlayerPrefs.HasKey(M_HIGHSCORE) ? PlayerPrefs.GetInt(M_HIGHSCORE) : 0;
    }

    public void SaveVolumesPreferences(float musicVolume, float sfxVolume)
    {
        PlayerPrefs.SetFloat(K_MUSIC_VOLUME, musicVolume);
        PlayerPrefs.SetFloat(K_SFX_VOLUME, sfxVolume);
        PlayerPrefs.Save();
    }

    public (float, float) LoadVolumePreferences()
    {
        float musicVolumeSaved;
        float sfxVolumeSaved;

        musicVolumeSaved = PlayerPrefs.HasKey(K_MUSIC_VOLUME) ? PlayerPrefs.GetFloat(K_MUSIC_VOLUME) : 0.5f;
        sfxVolumeSaved = PlayerPrefs.HasKey(K_SFX_VOLUME) ? PlayerPrefs.GetFloat(K_SFX_VOLUME) : 0.7f;

        return (musicVolumeSaved, sfxVolumeSaved);
    }
}


