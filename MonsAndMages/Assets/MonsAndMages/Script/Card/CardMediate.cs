using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardMediate : MonoBehaviour
{
    [SerializeField] private bool m_eventActive = true;
    [SerializeField] private GameObject m_renderer;
    [SerializeField] private GameObject m_rendererAlpha;

    private bool m_effect = false;
    private bool m_ready = false;
    private Button m_button;

    private IPlayer Player => GetComponentInParent<IPlayer>();

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        GameEvent.onPlayerTakeRuneStoneFromMediation += OnPlayerTakeRuneStoneFromMediation;
    }

    private void OnDisable()
    {
        GameEvent.onPlayerTakeRuneStoneFromMediation -= OnPlayerTakeRuneStoneFromMediation;
    }

    public void Start()
    {
        m_rendererAlpha.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void BtnTap()
    {
        //...
    }

    //

    public void Effect(CardEffectType Type, float Duration, Action OnComplete)
    {
        switch (Type)
        {
            case CardEffectType.Alpha:
                EffectAlpha(Duration, OnComplete);
                break;
        }
    }

    public void EffectAlpha(float Duration, Action OnComplete)
    {
        if (m_effect)
            return;
        m_effect = true;
        var AlphaGroup = m_rendererAlpha.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(0.25f, Duration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            AlphaGroup.DOFade(0f, Duration * 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_effect = false;
                OnComplete?.Invoke();
            });
        });
    }

    //

    private void OnPlayerTakeRuneStoneFromMediation(IPlayer Player, Action OnComplete)
    {
        if (!Player.Equals(this.Player))
            return;
        EffectAlpha(1f, m_eventActive ? OnComplete : null);
    }
}