using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class PlayerViewButton : MonoBehaviour
{
    [SerializeField] private Transform m_boxIndex;
    [SerializeField] private Transform m_boxHealth;
    [SerializeField] private Transform m_boxStun;

    [Space]
    [SerializeField] private TextMeshProUGUI m_tmpIndex;
    [SerializeField] private TextMeshProUGUI m_tmpHealth;
    [SerializeField] private TextMeshProUGUI m_tmpStun;

    private int m_healthLast = 0;
    private int m_stunLast = 0;

    //

    public PlayerView View { set; get; }

    public int Health { set => m_tmpHealth.text = value.ToString(); }

    public int Stun { set => m_tmpStun.text = value.ToString(); }

    //

    public void HealthUpdate(int Value, Action OnComplete)
    {
        this.transform.DOScale(Vector2.one * 1.2f, 0.1f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            m_tmpHealth.text = Value.ToString();
            this.transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.Linear).OnComplete(() => OnComplete?.Invoke());
            //
            HealthEffect(Value, m_healthLast);
            m_healthLast = Value;
        });
    }

    public void StunUpdate(int Value, Action OnComplete)
    {
        this.transform.DOScale(Vector2.one * 1.2f, 0.1f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            m_tmpStun.text = Value.ToString();
            this.transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.Linear).OnComplete(() => OnComplete?.Invoke());
            //
            StunEffect(Value, m_stunLast);
            m_stunLast = Value;
        });
    }

    //

    public void HealthEffect(int ValueCurrent, int ValueLast)
    {
        var ValueOffset = ValueCurrent - ValueLast;
        if (ValueOffset == 0)
            return;
        var TmpClone = Instantiate(m_tmpHealth, View.transform);
        TmpClone.text = (ValueOffset > 0 ? "+" : "") + ValueOffset.ToString();
        TmpClone.DOFade(0f, 1f).SetDelay(0.5f);
        TmpClone.transform.position = m_tmpHealth.transform.position;
        TmpClone.transform.localScale = Vector3.one;
        TmpClone.gameObject.SetActive(true);
        var TmpRecTransform = TmpClone.GetComponent<RectTransform>();
        TmpRecTransform.anchoredPosition += Vector2.up * 25;
        TmpRecTransform.DOAnchorPosY(TmpRecTransform.anchoredPosition.y + 50f, 1.5f).OnComplete(() => Destroy(TmpClone));
    }

    public void StunEffect(int ValueCurrent, int ValueLast)
    {
        var ValueOffset = ValueCurrent - ValueLast;
        if (ValueOffset == 0)
            return;
        var TmpClone = Instantiate(m_tmpStun, View.transform);
        TmpClone.text = (ValueOffset > 0 ? "+" : "") + ValueOffset.ToString();
        TmpClone.DOFade(0f, 1f).SetDelay(0.5f);
        TmpClone.transform.position = m_tmpStun.transform.position;
        TmpClone.transform.localScale = Vector3.one;
        TmpClone.gameObject.SetActive(true);
        var TmpRecTransform = TmpClone.GetComponent<RectTransform>();
        TmpRecTransform.anchoredPosition += Vector2.up * 25;
        TmpRecTransform.DOAnchorPosY(TmpRecTransform.anchoredPosition.y + 50f, 1.5f).OnComplete(() => Destroy(TmpClone));
    }
}