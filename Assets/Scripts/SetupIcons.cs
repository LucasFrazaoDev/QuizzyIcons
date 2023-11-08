using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SetupIcons
{
    private const string K_ICONS_BOARD_NAME = "IconsBoard";
    private const string K_CURSOR_POINTER_CLASS_NAME = "CursorPointerButtons";

    public static void InitializeDragDrop(VisualElement root, Controller controller)
    {
        root.Query<VisualElement>(K_ICONS_BOARD_NAME)
            .Children<VisualElement>()
            .ForEach(child =>
            {
                child.AddManipulator(new IconDragger(root, controller));
                child.AddToClassList(K_CURSOR_POINTER_CLASS_NAME);
            });
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
