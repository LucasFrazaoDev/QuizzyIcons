using System;
using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("References for others classes")]
    [SerializeField] private UIManager m_uiManager;
    [SerializeField] private Controller m_controller;

    [Header("Musics Audio clips")]
    [SerializeField] private AudioClip m_backgroundMusicClip;
    [SerializeField] private AudioClip m_finalMusicClip;

    [Header("SFX Audio clips")]
    [SerializeField] private AudioClip m_correctAnswerClip;
    [SerializeField] private AudioClip m_incorrectAnswerClip;

    private AudioSource m_musicSource;
    private AudioSource m_sfxSource;

    private void Awake()
    {
        m_musicSource = gameObject.AddComponent<AudioSource>();
        m_sfxSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        m_controller.OnQuestionAnswered += PlayAnswerSound;
        m_uiManager.OnMusicSliderChanged += ChangeMusicVolume;
        m_uiManager.OnSFXSliderChanged += ChangeSfxVolume;
        m_controller.OnPlayFinalMusic += ChangeBackgroundMusic;
    }

    private void Start()
    {
        m_musicSource.clip = m_backgroundMusicClip;
        m_musicSource.loop = true;
        m_musicSource.Play();
    }

    private void OnDisable()
    {
        m_controller.OnQuestionAnswered -= PlayAnswerSound;
        m_uiManager.OnMusicSliderChanged -= ChangeMusicVolume;
        m_uiManager.OnSFXSliderChanged -= ChangeSfxVolume;
        m_controller.OnPlayFinalMusic -= ChangeBackgroundMusic;
    }

    private void ChangeMusicVolume(float volume)
    {
        m_musicSource.volume = volume;
    }

    private void ChangeSfxVolume(float volume)
    {
        m_sfxSource.volume = volume;
    }

    private void PlayAnswerSound(bool wasCorrect)
    {
        AudioClip clipToPlay = wasCorrect ? m_correctAnswerClip : m_incorrectAnswerClip;
        m_sfxSource.PlayOneShot(clipToPlay);
    }

    private void ChangeBackgroundMusic()
    {
        StartCoroutine(FadeMusic());
    }

    private IEnumerator FadeMusic()
    {
        // Total fade music duration = 0.2f
        // Show panel transition = 0.25f

        float duration = 0.1f;
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / duration;
            m_musicSource.volume = Mathf.Lerp(1, 0, t);
            
            yield return null;
        }

        m_musicSource.Stop();
        m_musicSource.clip = m_finalMusicClip;
        m_musicSource.Play();

        currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / duration;
            m_musicSource.volume = Mathf.Lerp(0, 0.7f, t);

            yield return null;
        }

        m_musicSource.loop = false;
    }

}
