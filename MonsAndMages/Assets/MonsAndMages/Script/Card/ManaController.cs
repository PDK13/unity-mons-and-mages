using DG.Tweening;
using UnityEngine;

public class ManaController : MonoBehaviour
{
    private float m_posY = 30f;
    private int m_count;
    private Vector3 m_scale;
    private int m_manaLast = 0;

    private void Awake()
    {
        m_count = this.transform.childCount;
        m_scale = this.transform.GetChild(0).localScale;
    }

    private void Start()
    {
        for (int i = 0; i < m_count; i++)
            this.transform.GetChild(i).gameObject.SetActive(false);
    }

    public void InfoManaUpdate(int Value)
    {
        Value = Mathf.Clamp(Value, 0, m_count);
        //
        if (m_manaLast < Value)
        {
            for (int i = m_manaLast; i < Value; i++)
            {
                var Icon = this.transform.GetChild(i);
                var RecTransformIcon = Icon.GetComponent<RectTransform>();
                RecTransformIcon.anchoredPosition = new Vector2(RecTransformIcon.anchoredPosition.x, m_posY);
                RecTransformIcon.localScale = Vector3.zero;
                RecTransformIcon.gameObject.SetActive(true);
                RecTransformIcon.DOScale(m_scale, 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
                    RecTransformIcon.DOAnchorPos3DY(0, 0.1f));
            }
        }
        else
        if (m_manaLast > Value)
        {
            for (int i = m_manaLast - 1; i >= Value; i--)
            {
                var Icon = this.transform.GetChild(i);
                var RecTransformIcon = Icon.GetComponent<RectTransform>();
                RecTransformIcon.DOAnchorPos3DY(m_posY, 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
                    RecTransformIcon.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
                        RecTransformIcon.gameObject.SetActive(false)));
            }
        }
        //
        m_manaLast = Value;
    }
}