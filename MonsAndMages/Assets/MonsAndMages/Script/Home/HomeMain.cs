using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeMain : MonoBehaviour
{
    [SerializeField] private Button m_btnTutorial;
    [SerializeField] private Button m_btnPlay;

    private void Awake()
    {

    }

    private void Start()
    {
        m_btnPlay.interactable = false;
    }

    //

    public void BtnPlay() { }

    public void BtnTutorial()
    {

    }
}