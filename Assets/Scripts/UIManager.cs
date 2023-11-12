using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    // Sliders events
    public delegate void SliderChangeHandler(float value);
    public event SliderChangeHandler OnMusicSliderChanged;
    public event SliderChangeHandler OnSFXSliderChanged;

    // Buttons events
    public delegate void RestartGameButtonHandler();
    public event RestartGameButtonHandler OnRestartGameButtonClicked;

    public delegate void QuitGameButtonHandler();
    public event QuitGameButtonHandler OnQuitGameButtonClicked;

    private Controller m_controller;

    // UI elements (Visual elements)
    private VisualElement m_root;
    private VisualElement m_settingsPanel;
    private VisualElement m_pausePanel;
    private VisualElement m_dropBox;
    private VisualElement m_panelsContainer;

    private Label m_hintLabel;
    private Label m_hintNumberLabel;
    private Label m_questionNumLabel;
    private Label m_timeLabel;
    private Label m_answerIndicator;
    private Label m_currentScoreLabel;
    private Label m_highScoreLabel;
    private Label m_pauseLabel;
    private Label m_scoreFeedbackLabel;

    private Button m_openPausePanelButton;
    private Button m_closePausePanelButton;
    private Button m_openSettingsPanelButton;
    private Button m_closeSettingsPanelButton;
    private Button m_restartGameButton;
    private Button m_nextHintButton;
    private Button m_nextQuestionButton;
    private Button m_quitGameButton;

    private Slider m_musicVolumeSlider;
    private Slider m_sfxVolumeSlider;

    // UI elements names (string)
    private const string K_SETTINGS_PANEL_NAME = "SettingsPanel";
    private const string K_PAUSE_PANEL_NAME = "PausePanel";
    private const string K_PANELS_CONTAINER_NAME = "PanelsContainer";
    private const string K_DROPBOX = "DropBox";

    private const string K_HINT_LABEL_NAME = "HintLabel";
    private const string K_HINT_NUMBER_LABEL_NAME = "HintNumberLabel";
    private const string K_QUESTION_NUMBER_LABEL_NAME = "QuestionNumberLabel";
    private const string K_TIME_LABEL_NAME = "TimerLabel";
    private const string K_ANSWER_INDICATOR_NAME = "AnswerIndicatorLabel";
    private const string K_CURRENT_SCORE_LABEL_NAME = "CurrentScoreLabel";
    private const string K_HIGHSCORE_LABEL_NAME = "HighScoreLabel";
    private const string K_PAUSE_LABEL_NAME = "PauseLabel";
    private const string K_SCORE_FEEDBACK_LABEL_NAME = "ScoreFeedbackLabel";

    private const string K_OPEN_PAUSE_PANEL_BUTTON_NAME = "PauseButton";
    private const string K_CLOSE_PAUSE_PANEL_BUTTON_NAME = "ClosePausePanelButton";
    private const string K_OPEN_SETTINGS_PANEL_BUTTON_NAME = "OpenSettingsPanelButton";
    private const string K_CLOSE_SETTINGS_PANEL_BUTTON_NAME = "CloseSettingsPanelButton";
    private const string K_NEXT_HINT_BUTTON_NAME = "NextHintButton";
    private const string K_NEXT_QUESTION_BUTTON_NAME = "NextQuestionButton";
    private const string K_RESTART_GAME_BUTTON_NAME = "RestartGameButton";
    private const string K_QUIT_GAME_BUTTON_NAME = "QuitGameButton";

    private const string K_MUSIC_VOLUME_SLIDER_NAME = "MusicVolumeSlider";
    private const string K_SFX_VOLUME_SLIDER_NAME = "SFXVolumeSlider";

    private const string K_GAME_FINISHED_TEXT = "GAME FINISHED!";
    private const string K_CLASS_TO_SHOW_PANEL_NAME = "ShowPanelTransition";
    private const string K_CLASS_TO_SCORE_FEEDBACK_NAME = "RiseUpScoreFeedback";

    private void Awake()
    {
        m_root = GetComponent<UIDocument>().rootVisualElement;
        m_controller = GetComponent<Controller>();
    }

    private void OnEnable()
    {
        GetVisualElementsReference();
        GetLabelsReference();
        GetButtonsReference();
        GetSlidersReference();
    }

    private void Start()
    {
        InitializeButtons();
        InitializeSliders();
        HideAnswerIndicator();
    }

    private void OnDisable()
    {
        m_nextHintButton.clicked -= m_controller.NextHint;

        m_openPausePanelButton.clicked -= TogglePausePanel;
        m_closePausePanelButton.clicked -= TogglePausePanel;

        m_openSettingsPanelButton.clicked -= ToggleSettingsPanel;
        m_closeSettingsPanelButton.clicked -= ToggleSettingsPanel;

        m_restartGameButton.clicked -= HandleRestartGameButton;

        m_musicVolumeSlider.UnregisterValueChangedCallback(MusicSliderCallback);
        m_sfxVolumeSlider.UnregisterValueChangedCallback(SfxSliderCallback);
    }

    #region GetReferences
    private void GetVisualElementsReference()
    {
        m_settingsPanel = m_root.Q(K_SETTINGS_PANEL_NAME);
        m_pausePanel = m_root.Q(K_PAUSE_PANEL_NAME);
        m_dropBox = m_root.Q<VisualElement>(K_DROPBOX);
        m_panelsContainer = m_root.Q<VisualElement>(K_PANELS_CONTAINER_NAME);
    }

    private void GetLabelsReference()
    {
        m_hintLabel = m_root.Q<Label>(K_HINT_LABEL_NAME);
        m_hintNumberLabel = m_root.Q<Label>(K_HINT_NUMBER_LABEL_NAME);
        m_questionNumLabel = m_root.Q<Label>(K_QUESTION_NUMBER_LABEL_NAME);
        m_timeLabel = m_root.Q<Label>(K_TIME_LABEL_NAME);
        m_answerIndicator = m_root.Q<Label>(K_ANSWER_INDICATOR_NAME);
        m_currentScoreLabel = m_root.Q<Label>(K_CURRENT_SCORE_LABEL_NAME);
        m_highScoreLabel = m_root.Q<Label>(K_HIGHSCORE_LABEL_NAME);
        m_pauseLabel = m_root.Q<Label>(K_PAUSE_LABEL_NAME);
        m_scoreFeedbackLabel = m_root.Q<Label>(K_SCORE_FEEDBACK_LABEL_NAME);
    }

    private void GetButtonsReference()
    {
        m_openPausePanelButton = m_root.Q<Button>(K_OPEN_PAUSE_PANEL_BUTTON_NAME);
        m_closePausePanelButton = m_root.Q<Button>(K_CLOSE_PAUSE_PANEL_BUTTON_NAME);

        m_openSettingsPanelButton = m_root.Q<Button>(K_OPEN_SETTINGS_PANEL_BUTTON_NAME);
        m_closeSettingsPanelButton = m_root.Q<Button>(K_CLOSE_SETTINGS_PANEL_BUTTON_NAME);

        m_restartGameButton = m_root.Q<Button>(K_RESTART_GAME_BUTTON_NAME);
        m_quitGameButton = m_root.Q<Button>(K_QUIT_GAME_BUTTON_NAME);

        m_nextHintButton = m_root.Q<Button>(K_NEXT_HINT_BUTTON_NAME);
        m_nextQuestionButton = m_root.Q<Button>(K_NEXT_QUESTION_BUTTON_NAME);
    }

    private void GetSlidersReference()
    {
        m_musicVolumeSlider = m_root.Q<Slider>(K_MUSIC_VOLUME_SLIDER_NAME);
        m_sfxVolumeSlider = m_root.Q<Slider>(K_SFX_VOLUME_SLIDER_NAME);
    }

    public void SetInitialVolume(ref float musicVolume, ref float sfxVolume)
    {
        m_musicVolumeSlider.value = musicVolume;
        m_sfxVolumeSlider.value = sfxVolume;
    }
    #endregion

    #region ButtonsMethods
    public void InitializeButtons()
    {
        SetupIcons.InitializeDragDrop(m_root, m_controller);
        SetupIcons.InitializeIcons(m_root, m_controller.GetAllQuestions());

        m_nextHintButton.clicked += m_controller.NextHint;
        m_nextQuestionButton.clicked += m_controller.HandleWrongAnswer;

        m_openPausePanelButton.clicked += TogglePausePanel;
        m_closePausePanelButton.clicked += TogglePausePanel;

        m_openSettingsPanelButton.clicked += ToggleSettingsPanel;
        m_closeSettingsPanelButton.clicked += ToggleSettingsPanel;

        m_restartGameButton.clicked += HandleRestartGameButton;
        m_quitGameButton.clicked += HandleQuitGameButton;
    }

    private void TogglePausePanel()
    {
        if (m_panelsContainer.style.display == DisplayStyle.Flex)
        {
            StartCoroutine(HidePanel(m_pausePanel));
            ChangeGameState(false);
        }
        else
        {
            // the use of Invoke() to delay the call to ShowSettingsPanel its to ensure that the m_panelsContainer
            // has fully transitioned to DisplayStyle.Flex before starting the animation and avoid unexpected behaviors.
            m_panelsContainer.style.display = DisplayStyle.Flex;
            Invoke(nameof(ShowPausePanel), 0.05f);
            ChangeGameState(true);
        }
    }

    private void ToggleSettingsPanel()
    {
        if (m_panelsContainer.style.display == DisplayStyle.Flex)
        {
            StartCoroutine(HidePanel(m_settingsPanel));
            ChangeGameState(false);
            m_controller.SaveVolumeSettings(m_musicVolumeSlider.value, m_sfxVolumeSlider.value);
        }
        else
        {
            // Same situation above
            m_panelsContainer.style.display = DisplayStyle.Flex;
            Invoke(nameof(ShowSettingsPanel), 0.05f);
            ChangeGameState(true);
        }
    }

    private void ShowSettingsPanel()
    {
        m_settingsPanel.AddToClassList(K_CLASS_TO_SHOW_PANEL_NAME);
    }
    
    private void ShowPausePanel()
    {
        m_pausePanel.AddToClassList(K_CLASS_TO_SHOW_PANEL_NAME);
    }

    private void ChangeGameState(bool isPaused)
    {
        m_controller.IsGamePaused(isPaused);
    }

    private IEnumerator HidePanel(VisualElement panel)
    {
        panel.RemoveFromClassList(K_CLASS_TO_SHOW_PANEL_NAME);
        yield return new WaitForSeconds(0.25f);
        panel.parent.style.display = DisplayStyle.None;
    }

    private void HandleRestartGameButton()
    {
        OnRestartGameButtonClicked?.Invoke();
    }

    private void HandleQuitGameButton()
    {
        OnQuitGameButtonClicked?.Invoke();
    }
    #endregion

    #region SlidersMethods
    private void InitializeSliders()
    {
        m_musicVolumeSlider.RegisterValueChangedCallback(MusicSliderCallback);
        m_sfxVolumeSlider.RegisterValueChangedCallback(SfxSliderCallback);
    }

    private void MusicSliderCallback(ChangeEvent<float> e)
    {
        OnMusicSliderChanged?.Invoke(e.newValue);
    }

    private void SfxSliderCallback(ChangeEvent<float> e)
    {
        OnSFXSliderChanged?.Invoke(e.newValue);
    }
    #endregion

    #region LabelsMethods
    private void HideAnswerIndicator() => m_answerIndicator.style.visibility = Visibility.Hidden;

    public void ShowPointsScored(int scoreToShow)
    {
        /*
        TODO
            1 - Fix the bug when active a transition after a element is display.Flex
            2 - Add the color variation according to wrong/right answer
         */
        Debug.Log(scoreToShow);
        m_scoreFeedbackLabel.text = scoreToShow.ToString();
        m_scoreFeedbackLabel.style.display = DisplayStyle.Flex;


        float duration = 0.3f;
        float currentTime = 0f;

        while (currentTime <= duration)
            currentTime += Time.deltaTime;

        m_scoreFeedbackLabel.AddToClassList(K_CLASS_TO_SCORE_FEEDBACK_NAME);

        //m_scoreFeedbackLabel.style.display = DisplayStyle.None;
        //m_scoreFeedbackLabel.RemoveFromClassList(K_CLASS_TO_SCORE_FEEDBACK_NAME);
    }
    #endregion

    public void GiveAnswerFeedback(bool correct)
    {
        m_answerIndicator.style.visibility = Visibility.Visible;
        m_answerIndicator.text = correct ? "Your answer was correct!" : "Your answer was wrong!";

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

    public void ToogleOnOffButtons()
    {
        StartCoroutine(DisableEnableButtons());
    }

    private IEnumerator DisableEnableButtons()
    {
        m_nextHintButton.SetEnabled(false);
        m_nextQuestionButton.SetEnabled(false);
        yield return new WaitForSeconds(1.5f);
        m_nextHintButton.SetEnabled(true);
        m_nextQuestionButton.SetEnabled(true);
    }

    public void AllQuestionsAnsweredFeedBack()
    {
        // Reusing the pause panel to show finish game
        m_panelsContainer.style.display = DisplayStyle.Flex;
        Invoke(nameof(ShowPausePanel), 0.05f);
        ChangeGameState(true);

        m_closePausePanelButton.style.display = DisplayStyle.None;
        m_pauseLabel.text = K_GAME_FINISHED_TEXT;
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

    public void SetCurrentScore(int currentScore)
    {
        m_currentScoreLabel.text = "Score: " + currentScore.ToString();
    }

    public void SetHighScore(int highScore)
    {
        m_highScoreLabel.text = "Highscore: " + highScore.ToString();
    }
}
