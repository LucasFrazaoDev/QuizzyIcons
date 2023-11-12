using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string K_HIGHSCORE = "HighScore";
    private const string K_MUSIC_VOLUME = "MusicVolume";
    private const string K_SFX_VOLUME = "SfxVolume";

    private float m_defaultMusicVolume = 0.5f;
    private float m_defaultSfxVolume = 0.7f;

    public void SaveHighScore(int score)
    {
        if (LoadHighScore() > score) return;

        PlayerPrefs.SetInt(K_HIGHSCORE, score);
        PlayerPrefs.Save();
    }

    public int LoadHighScore()
    {
        // Verify if exists a previous highscore
        return PlayerPrefs.HasKey(K_HIGHSCORE) ? PlayerPrefs.GetInt(K_HIGHSCORE) : 0;
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

        musicVolumeSaved = PlayerPrefs.HasKey(K_MUSIC_VOLUME) ? PlayerPrefs.GetFloat(K_MUSIC_VOLUME) : m_defaultMusicVolume;
        sfxVolumeSaved = PlayerPrefs.HasKey(K_SFX_VOLUME) ? PlayerPrefs.GetFloat(K_SFX_VOLUME) : m_defaultSfxVolume;

        return (musicVolumeSaved, sfxVolumeSaved);
    }
}


