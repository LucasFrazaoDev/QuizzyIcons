using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void AllQuestionsAnswered(int score);
    public event AllQuestionsAnswered OnAllQuestionFinished;

    public delegate void VisualFeedbackScore(int scoreFeedback, bool changeScoreFeedback);
    public event VisualFeedbackScore OnVisualFeedbackScore;

    private int m_numberOfQuizQuestions = 10;
    [SerializeField] private QuestionsDatabase m_questionsDatabase;
    [SerializeField] private List<Question> m_quizQuestions = new List<Question>();

    private Question m_currentQuestion;

    private string m_currentHint;
    private int m_questionIndex = 0;
    private int m_hintIndex = 0;
    private int m_currentScore = 0;

    private int m_hintPenaltyScore = -4;
    private int m_questionPenaltyScore = -5;
    private int m_successScore = +15;


    public List<Question> Questions { get => m_quizQuestions; set => m_quizQuestions = value; }

    private void OnEnable()
    {
        SelectRandomQuestions();
    }

    private void SelectRandomQuestions()
    {
        List<Question> availableQuestions = new List<Question>(m_questionsDatabase.questions);

        for (int i = 0; i < m_numberOfQuizQuestions && availableQuestions.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableQuestions.Count);
            Question selectedQuestion = availableQuestions[randomIndex];
            m_quizQuestions.Add(selectedQuestion);

            // Remove the selected question to avoid repetition
            availableQuestions.RemoveAt(randomIndex);
        }
    }

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
        OnVisualFeedbackScore?.Invoke(m_successScore, true);
        NextQuestion();
    }

    public void HandleWrongtAnswer()
    {
        SetCurrentScore(m_questionPenaltyScore);
        OnVisualFeedbackScore?.Invoke(m_questionPenaltyScore, false);
        NextQuestion();
    }

    public void ShowNextHint()
    {
        if (m_hintIndex < 2)
        {
            SetCurrentScore(m_hintPenaltyScore);
            m_currentHint = m_currentQuestion.GetHints()[++m_hintIndex];
            OnVisualFeedbackScore?.Invoke(m_hintPenaltyScore, false);
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
            OnAllQuestionFinished?.Invoke(m_currentScore);
            // Display correct question number when game is finished
            m_questionIndex = 9;
        }
    }


    public Question GetCurrentQuestion() { return m_currentQuestion; }

    public int GetCurrentQuestionNum() { return m_questionIndex + 1; }

    public string GetCurrentHint() { return m_currentHint; }

    public int GetCurrentHintNum() { return m_hintIndex + 1; }

    public int GetCurrentScore() { return m_currentScore; }

    private void SetCurrentScore(int score) { m_currentScore += score; }
}
