using System;
using System.Collections;
using System.Collections.Generic;
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

    private Label m_hintLabel;
    private Label m_hintNumberLabel;
    private Label m_questionNumLabel;
    private Label m_timeLabel;
    private Label m_answerIndicator;
    private Label m_currentScoreLabel;
    private Label m_highScoreLabel;
    private Label m_pauseLabel;

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
    private const string M_SETTINGS_PANEL_NAME = "SettingsPanel";
    private const string M_PAUSE_PANEL_NAME = "PausePanel";
    private const string M_DROPBOX = "DropBox";

    private const string M_HINT_LABEL_NAME = "HintLabel";
    private const string M_HINT_NUMBER_LABEL_NAME = "HintNumberLabel";
    private const string M_QUESTION_NUMBER_LABEL_NAME = "QuestionNumberLabel";
    private const string M_TIME_LABEL_NAME = "TimerLabel";
    private const string M_ANSWER_INDICATOR_NAME = "AnswerIndicatorLabel";
    private const string M_CURRENT_SCORE_LABEL_NAME = "CurrentScoreLabel";
    private const string M_HIGHSCORE_LABEL_NAME = "HighScoreLabel";
    private const string M_PAUSE_LABEL_NAME = "PauseLabel";

    private const string M_OPEN_PAUSE_PANEL_BUTTON_NAME = "PauseButton";
    private const string M_CLOSE_PAUSE_PANEL_BUTTON_NAME = "ClosePausePanelButton";
    private const string M_OPEN_SETTINGS_PANEL_BUTTON_NAME = "OpenSettingsPanelButton";
    private const string M_CLOSE_SETTINGS_PANEL_BUTTON_NAME = "CloseSettingsPanelButton";
    private const string M_NEXT_HINT_BUTTON_NAME = "NextHintButton";
    private const string M_NEXT_QUESTION_BUTTON_NAME = "NextQuestionButton";
    private const string M_RESTART_GAME_BUTTON_NAME = "RestartGameButton";
    private const string M_QUIT_GAME_BUTTON_NAME = "QuitGameButton";

    private const string M_MUSIC_VOLUME_SLIDER_NAME = "MusicVolumeSlider";
    private const string M_SFX_VOLUME_SLIDER_NAME = "SFXVolumeSlider";

    private const string M_GAME_FINISHED_TEXT = "GAME FINISHED!";

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
        m_settingsPanel = m_root.Q(M_SETTINGS_PANEL_NAME);
        m_pausePanel = m_root.Q(M_PAUSE_PANEL_NAME);
        m_dropBox = m_root.Q<VisualElement>(M_DROPBOX);
    }

    private void GetLabelsReference()
    {
        m_hintLabel = m_root.Q<Label>(M_HINT_LABEL_NAME);
        m_hintNumberLabel = m_root.Q<Label>(M_HINT_NUMBER_LABEL_NAME);
        m_questionNumLabel = m_root.Q<Label>(M_QUESTION_NUMBER_LABEL_NAME);
        m_timeLabel = m_root.Q<Label>(M_TIME_LABEL_NAME);
        m_answerIndicator = m_root.Q<Label>(M_ANSWER_INDICATOR_NAME);
        m_currentScoreLabel = m_root.Q<Label>(M_CURRENT_SCORE_LABEL_NAME);
        m_highScoreLabel = m_root.Q<Label>(M_HIGHSCORE_LABEL_NAME);
        m_pauseLabel = m_root.Q<Label>(M_PAUSE_LABEL_NAME);
    }

    private void GetButtonsReference()
    {
        m_openPausePanelButton = m_root.Q<Button>(M_OPEN_PAUSE_PANEL_BUTTON_NAME);
        m_closePausePanelButton = m_root.Q<Button>(M_CLOSE_PAUSE_PANEL_BUTTON_NAME);

        m_openSettingsPanelButton = m_root.Q<Button>(M_OPEN_SETTINGS_PANEL_BUTTON_NAME);
        m_closeSettingsPanelButton = m_root.Q<Button>(M_CLOSE_SETTINGS_PANEL_BUTTON_NAME);

        m_restartGameButton = m_root.Q<Button>(M_RESTART_GAME_BUTTON_NAME);
        m_quitGameButton = m_root.Q<Button>(M_QUIT_GAME_BUTTON_NAME);

        m_nextHintButton = m_root.Q<Button>(M_NEXT_HINT_BUTTON_NAME);
        m_nextQuestionButton = m_root.Q<Button>(M_NEXT_QUESTION_BUTTON_NAME);
    }

    private void GetSlidersReference()
    {
        m_musicVolumeSlider = m_root.Q<Slider>(M_MUSIC_VOLUME_SLIDER_NAME);
        m_sfxVolumeSlider = m_root.Q<Slider>(M_SFX_VOLUME_SLIDER_NAME);
    }
    #endregion

    #region ButtonsMethods
    public void InitializeButtons()
    {
        /*
            TODO
            Criar um m�todo especifico pro bot�o, assim deixando um tempo fixo de 20 segundos 
            para cada quest�o
         */
        m_nextHintButton.clicked += m_controller.NextHint;
        m_nextQuestionButton.clicked += m_controller.HandleWrongAnswer;

        SetupIcons.InitializeDragDrop(m_root, m_controller);
        SetupIcons.InitializeIcons(m_root, m_controller.GetAllQuestions());

        m_openPausePanelButton.clicked += TogglePausePanel;
        m_closePausePanelButton.clicked += TogglePausePanel;

        m_openSettingsPanelButton.clicked += ToggleSettingsPanel;
        m_closeSettingsPanelButton.clicked += ToggleSettingsPanel;

        m_restartGameButton.clicked += HandleRestartGameButton;
        m_quitGameButton.clicked += HandleQuitGameButton;
    }

    private void TogglePausePanel()
    {
        if (m_pausePanel.style.display == DisplayStyle.Flex)
        {
            m_pausePanel.style.display = DisplayStyle.None;
            m_pausePanel.parent.style.display = DisplayStyle.None;
            m_controller.IsGamePaused(false);
        }
        else
        {
            m_pausePanel.parent.style.display = DisplayStyle.Flex;
            m_pausePanel.style.display = DisplayStyle.Flex;
            m_controller.IsGamePaused(true);
        }
    }

    private void ToggleSettingsPanel()
    {
        if (m_settingsPanel.style.display == DisplayStyle.Flex)
        {
            m_settingsPanel.style.display = DisplayStyle.None;
            m_settingsPanel.parent.style.display = DisplayStyle.None;
        }
        else
        {
            m_settingsPanel.parent.style.display = DisplayStyle.Flex;
            m_settingsPanel.style.display = DisplayStyle.Flex;
        }
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
        yield return new WaitForSeconds(2f);
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

    public void AllQuestionsAnswered()
    {
        // Reusing the pause panel to show finish game
        m_pausePanel.parent.style.display = DisplayStyle.Flex;
        m_pausePanel.style.display = DisplayStyle.Flex;
        m_controller.IsGamePaused(true);

        m_closePausePanelButton.style.display = DisplayStyle.None;
        m_pauseLabel.text = M_GAME_FINISHED_TEXT;
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
