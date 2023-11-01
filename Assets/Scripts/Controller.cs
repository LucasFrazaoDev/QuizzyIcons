using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Game m_game;
    private UILinker m_uiLinker;
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
            m_uiLinker.SetTimer(value.ToString());
            m_currentCounter = value;
        }
    }

    private void Awake()
    {
        m_game = GetComponent<Game>();
        m_uiLinker = GetComponent<UILinker>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        m_game.InitializeGame();
        UpdateUI();

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

        m_uiLinker.GiveAnswerFeedback(answerCorrect);
    }

    public void UpdateUI()
    {
        m_uiLinker.SetHint(m_game.GetCurrentHint());
        m_uiLinker.SetHintNumber(m_game.GetCurrentHintNum());
        m_uiLinker.SetQuestionNumber(m_game.GetCurrentQuestionNum());
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
}
