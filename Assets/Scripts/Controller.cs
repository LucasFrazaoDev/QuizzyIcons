using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Game m_game;

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
        
    }
}
