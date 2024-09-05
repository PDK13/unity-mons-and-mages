using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [SerializeField] private GameObject m_mask;
    [SerializeField] private GameObject m_renderer;

    private Tweener m_tween;

    public void BtnTap()
    {
        Debug.Log("Card tap invoke");
    }

    public void Init(Sprite Image)
    {
        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        m_renderer.GetComponent<Image>().sprite = Image;
    }

    public void Open(float Duration, Action OnComplete)
    {
        m_tween.Kill();
        this.transform.eulerAngles = Vector3.zero;
        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        m_tween = this.transform
            .DOLocalRotate(Vector3.up * 90f, Duration / 2)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_mask.SetActive(false);
                m_renderer.SetActive(true);
                m_tween = this.transform
                    .DOLocalRotate(Vector3.zero, Duration / 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => OnComplete?.Invoke());
            });
    }

    public void Close(float Duration, Action OnComplete)
    {
        m_tween.Kill();
        this.transform.eulerAngles = Vector3.zero;
        m_mask.SetActive(false);
        m_renderer.SetActive(true);
        m_tween = this.transform
            .DOLocalRotate(Vector3.up * 90f, Duration / 2)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_mask.SetActive(true);
                m_renderer.SetActive(false);
                m_tween = this.transform
                    .DOLocalRotate(Vector3.zero, Duration / 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => OnComplete?.Invoke());
            });
    }
}