using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public static PlayerView instance;

    [SerializeField] private GameObject m_btnCollect;
    [SerializeField] private GameObject m_btnBack;
    [SerializeField] private GameObject m_playerContent;
    [SerializeField] private GameObject m_startBox;

    [SerializeField] private TextMeshProUGUI m_tmpRuneStone;

    private IPlayer m_playerBase;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GameEvent.onInit += OnInit;
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onGameStart += OnGameStart;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onGameStart -= OnGameStart;
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

    private void OnInitPlayer(PlayerData[] Player)
    {
        foreach (var PlayerCheck in Player)
        {
            if (!PlayerCheck.Base)
                continue;
            m_playerBase = PlayerCheck.Player;
            m_tmpRuneStone.text = PlayerCheck.Player.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
        }
    }

    private void OnGameStart(Action OnComplete)
    {
        RectTransform StartBox = m_startBox.GetComponent<RectTransform>();
        StartBox.anchoredPosition = Vector3.up * 100;
        StartBox.DOAnchorPosY(-100, 0.5f).OnComplete(() =>
        {
            StartBox.DOAnchorPosY(100, 0.25f).OnComplete(() =>
            {
                m_btnCollect.SetActive(GameManager.instance.PlayerView);
                m_btnBack.SetActive(false);
                m_playerContent.SetActive(GameManager.instance.PlayerView);
                //
                OnComplete?.Invoke();
            }).SetDelay(0.25f);
        });
    }
}