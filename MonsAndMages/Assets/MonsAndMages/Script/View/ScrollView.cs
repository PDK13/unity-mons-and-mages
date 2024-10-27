using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool m_scrollAvaible = false;
    private Vector2 m_scrollValue = new Vector2();

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.PlayerChoice == ChoiceType.None || GameManager.instance.TutorialActive)
        {
            m_scrollAvaible = false;
            return;
        }
        m_scrollAvaible = true;
        m_scrollValue = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!m_scrollAvaible)
            return;
        if (Input.touchCount == 1)
            ScrollUpdate(Input.GetTouch(0).deltaPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_scrollAvaible = false;
        m_scrollValue = Vector2.zero;
    }

    private void ScrollUpdate(Vector2 delta)
    {
        m_scrollValue += new Vector2(delta.x, delta.y) * Time.fixedDeltaTime;

        if (m_scrollValue.y < -1)
        {
            m_scrollValue = new Vector2();
            GameEvent.onViewArea(ViewType.Wild, null);
        }
        else
        if (m_scrollValue.y > 1)
        {
            m_scrollValue = new Vector2();
            GameEvent.ViewArea(ViewType.Field, null);
        }
    }
}
