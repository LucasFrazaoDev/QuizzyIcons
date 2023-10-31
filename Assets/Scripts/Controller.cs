using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Game m_game;
    private UILinker m_uiLinker;

    private void Awake()
    {
        m_game = GetComponent<Game>();
        m_uiLinker = GetComponent<UILinker>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        m_game.InitializeGame();
        UpdateUI();
    }

    public void HandleWrongAnswer()
    {
        m_game.HandleWrongtAnswer();
        UpdateUI();
    }

    public void HandleCorrectAnswer()
    {
        m_game.HandleCorrectAnswer();
        UpdateUI();
    }

    public void UpdateUI()
    {
        m_uiLinker.SetHint(m_game.GetCurrentHint());
        m_uiLinker.SetHintNumber(m_game.GetCurrentHintNum());
        m_uiLinker.SetQuestionNumber(m_game.GetCurrentQuestionNum());
    }
}
