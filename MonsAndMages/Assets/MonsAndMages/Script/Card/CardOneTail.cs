using System;
using UnityEngine;

public class CardOneTail : MonoBehaviour
{
    private CardController m_controller;

    //

    private void Awake()
    {
        m_controller = GetComponent<CardController>();
    }

    private void OnEnable()
    {
        m_controller.onEnterActive += OnEnterActive;
        m_controller.onPassiveActive += OnPassiveActive;

        m_controller.onSpellActive += OnSpellActive;
    }

    private void OnDisable()
    {
        m_controller.onEnterActive -= OnEnterActive;
        m_controller.onPassiveActive -= OnPassiveActive;

        m_controller.onSpellActive -= OnSpellActive;
    }

    //

    private void OnEnterActive(Action OnComplete)
    {
        OnComplete?.Invoke();
    }

    private void OnPassiveActive(Action OnComplete)
    {
        OnComplete?.Invoke();
    }


    private void OnSpellActive(Action OnComplete)
    {
        m_controller.DoGrowthAdd(1, () => m_controller.DoAttackActive(OnComplete));
    }
}