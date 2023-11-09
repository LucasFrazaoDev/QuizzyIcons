using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    [SerializeField] private SaveManager m_saveManager;
    [SerializeField] private GameManager m_gameManager;

    private UIManager m_uiManager;
    private int m_currentCounter;
    private bool m_counterStopped;

    public delegate void AnswerAction(bool wasCorrect);
    public static event AnswerAction OnQuestionAnswered;

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
        m_gameManager.FinishedAllQuestion += AllQuestionsFinished;
    }

    

    private void Start()
    {
        Initialize();
        IsGamePaused(false);
        LoadHighScore();
    }

    private void OnDisable()
    {
        CancelButtonsSignature();
        m_gameManager.FinishedAllQuestion -= AllQuestionsFinished;
    }

    private void AllQuestionsFinished(int score)
    {
        m_uiManager.AllQuestionsAnswered();
        m_saveManager.SaveHighScore(score);
    }

    private void LoadHighScore()
    {
        int highScore = m_saveManager.LoadHighScore();
        m_uiManager.SetHighScore(highScore);
    }

    public void Initialize()
    {
        m_gameManager.InitializeGame();
        UpdateUI();
        ButtonsSignature();

        ResetCounter();
        StartCoroutine(UpdateCounter());
    }

    public void NextHint()
    {
        m_gameManager.ShowNextHint();
        UpdateUI(true);
        OnQuestionAnswered?.Invoke(false);
    }

    public void HandleWrongAnswer()
    {
        m_gameManager.HandleWrongtAnswer();
        UpdateUI(true);
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

    public void UpdateUI(bool isButtonClicked = false)
    {
        m_uiManager.SetHint(m_gameManager.GetCurrentHint());
        m_uiManager.SetHintNumber(m_gameManager.GetCurrentHintNum());
        m_uiManager.SetQuestionNumber(m_gameManager.GetCurrentQuestionNum());
        m_uiManager.SetCurrentScore(m_gameManager.GetCurrentScore());

        if (isButtonClicked) m_uiManager.ToogleOnOffButtons();
    }

    public List<Question> GetAllQuestions()
    {
        return m_gameManager.Questions;
    }

    private void ResetCounter()
    {
        CurrentCounter = 20;
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
