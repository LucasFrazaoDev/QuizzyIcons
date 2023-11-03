using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    private Game m_game;
    private UIManager m_uiManager;
    private int m_currentCounter;

    public delegate void AnswerAction(bool wasCorrect);
    public static event AnswerAction OnQuestionAnswered;

    public int CurrentCounter
    {
        get { return m_currentCounter; }
        set
        {
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
        m_game = GetComponent<Game>();
        m_uiManager = GetComponent<UIManager>();
    }

    private void Start()
    {
        Initialize();
    }

    private void OnDisable()
    {
        CancelButtonsSignature();
    }

    public void Initialize()
    {
        m_game.InitializeGame();
        UpdateUI();
        ButtonsSignature();

        ResetCounter();
        StartCoroutine(UpdateCounter());
    }

    public void HandleWrongAnswer()
    {
        m_game.HandleWrongtAnswer();
        UpdateUI();
        ResetCounter();
        OnQuestionAnswered?.Invoke(false);
    }

    public void HandleCorrectAnswer()
    {
        m_game.HandleCorrectAnswer();
        UpdateUI();
        ResetCounter();
        OnQuestionAnswered?.Invoke(true);
    }

    public void CheckAnswer(string answer)
    {
        bool answerCorrect = m_game.IsAnswerCorrect(answer);
    
        if (answerCorrect)
            HandleCorrectAnswer();
        else
            HandleWrongAnswer();

        m_uiManager.GiveAnswerFeedback(answerCorrect);
    }

    public void UpdateUI()
    {
        m_uiManager.SetHint(m_game.GetCurrentHint());
        m_uiManager.SetHintNumber(m_game.GetCurrentHintNum());
        m_uiManager.SetQuestionNumber(m_game.GetCurrentQuestionNum());
    }

    public List<Question> GetAllQuestions()
    {
        return m_game.Questions;
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
    }

    private void CancelButtonsSignature()
    {
        m_uiManager.OnRestartGameButtonClicked -= RestartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
