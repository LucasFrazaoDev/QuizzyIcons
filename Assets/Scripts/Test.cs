using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{
    private VisualElement m_root;

    private void Awake()
    {
        m_root = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        VisualElement icon = m_root.Q("TestIcon");
        icon.AddManipulator(new IconDragger(m_root));
    }
}
