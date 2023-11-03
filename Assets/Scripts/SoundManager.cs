using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private UIManager m_uiManager;

    [SerializeField] private AudioClip m_backgroundMusicClip;
    [SerializeField] private AudioClip m_correctAnswerClip;
    [SerializeField] private AudioClip m_incorrectAnswerClip;

    private AudioSource m_musicSource;
    private AudioSource m_sfxSource;

    private void Awake()
    {
        m_musicSource = gameObject.AddComponent<AudioSource>();
        m_sfxSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        m_musicSource.clip = m_backgroundMusicClip;
        m_musicSource.loop = true;
        m_musicSource.Play();

        Controller.OnQuestionAnswered += PlayAnswerSound;
        m_uiManager.OnMusicSliderChanged += ChangeMusicVolume;
        m_uiManager.OnSFXSliderChanged += ChangeSFXVolume;
    }

    private void ChangeMusicVolume(float volume)
    {
        m_musicSource.volume = volume;
    }

    private void ChangeSFXVolume(float volume)
    {
        m_sfxSource.volume = volume;
    }

    private void PlayAnswerSound(bool wasCorrect)
    {
        AudioClip clipToPlay = wasCorrect ? m_correctAnswerClip : m_incorrectAnswerClip;
        m_sfxSource.PlayOneShot(clipToPlay);
    }

    private void OnDestroy()
    {
        Controller.OnQuestionAnswered -= PlayAnswerSound;
        m_uiManager.OnMusicSliderChanged -= ChangeMusicVolume;
        m_uiManager.OnSFXSliderChanged -= ChangeSFXVolume;
    }
}