using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private Controller m_controller;

    private const string K_HIGHSCORE = "HighScore";
    private const string K_MUSIC_VOLUME = "MusicVolume";
    private const string K_SFX_VOLUME = "SfxVolume";

    private float m_defaultMusicVolume = 0.5f;
    private float m_defaultSfxVolume = 0.7f;

    private void OnEnable()
    {
        m_controller.OnSaveScore += SaveHighScore;
        m_controller.OnLoadHighScore += LoadHighScore;
        m_controller.OnSaveVolume += SaveVolumesPreferences;
        m_controller.OnLoadVolume += LoadVolumePreferences;
    }

    private void OnDisable()
    {
        m_controller.OnSaveScore -= SaveHighScore;
        m_controller.OnLoadHighScore -= LoadHighScore;
        m_controller.OnSaveVolume -= SaveVolumesPreferences;
        m_controller.OnLoadVolume -= LoadVolumePreferences;
    }

    private void SaveHighScore(int score)
    {
        if (LoadHighScore() > score) return;

        PlayerPrefs.SetInt(K_HIGHSCORE, score);
        PlayerPrefs.Save();
    }

    private int LoadHighScore()
    {
        // Verify if exists a previous highscore and return a value
        return PlayerPrefs.HasKey(K_HIGHSCORE) ? PlayerPrefs.GetInt(K_HIGHSCORE) : 0;
    }

    private void SaveVolumesPreferences(float musicVolume, float sfxVolume)
    {
        PlayerPrefs.SetFloat(K_MUSIC_VOLUME, musicVolume);
        PlayerPrefs.SetFloat(K_SFX_VOLUME, sfxVolume);
        PlayerPrefs.Save();
    }

    private (float, float) LoadVolumePreferences()
    {
        float musicVolumeSaved;
        float sfxVolumeSaved;

        musicVolumeSaved = PlayerPrefs.HasKey(K_MUSIC_VOLUME) ? PlayerPrefs.GetFloat(K_MUSIC_VOLUME) : m_defaultMusicVolume;
        sfxVolumeSaved = PlayerPrefs.HasKey(K_SFX_VOLUME) ? PlayerPrefs.GetFloat(K_SFX_VOLUME) : m_defaultSfxVolume;

        return (musicVolumeSaved, sfxVolumeSaved);
    }
}


