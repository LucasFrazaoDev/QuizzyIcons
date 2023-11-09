using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void AllQuestionsAnswered(int score);
    public event AllQuestionsAnswered FinishedAllQuestion;

    [SerializeField] private List<Question> m_questions = new List<Question>();

    private Question m_currentQuestion;
    private int m_questionIndex = 0;
    private int m_hintIndex = 0;
    private string m_currentHint;

    private int m_currentScore = 0;

    private int m_lowPenaltyScore = -2;
    private int m_penaltyScore = -5;
    private int m_successScore = 15;


    public List<Question> Questions { get => m_questions; set => m_questions = value; }

    public void InitializeGame()
    {
        m_currentQuestion = Questions[m_questionIndex];
        m_currentHint = m_currentQuestion.GetHints()[m_hintIndex];
    }

    public bool IsAnswerCorrect(string answer)
    {
        return m_currentQuestion.answer == answer;
    }

    public void HandleCorrectAnswer()
    {
        SetCurrentScore(m_successScore);
        NextQuestion();
    }

    public void HandleWrongtAnswer()
    {
        SetCurrentScore(m_penaltyScore);
        NextQuestion();
    }

    public void ShowNextHint()
    {
        if (m_hintIndex < 2)
        {
            SetCurrentScore(m_lowPenaltyScore);
            m_currentHint = m_currentQuestion.GetHints()[++m_hintIndex];
        }
        else
            NextQuestion();
    }

    public void NextQuestion()
    {
        m_questionIndex++;

        // Check if index is within the bounds of the list
        if (m_questionIndex < Questions.Count)
        {
            m_currentQuestion = Questions[m_questionIndex];

            m_hintIndex = 0;
            m_currentHint = m_currentQuestion.GetHints()[m_hintIndex];
        }
        else
        {
            // Finished all the questions
            FinishedAllQuestion?.Invoke(m_currentScore);
            m_questionIndex = 9;
        }
    }


    public Question GetCurrentQuestion() { return m_currentQuestion; }

    public int GetCurrentQuestionNum() { return m_questionIndex + 1; }

    public string GetCurrentHint() { return m_currentHint; }

    public int GetCurrentHintNum() { return m_hintIndex + 1; }

    public int GetCurrentScore() { return m_currentScore; }

    private void SetCurrentScore(int score)
    {
        m_currentScore += score;
    }
}
