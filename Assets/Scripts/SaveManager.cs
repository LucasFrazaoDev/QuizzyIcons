using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string M_HIGHSCORE = "HighScore";

    public void SaveHighScore(int score)
    {
        PlayerPrefs.SetInt(M_HIGHSCORE, score);
        PlayerPrefs.Save();
    }

    public int LoadHighScore()
    {
        // Verify if exists a previous highscore
        return PlayerPrefs.HasKey(M_HIGHSCORE) ? PlayerPrefs.GetInt(M_HIGHSCORE) : 0;
    }
}


