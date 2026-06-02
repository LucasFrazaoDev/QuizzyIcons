using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartPanelManager : MonoBehaviour
{
    private Controller m_controller;
    //private UIManager m_uiManager;

    private VisualElement m_root;
    private VisualElement m_startPanelOverlay;
    private VisualElement m_leaderboardContainer;
    private VisualElement m_howToPlayOverlay;
    private VisualElement m_startPanel;

    private Label m_languageLabel;
    private Label m_instructionsLabel;

    private TextField m_playerNameTextField;

    private Button m_englishButton;
    private Button m_portugueseButton;
    private Button m_startGameButton;
    private Button m_instructionsButton;
    private Button m_startSceneQuitButton;
    private Button m_closeInstructionsButton;

    private const string K_START_PANEL_OVERLAY_NAME = "StartPanelOverlay";
    private const string K_START_PANEL_NAME = "StartPanel";
    private const string K_LEADERBOARD_CONTAINER_NAME = "LeaderboardContainer";
    private const string K_HOW_TO_PLAY_OVERLAY_NAME = "HowToPlayOverlay";

    private const string K_LANGUAGE_LABEL_NAME = "LanguageLabel";
    private const string K_INSTRUCTIONS_LABEL_NAME = "InstructionsLabel";

    private const string K_PLAYER_NAME_TEXT_FIELD_NAME = "PlayerNameTextField";

    private const string K_ENGLISH_BUTTON_NAME = "StartEnglishButton";
    private const string K_PORTUGUESE_BUTTON_NAME = "StartPortugueseButton";
    private const string K_START_GAME_BUTTON_NAME = "StartGameButton";
    private const string K_INSTRUCTIONS_BUTTON_NAME = "InstructionsButton";
    private const string K_START_SCENE_QUIT_BUTTON_NAME = "StartSceneQuitButton";
    private const string K_CLOSE_INSTRUCTIONS_BUTTON_NAME = "CloseInstructionsButton";

    private const string K_LANGUAGE_BUTTON_ACTIVE_CLASS = "language-button-active";
    private const int K_MIN_NAME_LENGTH = 3;
    private const int K_MAX_NAME_LENGTH = 50;

    private const string K_CLASS_TO_HIDE_PANEL_NAME = "HidePanelTransition";

    private void Awake()
    {
        m_root = GetComponent<UIDocument>().rootVisualElement;
        m_controller = GetComponent<Controller>();
    }

    private void OnEnable()
    {
        GetReferences();
        InitializeButtons();
        InitializeTextField();

        LocalizationManager.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnDisable()
    {
        ClearButtons();

        LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
    }

    private void Start()
    {
        m_howToPlayOverlay.style.display = DisplayStyle.None;
        RefreshTexts();
        PopulateLeaderboard();
        UpdateLanguageButtons();
    }

    private void GetReferences()
    {
        m_startPanelOverlay = m_root.Q(K_START_PANEL_OVERLAY_NAME);
        m_startPanel = m_root.Q(K_START_PANEL_NAME);
        m_leaderboardContainer = m_root.Q(K_LEADERBOARD_CONTAINER_NAME);
        m_howToPlayOverlay = m_root.Q(K_HOW_TO_PLAY_OVERLAY_NAME);

        m_languageLabel = m_root.Q<Label>(K_LANGUAGE_LABEL_NAME);
        m_instructionsLabel = m_root.Q<Label>(K_INSTRUCTIONS_LABEL_NAME);

        m_playerNameTextField = m_root.Q<TextField>(K_PLAYER_NAME_TEXT_FIELD_NAME);

        m_englishButton = m_root.Q<Button>(K_ENGLISH_BUTTON_NAME);
        m_portugueseButton = m_root.Q<Button>(K_PORTUGUESE_BUTTON_NAME);
        m_startGameButton = m_root.Q<Button>(K_START_GAME_BUTTON_NAME);
        m_instructionsButton = m_root.Q<Button>(K_INSTRUCTIONS_BUTTON_NAME);
        m_startSceneQuitButton = m_root.Q<Button>(K_START_SCENE_QUIT_BUTTON_NAME);
        m_closeInstructionsButton = m_root.Q<Button>(K_CLOSE_INSTRUCTIONS_BUTTON_NAME);
    }

    private void InitializeButtons()
    {
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

        m_startGameButton.clicked += TryStartGame;
        m_instructionsButton.clicked += OpenHowToPlay;
        m_closeInstructionsButton.clicked += CloseHowToPlay;
        m_startSceneQuitButton.clicked += () => Application.Quit();
    }

    private void ClearButtons()
    {
        m_startGameButton.clicked -= TryStartGame;
        m_instructionsButton.clicked -= OpenHowToPlay;
        m_closeInstructionsButton.clicked -= CloseHowToPlay;
    }

    private void InitializeTextField()
    {
        m_playerNameTextField.maxLength = K_MAX_NAME_LENGTH;
    }

    private void TryStartGame()
    {
        string playerName = m_playerNameTextField.value.Trim();

        if (playerName.Length < K_MIN_NAME_LENGTH)
        {
            m_playerNameTextField.AddToClassList("invalid-field");
            return;
        }

        m_playerNameTextField.RemoveFromClassList("invalid-field");

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        StartCoroutine(HideStartPanel());
    }

    private IEnumerator HideStartPanel()
    {
        m_startPanel.AddToClassList(K_CLASS_TO_HIDE_PANEL_NAME);
        yield return new WaitForSeconds(0.25f);
        m_startPanelOverlay.style.display = DisplayStyle.None;
        m_controller.StartGame();
    }

    private void OpenHowToPlay()
    {
        m_howToPlayOverlay.style.display = DisplayStyle.Flex;
    }

    private void CloseHowToPlay()
    {
        m_howToPlayOverlay.style.display = DisplayStyle.None;
    }

    public void ShowStartPanel()
    {
        m_startPanelOverlay.style.display = DisplayStyle.Flex;
        m_howToPlayOverlay.style.display = DisplayStyle.None;
        PopulateLeaderboard();
        m_playerNameTextField.value = PlayerPrefs.GetString("PlayerName", "");
    }

    private void PopulateLeaderboard()
    {
        m_leaderboardContainer.Clear();

        VisualElement header = new VisualElement();
        header.AddToClassList("leaderboard-entry");

        // Criando os cabeçalhos das colunas com base na linguagem atual
        Label rankHeader = new Label(LocalizationManager.Get("Pos.", "Pos."));
        Label nameHeader = new Label(LocalizationManager.Get("Player", "Jogador"));
        Label scoreHeader = new Label(LocalizationManager.Get("Score", "Pontuação"));

        rankHeader.style.width = 60;
        rankHeader.style.fontSize = 24;
        rankHeader.style.unityFontStyleAndWeight = FontStyle.Bold;

        nameHeader.style.flexGrow = 1;
        nameHeader.style.fontSize = 24;
        nameHeader.style.unityFontStyleAndWeight = FontStyle.Bold;

        scoreHeader.style.minWidth = 60;
        scoreHeader.style.fontSize = 24;
        scoreHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        scoreHeader.style.unityTextAlign = TextAnchor.MiddleRight;

        header.Add(rankHeader);
        header.Add(nameHeader);
        header.Add(scoreHeader);

        // Adiciona o cabeçalho à tabela
        m_leaderboardContainer.Add(header);

        // Carrega as entradas do leaderboard
        List<LeaderboardEntry> entries = LeaderboardManager.LoadEntries();

        if (entries.Count == 0)
        {
            Label emptyLabel = new Label(LocalizationManager.Get(
                "No scores yet. Be the first!",
                "Nenhuma pontuação ainda. Seja o primeiro!"));
            emptyLabel.name = "EmptyLeaderboardLabel";
            m_leaderboardContainer.Add(emptyLabel);
            return;
        }

        for (int i = 0; i < entries.Count; i++)
        {
            VisualElement entry = new VisualElement();
            entry.AddToClassList("leaderboard-entry");

            Label rankLabel = new Label($"{i + 1}.");
            Label nameLabel = new Label(entries[i].playerName);
            Label scoreLabel = new Label(entries[i].score.ToString());

            rankLabel.style.width = 60;
            rankLabel.style.fontSize = 24;

            nameLabel.style.flexGrow = 1;
            nameLabel.style.fontSize = 24;

            scoreLabel.style.minWidth = 60;
            scoreLabel.style.fontSize = 24;
            scoreLabel.style.unityTextAlign = TextAnchor.MiddleRight;

            entry.Add(rankLabel);
            entry.Add(nameLabel);
            entry.Add(scoreLabel);

            m_leaderboardContainer.Add(entry);
        }
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

    private void RefreshTexts()
    {
        m_languageLabel.text = LocalizationManager.Get("Language:", "Idioma:");

        m_instructionsLabel.text = LocalizationManager.Get(
            "Drag the icon corresponding to the hint to the indicated location.\n\nExtra Hint: -5 points\nNext question/Wrong answer: -8 points\nCorrect answer: +20 points\n\nAttention!! If you answer with the wrong icon you may lose the chance later.",
            "Arraste o ícone correspondente à dica para o local indicado.\n\nDica extra: -5 pontos\nPróxima pergunta/Resposta errada: -8 pontos\nResposta correta: +20 pontos\n\nAtenção!! Se responder com o ícone errado você pode perder a chance posteriormente."
        );

        m_playerNameTextField.label = LocalizationManager.Get(
            "Enter your name:",
            "Digite seu nome:"
        );
    }

    private void OnLanguageChanged()
    {
        RefreshTexts();
        UpdateLanguageButtons();
        PopulateLeaderboard();
    }
}