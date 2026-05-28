using System.Collections;
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

    [SerializeField]
    private GameManager m_gameManager;

    private int m_currentCounter = 30;

    private Coroutine m_counterCoroutine;

    private bool m_isChangingQuestion;
    private bool m_isPaused;

    public int CurrentCounter
    {
        get => m_currentCounter;

        set
        {
            m_currentCounter = value;

            if (m_currentCounter <= 0)
            {
                m_currentCounter = 0;

                m_uiManager.SetTimer("0");

                HandleWrongAnswer();

                return;
            }

            m_uiManager.SetTimer(m_currentCounter.ToString());
        }
    }

    private void Awake()
    {
        m_uiManager = GetComponent<UIManager>();

        Initialize();
    }

    private void OnEnable()
    {
        ButtonsSignature();

        m_gameManager.OnAllQuestionFinished += AllQuestionsFinished;
        m_gameManager.OnVisualFeedbackScore += ScoredPointsToShow;
    }

    private void Start()
    {
        CallLoadHighScore();
        LoadVolumeSettings();

        StartCounter();
    }

    private void OnDisable()
    {
        CancelButtonsSignature();

        m_gameManager.OnAllQuestionFinished -= AllQuestionsFinished;
        m_gameManager.OnVisualFeedbackScore -= ScoredPointsToShow;

        StopCounter();
    }

    private void AllQuestionsFinished(int score)
    {
        StopCounter();

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
        (float musicVolume, float sfxVolume) =
            OnLoadVolume?.Invoke() ?? (0.5f, 0.5f);

        m_uiManager.SetInitialVolume(ref musicVolume, ref sfxVolume);
    }

    public void Initialize()
    {
        m_gameManager.InitializeGame();

        UpdateUI(true);

        ResetCounter();
    }

    public void NextHint()
    {
        m_gameManager.ShowNextHint();

        UpdateUI();

        OnQuestionAnswered?.Invoke(false);
    }

    public void HandleWrongAnswer()
    {
        if (m_isChangingQuestion)
            return;

        StartCoroutine(HandleWrongAnswerRoutine());
    }

    private IEnumerator HandleWrongAnswerRoutine()
    {
        m_isChangingQuestion = true;

        m_gameManager.HandleWrongtAnswer();

        UpdateUI();

        ResetCounter();

        OnQuestionAnswered?.Invoke(false);

        yield return null;

        m_isChangingQuestion = false;
    }

    public void HandleCorrectAnswer()
    {
        if (m_isChangingQuestion)
            return;

        StartCoroutine(HandleCorrectAnswerRoutine());
    }

    private IEnumerator HandleCorrectAnswerRoutine()
    {
        m_isChangingQuestion = true;

        m_gameManager.HandleCorrectAnswer();

        UpdateUI();

        ResetCounter();

        OnQuestionAnswered?.Invoke(true);

        yield return null;

        m_isChangingQuestion = false;
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

        if (!isStartingGame)
            m_uiManager.ToogleOnOffButtons();
    }

    public Question[] GetQuestions()
    {
        return m_gameManager.GetQuestions();
    }

    private void ResetCounter()
    {
        CurrentCounter = 30;
    }

    private IEnumerator UpdateCounter()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);

            if (!m_isPaused)
                CurrentCounter--;
        }
    }

    private void StartCounter()
    {
        if (m_counterCoroutine != null)
            return;

        m_counterCoroutine = StartCoroutine(UpdateCounter());
    }

    private void StopCounter()
    {
        if (m_counterCoroutine == null)
            return;

        StopCoroutine(m_counterCoroutine);

        m_counterCoroutine = null;
    }

    public void IsGamePaused(bool isPaused)
    {
        m_isPaused = isPaused;
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

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}