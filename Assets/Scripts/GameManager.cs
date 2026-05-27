using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void AllQuestionsAnswered(int score);
    public event AllQuestionsAnswered OnAllQuestionFinished;

    public delegate void VisualFeedbackScore(int scoreFeedback, bool changeScoreFeedback);
    public event VisualFeedbackScore OnVisualFeedbackScore;

    [SerializeField] private int m_numberOfQuizQuestions = 10;
    [SerializeField] private QuestionsDatabase m_questionsDatabase;
    [SerializeField] private List<Question> m_quizQuestions = new List<Question>();

    private Question m_currentQuestion;

    private string m_currentHint;
    private int m_questionIndex = 0;
    private int m_hintIndex = 0;
    private int m_currentScore = 0;

    private int m_hintPenaltyScore = -6;
    private int m_failedPenaltyScore = -7;
    private int m_successScore = 20;


    public List<Question> Questions { get => m_quizQuestions; set => m_quizQuestions = value; }

    private void OnEnable()
    {
        SelectRandomQuestions();
    }

    private void SelectRandomQuestions()
    {
        List<Question> shuffledQuestions = new List<Question>(m_questionsDatabase.questions);

        // Embaralha a lista
        shuffledQuestions = shuffledQuestions
            .OrderBy(q => Random.value)
            .ToList();

        // Pega apenas a quantidade necessária
        for (int i = 0; i < m_numberOfQuizQuestions && i < shuffledQuestions.Count; i++)
        {
            m_quizQuestions.Add(shuffledQuestions[i]);
        }
    }

    public void InitializeGame()
    {
        // Embaralha e seleciona as perguntas do quiz
        m_quizQuestions.Clear();
        SelectRandomQuestions();

        m_questionIndex = 0;
        m_hintIndex = 0;

        if (m_quizQuestions.Count > 0)
        {
            m_currentQuestion = m_quizQuestions[m_questionIndex];
            m_currentHint = m_currentQuestion.GetHints()[m_hintIndex];
        }
    }

    public bool IsAnswerCorrect(string answer)
    {
        return m_currentQuestion.answer == answer;
    }

    public void HandleCorrectAnswer()
    {
        SetCurrentScore(m_successScore);
        OnVisualFeedbackScore?.Invoke(m_successScore, true);

        NextQuestion();
    }

    public void HandleWrongtAnswer()
    {
        SetCurrentScore(m_failedPenaltyScore);
        OnVisualFeedbackScore?.Invoke(m_failedPenaltyScore, false);

        NextQuestion();
    }

    public void ShowNextHint()
    {
        // Verifica se ainda existem dicas disponíveis
        if (m_hintIndex < m_currentQuestion.GetHints().Length - 1)
        {
            SetCurrentScore(m_hintPenaltyScore);

            m_hintIndex++;
            m_currentHint = m_currentQuestion.GetHints()[m_hintIndex];

            OnVisualFeedbackScore?.Invoke(m_hintPenaltyScore, false);
        }
        else
        {
            NextQuestion();
        }
    }

    public void NextQuestion()
    {
        m_questionIndex++;

        // Verifica se ainda existem perguntas
        if (m_questionIndex < m_quizQuestions.Count)
        {
            m_currentQuestion = m_quizQuestions[m_questionIndex];

            // Reseta as dicas da nova pergunta
            m_hintIndex = 0;
            m_currentHint = m_currentQuestion.GetHints()[m_hintIndex];
        }
        else
        {
            // Quiz finalizado
            OnAllQuestionFinished?.Invoke(m_currentScore);

            // Evita índice inválido
            m_questionIndex = m_quizQuestions.Count - 1;
        }
    }

    public Question GetCurrentQuestion()
    {
        return m_currentQuestion;
    }

    public int GetCurrentQuestionNum()
    {
        return m_questionIndex + 1;
    }

    public string GetCurrentHint()
    {
        return m_currentHint;
    }

    public int GetCurrentHintNum()
    {
        return m_hintIndex + 1;
    }

    public int GetCurrentScore()
    {
        return m_currentScore;
    }

    private void SetCurrentScore(int score)
    {
        m_currentScore += score;
    }
}
