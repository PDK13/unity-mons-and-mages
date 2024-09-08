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

        GameEvent.onViewPlayer += OnViewPlayer;
        GameEvent.onViewWild += OnViewCollect;
        GameEvent.onViewField += OnViewBack;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;

        GameEvent.onViewPlayer -= OnViewPlayer;
        GameEvent.onViewWild -= OnViewCollect;
        GameEvent.onViewField -= OnViewBack;
    }

    //

    public void BtnViewPlayer(int PlayerIndex)
    {
        GameEvent.ViewPlayer(GameManager.instance.GetPlayer(PlayerIndex), false);
    }

    public void BtnViewCollect()
    {
        GameEvent.ViewWild(false);
    }

    public void BtnViewBack()
    {
        GameEvent.ViewField(false);
    }

    //

    private void OnInit()
    {
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.SetActive(false);
    }


    private void OnViewPlayer(IPlayer Player, bool Update)
    {
        if (!Update)
        {
            m_btnCollect.SetActive(false);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(false);
        }
        else
        {
            m_btnCollect.SetActive(GameManager.instance.PlayerView);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(GameManager.instance.PlayerView);
        }
    }

    private void OnViewCollect(bool Update)
    {
        if (!Update)
        {
            m_btnCollect.SetActive(false);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(false);
        }
        else
        {
            m_btnCollect.SetActive(false);
            m_btnBack.SetActive(GameManager.instance.PlayerView);
            m_playerContent.SetActive(false);
        }
    }

    private void OnViewBack(bool Update)
    {
        if (!Update)
        {
            m_btnCollect.SetActive(false);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(false);
        }
        else
        {
            m_btnCollect.SetActive(GameManager.instance.PlayerView);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(GameManager.instance.PlayerView);
        }
    }
}