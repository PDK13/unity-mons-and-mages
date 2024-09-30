using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardForestWing : MonoBehaviour
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
        OnComplete?.Invoke();
    }
}