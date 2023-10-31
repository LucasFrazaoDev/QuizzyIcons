using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SetupIcons
{
    public static void InitializeDragDrop(VisualElement root)
    {
        root.Query<VisualElement>("IconsBoard")
            .Children<VisualElement>()
            .ForEach(child => { child.AddManipulator(new IconDragger(root)); });
    }

    public static void InitializeIcons(VisualElement root, List<Question> questions)
    {
        int currentIconIndex = 0;

        foreach (Question question in questions)
        {
            VisualElement questionIcon = root.Query<VisualElement>("IconsBoard").Children<VisualElement>().AtIndex(currentIconIndex);
            questionIcon.style.backgroundImage = Resources.Load<Texture2D>("img/" + question.answer);
            questionIcon.userData = question;

            currentIconIndex++;
        }
    }
}
