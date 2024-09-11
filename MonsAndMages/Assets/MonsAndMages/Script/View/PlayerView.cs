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

    [SerializeField] private GameObject m_btnMediate;
    [SerializeField] private GameObject m_btnCollect;
    [SerializeField] private GameObject m_btnBack;

    [SerializeField] private GameObject m_playerContent;
    [SerializeField] private GameObject m_runeStoneShow;
    [SerializeField] private GameObject m_runeStoneSupply;

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

        GameEvent.onPlayerStart += OnPlayerStart;
        GameEvent.onPlayerTakeRuneStoneFromSupply += OnPlayerTakeRuneStoneFromSupply;
        GameEvent.onPlayerTakeRuneStoneFromMediation += OnPlayerTakeRuneStoneFromMediation;
        GameEvent.onPlayerDoChoice += OnPlayerDoChoice;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;
        GameEvent.onInitPlayer -= OnInitPlayer;

        GameEvent.onPlayerStart -= OnPlayerStart;
        GameEvent.onPlayerTakeRuneStoneFromSupply -= OnPlayerTakeRuneStoneFromSupply;
        GameEvent.onPlayerTakeRuneStoneFromMediation -= OnPlayerTakeRuneStoneFromMediation;
        GameEvent.onPlayerDoChoice -= OnPlayerDoChoice;
    }

    //

    public void BtnViewPlayer(int PlayerIndex)
    {
        m_btnMediate.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.SetActive(false);
        GameEvent.ViewPlayer(GameManager.instance.GetPlayer(PlayerIndex), () =>
        {
            m_btnMediate.SetActive(GameManager.instance.PlayerView);
            m_btnCollect.SetActive(GameManager.instance.PlayerView);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(GameManager.instance.PlayerView);
        });
    }

    public void BtnViewMediate()
    {

    }

    public void BtnViewCollect()
    {
        m_btnCollect.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.SetActive(false);
        GameEvent.ViewWild(() =>
        {
            m_btnMediate.SetActive(false);
            m_btnCollect.SetActive(false);
            m_btnBack.SetActive(GameManager.instance.PlayerView);
            m_playerContent.SetActive(false);
        });
    }

    public void BtnViewBack()
    {
        m_btnMediate.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.SetActive(false);
        GameEvent.ViewField(() =>
        {
            m_btnMediate.SetActive(GameManager.instance.PlayerView);
            m_btnCollect.SetActive(GameManager.instance.PlayerView);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(GameManager.instance.PlayerView);
        });
    }

    //

    private void OnInit()
    {
        m_btnMediate.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.SetActive(false);
        m_runeStoneSupply.SetActive(false);
    }

    private void OnInitPlayer(PlayerData[] Player)
    {
        for (int i = 0; i < m_playerContent.transform.childCount; i++)
        {
            if (i >= Player.Length)
            {
                m_playerContent.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }
            if (Player[i].Base)
            {
                m_playerBase = Player[i].Player;
                m_tmpRuneStone.text = Player[i].Player.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
            }
        }
    }


    private void OnPlayerStart(IPlayer Player, Action OnComplete)
    {
        for (int i = 0; i < m_playerContent.transform.childCount; i++)
        {
            if (Player.Index != i)
                continue;
            m_playerContent.transform.GetChild(i).DOScale(Vector2.one * 1.2f, 0.2f).OnComplete(() =>
            {
                m_playerContent.transform.GetChild(i).DOScale(Vector2.one, 0.2f).OnComplete(() =>
                {
                    OnComplete?.Invoke();
                });
            });
            break;
        }
    }

    private void OnPlayerTakeRuneStoneFromSupply(IPlayer Player, int Value, Action OnComplete)
    {
        RectTransform RuneStone = Instantiate(m_runeStoneSupply, this.transform).GetComponent<RectTransform>();
        RuneStone.gameObject.SetActive(true);
        RuneStone.transform.Find("fx-glow").DORotate(Vector3.forward * 359f, 1.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        RuneStone.anchorMax = Vector3.one * 0.5f;
        RuneStone.anchorMin = Vector3.one * 0.5f;
        RuneStone.DOAnchorPos(Vector3.zero, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            RuneStone.DOMove(m_runeStoneShow.transform.position, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                m_runeStoneShow.GetComponentInChildren<TextMeshProUGUI>().text = Player.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
                Destroy(RuneStone.gameObject, 0.2f);
                OnComplete?.Invoke();
            });
        });
    }

    private void OnPlayerTakeRuneStoneFromMediation(IPlayer Player, Action OnComplete)
    {
        OnComplete?.Invoke();
    }

    private void OnPlayerDoChoice(IPlayer Player, Action OnComplete)
    {
        m_btnMediate.SetActive(true);
        m_btnCollect.SetActive(true);
        OnComplete?.Invoke();
    }
}