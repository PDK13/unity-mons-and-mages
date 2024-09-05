using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TopView : MonoBehaviour
{
    [SerializeField] private GameObject m_btnCollect;
    [SerializeField] private GameObject m_btnBack;
    [SerializeField] private GameObject m_playerContent;

    private void OnEnable()
    {
        GameEvent.onViewPlayer += OnViewPlayer;
        GameEvent.onViewCollect += OnViewCollect;
        GameEvent.onViewBack += OnViewBack;
    }

    private void OnDisable()
    {
        GameEvent.onViewPlayer -= OnViewPlayer;
        GameEvent.onViewCollect -= OnViewCollect;
        GameEvent.onViewBack -= OnViewBack;
    }

    //

    public void BtnViewPlayer(int PlayerIndex)
    {
        GameEvent.ViewPlayer(GameManager.instance.GetPlayer(PlayerIndex), false);
    }

    public void BtnViewCollect()
    {
        GameEvent.ViewCollect(false);
    }

    public void BtnViewBack()
    {
        GameEvent.ViewBack(false);
    }

    //

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
            m_btnCollect.SetActive(true);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(true);
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
            m_btnBack.SetActive(true);
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
            m_btnCollect.SetActive(true);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(true);
        }
    }
}