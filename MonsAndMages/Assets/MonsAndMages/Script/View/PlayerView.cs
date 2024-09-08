using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private GameObject m_btnCollect;
    [SerializeField] private GameObject m_btnBack;
    [SerializeField] private GameObject m_playerContent;

    private void OnEnable()
    {
        GameEvent.onInit += OnInit;

        GameEvent.onPlayerStart += OnPlayerStart;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;

        GameEvent.onPlayerStart -= OnPlayerStart;
    }

    //

    public void BtnViewPlayer(int PlayerIndex)
    {
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.SetActive(false);
        GameEvent.ViewPlayer(GameManager.instance.GetPlayer(PlayerIndex), () =>
        {
            m_btnCollect.SetActive(GameManager.instance.PlayerView);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(GameManager.instance.PlayerView);
        });
    }

    public void BtnViewCollect()
    {
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.SetActive(false);
        GameEvent.ViewWild(() =>
        {
            m_btnCollect.SetActive(false);
            m_btnBack.SetActive(GameManager.instance.PlayerView);
            m_playerContent.SetActive(false);
        });
    }

    public void BtnViewBack()
    {
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.SetActive(false);
        GameEvent.ViewField(() =>
        {
            m_btnCollect.SetActive(GameManager.instance.PlayerView);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(GameManager.instance.PlayerView);
        });
    }

    //

    private void OnInit()
    {
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.SetActive(false);
    }

    private void OnPlayerStart(IPlayer Player, Action OnComplete)
    {
        if (Player.Base)
        {
            m_btnCollect.SetActive(GameManager.instance.PlayerView);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(GameManager.instance.PlayerView);
        }
    }
}