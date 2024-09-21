using System;
using UnityEngine;

public class CardOneTail : MonoBehaviour
{
    private CardController m_controller;

    private void Awake()
    {
        m_controller = GetComponent<CardController>();
    }
}