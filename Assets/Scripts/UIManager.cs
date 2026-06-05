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
    private FeedbackManager m_feedbackManager;

    private VisualElement m_root;
    private VisualElement m_settingsPanel;
    private VisualElement m_pausePanel;
    private VisualElement m_panelsContainer;

    private Label m_hintLabel;
    private Label m_hintNumberLabel;
    private Label m_questionNumLabel;
    private Label m_timeLabel;
    private Label m_currentScoreLabel;
    private Label m_highScoreLabel;
    private Label m_pauseLabel;
    private Label m_showScoreFinalPanelLabel;
    private Label m_dragYourAnswerLabel;

    private Button m_openPausePanelButton;
    private Button m_closePausePanelButton;
    private Button m_openSettingsPanelButton;
    private Button m_closeSettingsPanelButton;
    private Button m_restartGameButton;
    private Button m_nextHintButton;
    private Button m_nextQuestionButton;
    private Button m_quitGameButton;
    private Button m_englishButton;
    private Button m_portugueseButton;

    private Slider m_musicVolumeSlider;
    private Slider m_sfxVolumeSlider;

    private const string K_SETTINGS_PANEL_NAME = "SettingsPanel";
    private const string K_PAUSE_PANEL_NAME = "PausePanel";
    private const string K_PANELS_CONTAINER_NAME = "PanelsContainer";

    private const string K_HINT_LABEL_NAME = "HintLabel";
    private const string K_HINT_NUMBER_LABEL_NAME = "HintNumberLabel";
    private const string K_QUESTION_NUMBER_LABEL_NAME = "QuestionNumberLabel";
    private const string K_TIME_LABEL_NAME = "TimerLabel";
    private const string K_CURRENT_SCORE_LABEL_NAME = "CurrentScoreLabel";
    private const string K_HIGHSCORE_LABEL_NAME = "HighScoreLabel";
    private const string K_PAUSE_LABEL_NAME = "PauseLabel";
    private const string K_SHOW_FINAL_SCORE_LABEL_NAME = "ShowScoreFinalPanelLabel";
    private const string K_DRAG_YOUR_ANSWER_LABEL_NAME = "DragYourAnswerLabel";

    private const string K_OPEN_PAUSE_PANEL_BUTTON_NAME = "PauseButton";
    private const string K_CLOSE_PAUSE_PANEL_BUTTON_NAME = "ClosePausePanelButton";
    private const string K_OPEN_SETTINGS_PANEL_BUTTON_NAME = "OpenSettingsPanelButton";
    private const string K_CLOSE_SETTINGS_PANEL_BUTTON_NAME = "CloseSettingsPanelButton";
    private const string K_NEXT_HINT_BUTTON_NAME = "NextHintButton";
    private const string K_NEXT_QUESTION_BUTTON_NAME = "NextQuestionButton";
    private const string K_RESTART_GAME_BUTTON_NAME = "RestartGameButton";
    private const string K_QUIT_GAME_BUTTON_NAME = "QuitGameButton";
    private const string K_ENGLISH_BUTTON_NAME = "EnglishButton";
    private const string K_PORTUGUESE_BUTTON_NAME = "PortugueseButton";

    private const string K_MUSIC_VOLUME_SLIDER_NAME = "MusicVolumeSlider";
    private const string K_SFX_VOLUME_SLIDER_NAME = "SFXVolumeSlider";

    private const string K_GAME_FINISHED_TEXT_EN = "GAME FINISHED!";
    private const string K_GAME_FINISHED_TEXT_PT = "FIM DE JOGO!";
    private const string K_CLASS_TO_SHOW_PANEL_NAME = "ShowPanelTransition";
    private const string K_LANGUAGE_BUTTON_ACTIVE_CLASS = "language-button-active";

    private void Awake()
    {
        m_root = GetComponent<UIDocument>().rootVisualElement;
        m_controller = GetComponent<Controller>();
        m_feedbackManager = GetComponent<FeedbackManager>();
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
        InitializeIcons();
        InitializeButtons();
        InitializeSliders();
        RefreshStaticLabels();
    }

    private void OnDisable()
    {
        ClearButtonsSignature();
        ClearSlidersSignature();
    }

    private void InitializeIcons()
    {
        SetupIcons.InitializeDragDrop(m_root, m_controller);
        SetupIcons.InitializeIcons(m_root, m_controller.GetQuestions());
    }

    #region GetReferences
    private void GetVisualElementsReference()
    {
        m_settingsPanel = m_root.Q(K_SETTINGS_PANEL_NAME);
        m_pausePanel = m_root.Q(K_PAUSE_PANEL_NAME);
        m_panelsContainer = m_root.Q<VisualElement>(K_PANELS_CONTAINER_NAME);
    }

    private void GetLabelsReference()
    {
        m_hintLabel = m_root.Q<Label>(K_HINT_LABEL_NAME);
        m_hintNumberLabel = m_root.Q<Label>(K_HINT_NUMBER_LABEL_NAME);
        m_questionNumLabel = m_root.Q<Label>(K_QUESTION_NUMBER_LABEL_NAME);
        m_timeLabel = m_root.Q<Label>(K_TIME_LABEL_NAME);
        m_currentScoreLabel = m_root.Q<Label>(K_CURRENT_SCORE_LABEL_NAME);
        m_highScoreLabel = m_root.Q<Label>(K_HIGHSCORE_LABEL_NAME);
        m_pauseLabel = m_root.Q<Label>(K_PAUSE_LABEL_NAME);
        m_showScoreFinalPanelLabel = m_root.Q<Label>(K_SHOW_FINAL_SCORE_LABEL_NAME);
        m_dragYourAnswerLabel = m_root.Q<Label>(K_DRAG_YOUR_ANSWER_LABEL_NAME);
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
        m_englishButton = m_root.Q<Button>(K_ENGLISH_BUTTON_NAME);
        m_portugueseButton = m_root.Q<Button>(K_PORTUGUESE_BUTTON_NAME);
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
        m_nextHintButton.clicked += m_controller.NextHint;
        m_nextQuestionButton.clicked += HandleNextQuestionButton;


        m_openPausePanelButton.clicked += TogglePausePanel;
        m_closePausePanelButton.clicked += TogglePausePanel;

        m_openSettingsPanelButton.clicked += ToggleSettingsPanel;
        m_closeSettingsPanelButton.clicked += ToggleSettingsPanel;

        m_restartGameButton.clicked += HandleRestartGameButton;
        m_quitGameButton.clicked += HandleQuitGameButton;

        m_englishButton.clicked += () =>
        {
            LocalizationManager.SetLanguage(Language.English);
            UpdateLanguageButtons();
        };

        m_portugueseButton.clicked += () =>
        {
            LocalizationManager.SetLanguage(Language.Portuguese);
            UpdateLanguageButtons();
        };

        UpdateLanguageButtons();
    }

    private void UpdateLanguageButtons()
    {
        bool isEnglish = LocalizationManager.CurrentLanguage == Language.English;

        if (isEnglish)
        {
            m_englishButton.AddToClassList(K_LANGUAGE_BUTTON_ACTIVE_CLASS);
            m_portugueseButton.RemoveFromClassList(K_LANGUAGE_BUTTON_ACTIVE_CLASS);
        }
        else
        {
            m_portugueseButton.AddToClassList(K_LANGUAGE_BUTTON_ACTIVE_CLASS);
            m_englishButton.RemoveFromClassList(K_LANGUAGE_BUTTON_ACTIVE_CLASS);
        }
    }

    private void TogglePausePanel()
    {
        if (m_panelsContainer.style.display == DisplayStyle.Flex)
        {
            StartCoroutine(HidePanel(m_pausePanel));
        }
        else
        {
            m_panelsContainer.style.display = DisplayStyle.Flex;
            StartCoroutine(ShowPanelDelayed(m_pausePanel));
            ChangeGameState(true);
        }
    }

    private void ToggleSettingsPanel()
    {
        if (m_panelsContainer.style.display == DisplayStyle.Flex)
        {
            StartCoroutine(HidePanel(m_settingsPanel));
            m_controller.SaveVolumeSettings(m_musicVolumeSlider.value, m_sfxVolumeSlider.value);
        }
        else
        {
            m_panelsContainer.style.display = DisplayStyle.Flex;
            StartCoroutine(ShowPanelDelayed(m_settingsPanel));
            ChangeGameState(true);
        }
    }

    public void AllQuestionsAnsweredFeedBack(int finalScore)
    {
        m_panelsContainer.style.display = DisplayStyle.Flex;
        StartCoroutine(ShowPanelDelayed(m_pausePanel));
        ChangeGameState(true);

        m_closePausePanelButton.style.display = DisplayStyle.None;
        m_pauseLabel.text = LocalizationManager.Get(K_GAME_FINISHED_TEXT_EN, K_GAME_FINISHED_TEXT_PT);

        ShowScoreInFinalPanel(finalScore);
        m_showScoreFinalPanelLabel.style.display = DisplayStyle.Flex;
    }

    private IEnumerator ShowPanelDelayed(VisualElement panel)
    {
        yield return null;
        panel.AddToClassList(K_CLASS_TO_SHOW_PANEL_NAME);
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
        ChangeGameState(false);
    }

    private void HandleRestartGameButton()
    {
        OnRestartGameButtonClicked?.Invoke();
    }

    private void HandleNextQuestionButton()
    {
        m_controller.HandleWrongAnswer(true);
    }

    private void HandleQuitGameButton()
    {
        OnQuitGameButtonClicked?.Invoke();
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

    private void ClearButtonsSignature()
    {
        m_nextHintButton.clicked -= m_controller.NextHint;
        m_nextQuestionButton.clicked -= HandleNextQuestionButton;
        m_openPausePanelButton.clicked -= TogglePausePanel;
        m_closePausePanelButton.clicked -= TogglePausePanel;
        m_openSettingsPanelButton.clicked -= ToggleSettingsPanel;
        m_closeSettingsPanelButton.clicked -= ToggleSettingsPanel;
        m_restartGameButton.clicked -= HandleRestartGameButton;
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

    private void ClearSlidersSignature()
    {
        m_musicVolumeSlider.UnregisterValueChangedCallback(MusicSliderCallback);
        m_sfxVolumeSlider.UnregisterValueChangedCallback(SfxSliderCallback);
    }
    #endregion

    #region StringTextsMethods
    public void RefreshStaticLabels()
    {
        m_dragYourAnswerLabel.text = LocalizationManager.Get("Drag your answer here", "Arraste sua resposta aqui");
        m_musicVolumeSlider.label = LocalizationManager.Get("Music", "Música");
        m_nextHintButton.text = LocalizationManager.Get("Hint", "Dica");
        m_nextQuestionButton.text = LocalizationManager.Get("Next Question", "Próxima Questão");
    }

    public void SetTimer(string seconds)
    {
        m_timeLabel.text = LocalizationManager.Get(
            $"Time remaining: {seconds} seconds",
            $"Tempo restante: {seconds} segundos"
        );
    }

    public void SetHint(string hintText)
    {
        m_hintLabel.text = hintText;
    }

    public void SetHintNumber(int hintNumber)
    {
        m_hintNumberLabel.text = LocalizationManager.Get(
            $"Hint {hintNumber}: ",
            $"Dica {hintNumber}: "
        );
    }

    public void SetQuestionNumber(int questionNum)
    {
        m_questionNumLabel.text = LocalizationManager.Get(
            $"Question {questionNum}",
            $"Pergunta {questionNum}"
        );
    }

    public void SetCurrentScore(int currentScore)
    {
        m_currentScoreLabel.text = LocalizationManager.Get(
            $"Score: {currentScore}",
            $"Pontuação: {currentScore}"
        );
    }

    public void SetHighScore(int highScore)
    {
        m_highScoreLabel.text = LocalizationManager.Get(
            $"Highscore: {highScore}",
            $"Recorde: {highScore}"
        );
    }

    public void ShowScoreInFinalPanel(int finalScore)
    {
        if (finalScore < 200)
            m_showScoreFinalPanelLabel.text = LocalizationManager.Get(
                $"Your score: {finalScore}",
                $"Sua pontuação: {finalScore}"
            );
        else
            m_showScoreFinalPanelLabel.text = LocalizationManager.Get(
                $"Unbeatable!\nPerfect score: {finalScore}",
                $"Imbatível!\nPontuação perfeita: {finalScore}"
            );
    }

    public void GiveAnswerFeedback(bool correct, string answerName)
    {
        m_feedbackManager.GiveAnswerFeedback(correct, answerName);
    }

    public void ShowPointsScored(int scoreToShow, bool changeScoreFeedback)
    {
        m_feedbackManager.ShowPointsScored(scoreToShow, changeScoreFeedback);
    }

    public void RemoveIconByAnswer(string answer)
    {
        const string K_ICONS_BOARD_NAME = "IconsBoard";

        m_root.Query<VisualElement>(K_ICONS_BOARD_NAME)
            .Children<VisualElement>()
            .ForEach(icon =>
            {
                if (icon.userData is Question question && question.answer == answer)
                    icon.RemoveFromHierarchy();
            });
    }
    #endregion
}