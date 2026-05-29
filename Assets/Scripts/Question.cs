using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "Questions/Question SO")]
public class Question : ScriptableObject
{
    public Texture2D icon;
    public string answer;
    public string answerPortuguese;

    [Space(20)] // Adds 20 pixels of space
    [Header("Hints")]
    public string[] hints;
    public string[] hintsPortuguese;

    public string GetAnswer()
    {
        if (LocalizationManager.CurrentLanguage == Language.Portuguese
            && !string.IsNullOrEmpty(answerPortuguese))
            return answerPortuguese;

        return answer;
    }

    public string[] GetHints()
    {
        if (LocalizationManager.CurrentLanguage == Language.Portuguese
            && hintsPortuguese != null
            && hintsPortuguese.Length > 0)
            return hintsPortuguese;

        if (hints == null || hints.Length == 0)
        {
            Debug.LogWarning($"Hints not initialized in question: {name}");
            return new string[0];
        }

        return hints;
    }
}