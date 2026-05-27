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

    // Runtime questions (não altera o ScriptableObject original)
    private Question[] m_runtimeQuestions;

    private Question m_currentQuestion;

    private string m_currentHint;

    private int m_questionIndex;
    private int m_hintIndex;
    private int m_currentScore;

    private int m_hintPenaltyScore = -6;
    private int m_failedPenaltyScore = -7;
    private int m_successScore = 20;

    private void CreateRuntimeQuestions()
    {
        m_runtimeQuestions = m_questionsDatabase.questions
            .OrderBy(q => Random.value)
            .Take(m_numberOfQuizQuestions)
            .ToArray();
    }

    public void InitializeGame()
    {
        CreateRuntimeQuestions();

        m_questionIndex = 0;
        m_hintIndex = 0;
        m_currentScore = 0;

        if (m_runtimeQuestions.Length > 0)
        {
            m_currentQuestion = m_runtimeQuestions[m_questionIndex];

            if (m_currentQuestion.GetHints().Length > 0)
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

        if (m_questionIndex < m_runtimeQuestions.Length)
        {
            m_currentQuestion = m_runtimeQuestions[m_questionIndex];

            m_hintIndex = 0;

            if (m_currentQuestion.GetHints().Length > 0)
                m_currentHint = m_currentQuestion.GetHints()[m_hintIndex];
        }
        else
        {
            OnAllQuestionFinished?.Invoke(m_currentScore);

            m_questionIndex = m_runtimeQuestions.Length - 1;
        }
    }

    public Question GetCurrentQuestion()
    {
        return m_currentQuestion;
    }

    public Question[] GetQuestions()
    {
        return m_runtimeQuestions;
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