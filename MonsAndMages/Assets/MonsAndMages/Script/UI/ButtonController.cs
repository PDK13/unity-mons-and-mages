using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void Start()
    {
        m_button.onClick.AddListener(() => ButtonPress());
    }

    private void OnEnable()
    {
        GameEvent.onButtonInteractable += OnButtonInteractable;
    }

    private void OnDisable()
    {
        GameEvent.onButtonInteractable -= OnButtonInteractable;
    }

    private void OnButtonInteractable(bool Interactable)
    {
        m_button.interactable = Interactable;
    }

    private void ButtonPress()
    {
        GameEvent.ButtonPressed(this.m_button);
    }
}