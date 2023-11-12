using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionsDatabase", menuName = "Questions/Questions Database SO")]
public class QuestionsDatabase : ScriptableObject
{
    public Question[] questions;
}
