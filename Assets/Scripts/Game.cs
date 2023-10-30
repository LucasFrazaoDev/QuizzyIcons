using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private List<Question> m_questions = new List<Question>();

    private Question m_currentQuestion;
    private int m_questionIndex = 0;
    private string m_currentHint;
    private int m_hintIndex = 0;

    public void InitializeGame()
    {
        m_currentQuestion = m_questions[m_questionIndex];
        m_currentHint = m_currentQuestion.GetHints()[m_hintIndex];
    }

    public bool IsAnswerCorrect(string answer)
    {
        return m_currentQuestion.answer == answer;
    }

    public void HandleCorrectAnswer()
    {
        NextQuestion();
    }

    public void HandleWrongtAnswer()
    {
        if (m_hintIndex < 2)
        {
            m_currentHint = m_currentQuestion.GetHints()[++m_hintIndex];
        }
        else
        {
            NextQuestion();
        }
    }

    public void NextQuestion()
    {
        m_currentQuestion = m_questions[++m_questionIndex];

        m_hintIndex = 0;
        m_currentHint = m_currentQuestion.GetHints()[m_hintIndex];
    }

    public Question GetCurrentQuestion() { return m_currentQuestion; }

    public int GetCurrentQuestionNum() { return m_questionIndex + 1; }

    public string GetCurrentHint() { return m_currentHint; }

    public int GetCurrentHintNum() { return m_hintIndex + 1; }
}
