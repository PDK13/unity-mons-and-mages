using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class UISafeArea : MonoBehaviour
{
    private void Start()
    {
        SetUpdateSafeArea();
    }

#if UNITY_EDITOR
    private void Update()
    {
        SetUpdateSafeArea();
    }
#endif

    private void SetUpdateSafeArea()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        Rect safeRect = Screen.safeArea;

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;

        Vector2 anchorPos = rectTransform.anchoredPosition;
        anchorPos.x = safeRect.x / 2;
        rectTransform.anchoredPosition = anchorPos;

        Vector2 size = rectTransform.sizeDelta;
        size.x = -safeRect.x;
        rectTransform.sizeDelta = size;
    }
}
