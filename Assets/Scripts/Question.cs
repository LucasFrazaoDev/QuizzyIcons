using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "Questions/Question SO")]
public class Question : ScriptableObject
{
    public string answer;
    public string[] hints = new string[3];

    public string[] GetHints()
    {
        if (hints.Length == 0)
            Debug.Log("Hints not initialized!");

        return hints;
    }
}
