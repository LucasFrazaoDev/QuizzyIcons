using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UILinker : MonoBehaviour
{
    private Controller m_controller;
    private VisualElement m_root;

    private Label m_hintLabel;
    private Label m_hintNumberLabel;
    private Label m_questionNumLabel;
    private Label m_timeLabel;
    private Label m_answerIndicator;
    private Label m_highscoreLabel;
    private Label m_currentScoreLabel;
    private Button m_nextHintButton;

    private void Awake()
    {
        m_root = GetComponent<UIDocument>().rootVisualElement;
        m_controller = GetComponent<Controller>();
    }

    private void OnEnable()
    {
        VisualElement icon = m_root.Q("TestIcon");
        m_hintLabel = m_root.Q<Label>("HintLabel");
        m_hintNumberLabel = m_root.Q<Label>("HintNumberLabel");
        m_questionNumLabel = m_root.Q<Label>("QuestionNumberLabel");
        m_timeLabel = m_root.Q<Label>("TimerLabel");
        m_answerIndicator = m_root.Q<Label>("AnswerIndicatorLabel");
        m_highscoreLabel = m_root.Q<Label>("HighscoreLabel");
        m_currentScoreLabel = m_root.Q<Label>("CurrentScoreLabel");
        m_nextHintButton = m_root.Q<Button>("NextHintButton");
    }

    private void Start()
    {
        InitializeButton();
    }

    public void InitializeButton()
    {
        m_nextHintButton.clicked += m_controller.HandleWrongAnswer;
        SetupIcons.InitializeDragDrop(m_root, m_controller);
        SetupIcons.InitializeIcons(m_root, m_controller.GetAllQuestions());
    }

    public void GiveAnswerFeedback(bool correct)
    {
        m_answerIndicator.style.visibility = Visibility.Visible;
        m_answerIndicator.text = correct ? "Your answer was correct!" : "Your answer was wrong!";

        StyleColor colorCorrect = new StyleColor(new Color32(0, 132, 19, 255));
        StyleColor colorWrong = new StyleColor(new Color32(132, 0, 19, 255));
        m_answerIndicator.style.color = correct ? colorCorrect : colorWrong;

        StartCoroutine(CleanUpQuestion());
    }

    IEnumerator CleanUpQuestion()
    {
        yield return new WaitForSeconds(2f);
        m_answerIndicator.style.visibility = Visibility.Hidden;

        VisualElement dropZone = m_root.Q<VisualElement>("DropBox");
        if (dropZone.childCount > 0)
            dropZone.RemoveAt(0);
    }

    public void SetTimer(string seconds)
    {
        m_timeLabel.text = "Time remaining: " + seconds + " seconds";
    }

    public void SetHint(string hintText)
    {
        m_hintLabel.text = hintText;
    }

    public void SetHintNumber(int hintNumber)
    {
        m_hintNumberLabel.text = "Hint " + hintNumber.ToString() + ": ";
    }

    public void SetQuestionNumber(int questionNum)
    {
        m_questionNumLabel.text = "Question " + questionNum.ToString();
    }
}
