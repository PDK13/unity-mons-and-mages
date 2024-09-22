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
        throw new NotImplementedException();
    }

    private void OnPassiveActive(Action OnComplete)
    {
        throw new NotImplementedException();
    }


    private void OnSpellActive(Action OnComplete)
    {
        throw new NotImplementedException();
    }
}