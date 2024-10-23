using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class StaffController : MonoBehaviour
{
    private RectTransform m_recTransform;

    //

    public RectTransform Pointer { get; set; }

    public RectTransform Centre { get; set; }

    public Vector2 CentreInPointer
    {
        get
        {
            if (Centre == null)
                return new Vector2(-(Pointer.localPosition.x + 226 * 0.5f), 0);
            return Pointer.InverseTransformPoint(Centre.position);
        }
    }

    public Image Renderer => this.GetComponent<Image>();

    //

    private void Awake()
    {
        m_recTransform = GetComponent<RectTransform>();
    }

    //

    public void DoFixed()
    {
        m_recTransform.SetParent(Pointer, true);
        m_recTransform.localPosition = CentreInPointer;
        m_recTransform.SetSiblingIndex(Pointer.childCount - 1);
    }

    public void DoMoveNextJump(Action OnComplete)
    {
        m_recTransform.SetParent(Pointer, true);
        m_recTransform.SetSiblingIndex(Pointer.childCount - 1);
        m_recTransform
            .DOLocalJump(CentreInPointer, 45f, 1, 1f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => OnComplete?.Invoke());
    }

    public void DoMoveCentreLinear(Action OnComplete)
    {
        m_recTransform.SetParent(Pointer, true);
        m_recTransform.SetSiblingIndex(Pointer.childCount - 1);

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        m_recTransform
            .DOLocalMove(CentreInPointer, MoveDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => OnComplete?.Invoke());
    }

    public void DoMoveCentreJump(Action OnComplete)
    {
        m_recTransform.SetParent(Pointer, true);
        m_recTransform.SetSiblingIndex(Pointer.childCount - 1);

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, m_recTransform.DOScale(Vector3.one * 1.35f, MoveDuration * 0.5f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, m_recTransform.DOLocalJump(CentreInPointer, 50, 1, MoveDuration).SetEase(Ease.Linear));
        CardTween.Insert(MoveDuration * 0.5f, m_recTransform.DOScale(Vector3.one, MoveDuration * 0.5f).SetEase(Ease.InCirc));
        CardTween.OnComplete(() => OnComplete?.Invoke());
        CardTween.Play();
    }


    public void DoRumble(Action OnComplete)
    {
        var RumbleDuration = GameManager.instance.TweenConfig.CardAction.RumbleDuration;

        transform.SetSiblingIndex(Pointer.childCount - 1);
        Renderer.maskable = false;
        Renderer.transform.DOScale(Vector3.one * 1.35f, RumbleDuration * 0.8f).SetEase(Ease.OutQuad).OnComplete((TweenCallback)(() =>
        {
            Renderer.transform.DOScale(Vector3.one, RumbleDuration * 0.2f).SetEase(Ease.Linear).OnComplete((TweenCallback)(() =>
            {
                transform.SetSiblingIndex(Pointer.GetSiblingIndex() - 1);
                OnComplete?.Invoke();
            }));
        }));
    }
}