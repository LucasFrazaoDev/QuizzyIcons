using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FeedbackManager : MonoBehaviour
{
    private VisualElement m_root;
    private VisualElement m_dropBox;

    private Label m_answerIndicator;
    private Label m_scoreFeedbackLabel;

    private const string K_ANSWER_INDICATOR_NAME = "AnswerIndicatorLabel";
    private const string K_SCORE_FEEDBACK_LABEL_NAME = "ScoreFeedbackLabel";
    private const string K_DROPBOX = "DropBox";

    private const string K_CLASS_TO_SCORE_FEEDBACK_NAME = "RiseUpScoreFeedback";

    private void OnEnable()
    {
        m_root = GetComponent<UIDocument>().rootVisualElement;

        m_answerIndicator = m_root.Q<Label>(K_ANSWER_INDICATOR_NAME);
        m_scoreFeedbackLabel = m_root.Q<Label>(K_SCORE_FEEDBACK_LABEL_NAME);
        m_dropBox = m_root.Q<VisualElement>(K_DROPBOX);
    }

    private void Start()
    {
        HideAnswerIndicator();
    }

    private void HideAnswerIndicator()
    {
        m_answerIndicator.style.visibility = Visibility.Hidden;
    }

    public void GiveAnswerFeedback(bool correct, string answerName)
    {
        m_answerIndicator.style.visibility = Visibility.Visible;
        m_answerIndicator.text = correct
            ? $"{answerName} — {LocalizationManager.Get("Correct!", "Correto!")}"
            : $"{answerName} — {LocalizationManager.Get("Wrong!", "Errado!")}";

        StyleColor colorCorrect = new StyleColor(new Color32(0, 132, 19, 255));
        StyleColor colorWrong = new StyleColor(new Color32(132, 0, 19, 255));
        m_answerIndicator.style.color = correct ? colorCorrect : colorWrong;

        StartCoroutine(CleanUpQuestion());
    }

    private IEnumerator CleanUpQuestion()
    {
        yield return new WaitForSeconds(1.5f);
        m_answerIndicator.style.visibility = Visibility.Hidden;

        if (m_dropBox.childCount > 0)
            m_dropBox.RemoveAt(0);
    }

    public void ShowPointsScored(int scoreToShow, bool changeScoreFeedback)
    {
        m_scoreFeedbackLabel.text = (scoreToShow > 0) ? $"+{scoreToShow}" : scoreToShow.ToString();

        StyleColor colorCorrect = new StyleColor(new Color32(0, 132, 19, 255));
        StyleColor colorWrong = new StyleColor(new Color32(132, 0, 19, 255));
        m_scoreFeedbackLabel.style.color = changeScoreFeedback ? colorCorrect : colorWrong;

        m_scoreFeedbackLabel.style.display = DisplayStyle.Flex;

        StartCoroutine(ShowScoreFeedback());
    }

    private IEnumerator ShowScoreFeedback()
    {
        yield return new WaitForSeconds(0.05f);

        m_scoreFeedbackLabel.AddToClassList(K_CLASS_TO_SCORE_FEEDBACK_NAME);

        yield return new WaitForSeconds(1f);

        m_scoreFeedbackLabel.RemoveFromClassList(K_CLASS_TO_SCORE_FEEDBACK_NAME);
        m_scoreFeedbackLabel.style.display = DisplayStyle.None;
    }
}