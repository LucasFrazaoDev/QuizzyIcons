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
    private TimerController m_timerController;

    [SerializeField]
    private GameManager m_gameManager;

    private bool m_isChangingQuestion;

    private void Awake()
    {
        m_uiManager = GetComponent<UIManager>();
        m_timerController = GetComponent<TimerController>();

        Initialize();
    }

    private void OnEnable()
    {
        ButtonsSignature();

        m_gameManager.OnAllQuestionFinished += AllQuestionsFinished;
        m_gameManager.OnVisualFeedbackScore += ScoredPointsToShow;

        LocalizationManager.OnLanguageChanged += OnLanguageChanged;

        m_timerController.OnTimerExpired += OnTimerExpired;
        m_timerController.OnTimerTick += OnTimerTick;
    }

    private void Start()
    {
        CallLoadHighScore();
        LoadVolumeSettings();

        m_timerController.ResetTimer();
    }

    private void OnDisable()
    {
        CancelButtonsSignature();

        m_gameManager.OnAllQuestionFinished -= AllQuestionsFinished;
        m_gameManager.OnVisualFeedbackScore -= ScoredPointsToShow;

        LocalizationManager.OnLanguageChanged -= OnLanguageChanged;

        m_timerController.OnTimerExpired -= OnTimerExpired;
        m_timerController.OnTimerTick -= OnTimerTick;

        m_timerController.StopTimer();
    }

    public void StartGame()
    {
        m_timerController.StartTimer();
    }

    private void OnTimerTick(int currentValue)
    {
        m_uiManager.SetTimer(currentValue.ToString());
    }

    private void OnLanguageChanged()
    {
        m_gameManager.RefreshCurrentHint();
        UpdateUI(true);
        m_uiManager.SetHighScore(OnLoadHighScore?.Invoke() ?? 0);
        m_uiManager.SetTimer(m_timerController.CurrentCounter.ToString());
        m_uiManager.RefreshStaticLabels();
    }

    private void AllQuestionsFinished(int score)
    {
        m_timerController.StopTimer();

        string playerName = PlayerPrefs.GetString("PlayerName", "Anonymous");
        LeaderboardManager.SaveScore(playerName, score);

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
    }

    public void NextHint()
    {
        bool isLastHint = m_gameManager.GetCurrentHintNum() >= m_gameManager.GetCurrentQuestion().GetHints().Length;

        if (isLastHint)
        {
            string localizedAnswer = m_gameManager.GetCurrentQuestion().GetAnswer();
            m_uiManager.RemoveIconByAnswer(m_gameManager.GetCurrentQuestion().answer);
            m_uiManager.GiveAnswerFeedback(false, localizedAnswer);

            m_timerController.StopTimer(); 
            m_timerController.ResetTimer();
            m_timerController.StartTimer();
        }

        m_gameManager.ShowNextHint();

        UpdateUI();

        OnQuestionAnswered?.Invoke(false);
    }

    public void HandleWrongAnswer(bool removeCurrentIcon = false)
    {
        if (m_isChangingQuestion)
            return;

        StartCoroutine(HandleWrongAnswerRoutine(removeCurrentIcon));
    }

    private IEnumerator HandleWrongAnswerRoutine(bool removeCurrentIcon = false)
    {
        m_isChangingQuestion = true;

        if (removeCurrentIcon)
        {
            string localizedAnswer = m_gameManager.GetCurrentQuestion().GetAnswer();
            m_uiManager.RemoveIconByAnswer(m_gameManager.GetCurrentQuestion().answer);
            m_uiManager.GiveAnswerFeedback(false, localizedAnswer); // <- novo
        }

        m_gameManager.HandleWrongtAnswer();

        UpdateUI();

        m_timerController.StopTimer();
        m_timerController.ResetTimer();
        m_timerController.StartTimer();

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

        m_timerController.StopTimer();
        m_timerController.ResetTimer();
        m_timerController.StartTimer();

        OnQuestionAnswered?.Invoke(true);

        yield return null;

        m_isChangingQuestion = false;
    }

    public void CheckAnswer(string answer)
    {
        bool answerCorrect = m_gameManager.IsAnswerCorrect(answer);

        string localizedAnswer = m_gameManager.GetCurrentQuestion().GetAnswer();
        string currentQuestionAnswer = m_gameManager.GetCurrentQuestion().answer;

        if (answerCorrect)
        {
            HandleCorrectAnswer();
        }
        else
        {
            // Remove o ícone da pergunta atual (Torre Eiffel no exemplo)
            m_uiManager.RemoveIconByAnswer(currentQuestionAnswer);

            // Remove a pergunta do ícone usado errado (Coliseu no exemplo)
            m_gameManager.RemoveQuestionByAnswer(answer);

            HandleWrongAnswer();
        }

        m_uiManager.GiveAnswerFeedback(answerCorrect, localizedAnswer);
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

    private void OnTimerExpired()
    {
        HandleWrongAnswer(removeCurrentIcon: true);
    }

    public Question[] GetQuestions()
    {
        return m_gameManager.GetQuestions();
    }

    public void IsGamePaused(bool isPaused)
    {
        m_timerController.SetPaused(isPaused);
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