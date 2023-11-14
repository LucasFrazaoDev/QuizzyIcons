using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public delegate void AnswerAction(bool wasCorrect);
    public event AnswerAction OnQuestionAnswered;

    public delegate void PlayFinalMusic();
    public event PlayFinalMusic OnPlayFinalMusic;

    public delegate void SaveScore(int score);
    public event SaveScore OnSaveScore;

    public delegate int LoadHighScore();
    public event LoadHighScore OnLoadHighScore;

    public delegate void SaveVolume(float musicVolume, float sfxVolume);
    public event SaveVolume OnSaveVolume;

    public delegate (float, float) LoadVolume();
    public event LoadVolume OnLoadVolume;

    private UIManager m_uiManager;
    [SerializeField] private GameManager m_gameManager;

    private int m_currentCounter = 25;
    private bool m_counterStopped;

    public int CurrentCounter
    {
        get { return m_currentCounter; }
        set
        {
            if (m_counterStopped) return;

            if (value == 0)
            {
                HandleWrongAnswer();
                return;
            }
            m_uiManager.SetTimer(value.ToString());
            m_currentCounter = value;
        }
    }

    private void Awake()
    {
        m_uiManager = GetComponent<UIManager>();
    }

    private void OnEnable()
    {
        ButtonsSignature();

        m_gameManager.OnAllQuestionFinished += AllQuestionsFinished;
        m_gameManager.OnVisualFeedbackScore += ScoredPointsToShow;
    }

    private void Start()
    {
        Initialize();
        IsGamePaused(false);
        CallLoadHighScore();
        LoadVolumeSettings();
    }

    private void OnDisable()
    {
        CancelButtonsSignature();

        m_gameManager.OnAllQuestionFinished -= AllQuestionsFinished;
        m_gameManager.OnVisualFeedbackScore -= ScoredPointsToShow;
    }

    private void AllQuestionsFinished(int score)
    {
        m_uiManager.AllQuestionsAnsweredFeedBack(score);
        OnPlayFinalMusic?.Invoke();
        OnSaveScore?.Invoke(score);
    }

    private void ScoredPointsToShow(int scoreToShow, bool changeScoreFeedback)
    {
        m_uiManager.ShowPointsScored(scoreToShow, changeScoreFeedback);
    }

    private void CallLoadHighScore()
    {
        int highScore = OnLoadHighScore?.Invoke() ?? 0;
        m_uiManager.SetHighScore(highScore);
    }

    public void SaveVolumeSettings(float musicVolume, float sfxVolume)
    {
        OnSaveVolume?.Invoke(musicVolume, sfxVolume);
    }

    public void LoadVolumeSettings()
    {
        (float musicVolume, float sfxVolume) = OnLoadVolume?.Invoke() ?? (0.5f, 0.5f);
        m_uiManager.SetInitialVolume(ref musicVolume, ref sfxVolume);
    }

    public void Initialize()
    {
        m_gameManager.InitializeGame();
        UpdateUI(true);

        ResetCounter();
        StartCoroutine(UpdateCounter());
    }

    public void NextHint()
    {
        m_gameManager.ShowNextHint();
        UpdateUI();
        OnQuestionAnswered?.Invoke(false);
    }

    public void HandleWrongAnswer()
    {
        m_gameManager.HandleWrongtAnswer();
        UpdateUI();
        ResetCounter();
        OnQuestionAnswered?.Invoke(false);
    }

    public void HandleCorrectAnswer()
    {
        m_gameManager.HandleCorrectAnswer();
        UpdateUI();
        ResetCounter();
        OnQuestionAnswered?.Invoke(true);
    }

    public void CheckAnswer(string answer)
    {
        bool answerCorrect = m_gameManager.IsAnswerCorrect(answer);
    
        if (answerCorrect)
            HandleCorrectAnswer();
        else
            HandleWrongAnswer();

        m_uiManager.GiveAnswerFeedback(answerCorrect);
    }

    public void UpdateUI(bool isStartingGame = false)
    {
        m_uiManager.SetHint(m_gameManager.GetCurrentHint());
        m_uiManager.SetHintNumber(m_gameManager.GetCurrentHintNum());
        m_uiManager.SetQuestionNumber(m_gameManager.GetCurrentQuestionNum());
        m_uiManager.SetCurrentScore(m_gameManager.GetCurrentScore());

        if (!isStartingGame) m_uiManager.ToogleOnOffButtons();
    }

    public List<Question> GetAllQuestions()
    {
        return m_gameManager.Questions;
    }

    private void ResetCounter()
    {
        CurrentCounter = 30;
    }

    private IEnumerator UpdateCounter()
    {
        yield return new WaitForSeconds(1f);

        CurrentCounter--;
        StartCoroutine(UpdateCounter());
    }

    private void ButtonsSignature()
    {
        m_uiManager.OnRestartGameButtonClicked += RestartGame;
        m_uiManager.OnQuitGameButtonClicked += QuitGame;
    }

    private void CancelButtonsSignature()
    {
        m_uiManager.OnRestartGameButtonClicked -= RestartGame;
        m_uiManager.OnQuitGameButtonClicked -= QuitGame;
    }

    public void IsGamePaused(bool isPaused)
    {
        m_counterStopped = isPaused;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
