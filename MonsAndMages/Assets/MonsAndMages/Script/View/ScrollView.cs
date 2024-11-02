using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private float m_zoomSpeed = 0.01f;
    [SerializeField] private Transform m_zoomTarget;

    private float m_zoomValue = 0;
    private bool m_scrollAvaible = false;
    private Vector2 m_scrollValue = new Vector2();

#if UNITY_EDITOR
    public void Update()
    {
        ZoomUpdate(Input.GetAxis("Mouse ScrollWheel") * 100f * m_zoomSpeed);
    }
#endif

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
        if (Input.touchCount == 2)
        {
            Touch touch01 = Input.GetTouch(0);
            Touch touch02 = Input.GetTouch(1);

            Vector2 touch01PrevPos = touch01.position - touch01.deltaPosition;
            Vector2 touch02PrevPos = touch02.position - touch02.deltaPosition;

            float lengthPrev = (touch01PrevPos - touch02PrevPos).magnitude;
            float lengthCurrent = (touch01.position - touch02.position).magnitude;

            float offset = lengthCurrent - lengthPrev;

            ZoomUpdate(offset * m_zoomSpeed);
        }

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

    private void ZoomUpdate(float ZoomValue)
    {
        m_zoomValue = ZoomValue;
        m_zoomTarget.localScale += Vector3.one * ZoomValue;
        if (m_zoomTarget.localScale.x < 0)
            m_zoomTarget.localScale = Vector3.zero;
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