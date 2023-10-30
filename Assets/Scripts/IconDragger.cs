using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IconDragger : MouseManipulator
{
    private Vector2 m_startPos;
    private Vector2 m_elemStartPosGlobal;
    private Vector2 m_elemStartPosLocal;

    private VisualElement m_dragArea;
    private VisualElement m_iconContainer;
    private VisualElement m_dropZone;

    private bool m_isActive;

    public IconDragger(VisualElement root)
    {
        m_dragArea = root.Q("DragArea");
        m_dropZone = root.Q("DropBox");

        m_isActive = false;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void OnMouseDown(MouseDownEvent e)
    {
        m_iconContainer = target.parent;

        // Mouse start position
        m_startPos = e.localMousePosition;

        // Get both target start pos
        m_elemStartPosGlobal = target.worldBound.position;
        m_elemStartPosLocal = target.layout.position;

        // Enable drag area
        m_dragArea.style.display = DisplayStyle.Flex;
        m_dragArea.Add(target);

        // Correct pos after repositioning
        target.style.top = m_elemStartPosGlobal.y;
        target.style.left = m_elemStartPosGlobal.x;

        m_isActive = true;
        target.CaptureMouse();
        e.StopPropagation();
    }

    private void OnMouseMove(MouseMoveEvent e)
    {
        if (!m_isActive || !target.HasMouseCapture()) return;

        Vector2 diff = e.localMousePosition - m_startPos;

        target.style.top = target.layout.y + diff.y;
        target.style.left = target.layout.x + diff.x;

        e.StopPropagation();
    }

    private void OnMouseUp(MouseUpEvent e)
    {
        if (!m_isActive || !target.HasMouseCapture()) return;

        if (target.worldBound.Overlaps(m_dropZone.worldBound))
        {
            m_dropZone.Add(target);

            target.style.top = m_dropZone.contentRect.center.y - target.layout.height / 2;
            target.style.left = m_dropZone.contentRect.center.x - target.layout.width / 2;
        }
        else
        {
            m_iconContainer.Add(target);

            target.style.top = m_elemStartPosLocal.y - m_iconContainer.contentRect.position.y;
            target.style.left = m_elemStartPosLocal.x - m_iconContainer.contentRect.position.x;
        }

        m_isActive = false;
        target.ReleaseMouse();
        e.StopPropagation();

        m_dragArea.style.display = DisplayStyle.None;
    }
}