using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;

    public LeaderboardEntry(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public static class LeaderboardManager
{
    private const string K_LEADERBOARD_KEY = "Leaderboard";
    private const int K_MAX_ENTRIES = 10;

    public static List<LeaderboardEntry> LoadEntries()
    {
        string json = PlayerPrefs.GetString(K_LEADERBOARD_KEY, "{}");
        LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
        return data.entries ?? new List<LeaderboardEntry>();
    }

    public static void SaveScore(string playerName, int score)
    {
        List<LeaderboardEntry> entries = LoadEntries();

        LeaderboardEntry existing = entries.FirstOrDefault(e =>
            e.playerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            if (score > existing.score)
                existing.score = score;
        }
        else
        {
            entries.Add(new LeaderboardEntry(playerName, score));
        }

        entries = entries
            .OrderByDescending(e => e.score)
            .Take(K_MAX_ENTRIES)
            .ToList();

        LeaderboardData data = new LeaderboardData { entries = entries };
        PlayerPrefs.SetString(K_LEADERBOARD_KEY, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }
}