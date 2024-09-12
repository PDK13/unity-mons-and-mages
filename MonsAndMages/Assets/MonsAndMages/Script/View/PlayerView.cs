using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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

    private ViewType m_viewType = ViewType.None;

    private IPlayer m_playerBase;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GameEvent.onInit += OnInit;
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onView += OnView;
        GameEvent.onViewUI += OnViewUI;

        GameEvent.onPlayerStart += OnPlayerStart;
        GameEvent.onPlayerTakeRuneStoneFromSupply += OnPlayerTakeRuneStoneFromSupply;
        GameEvent.onPlayerTakeRuneStoneFromMediation += OnPlayerTakeRuneStoneFromMediation;
        GameEvent.onPlayerStunnedCheck += OnPlayerStunnedCheck;

        GameEvent.onPlayerDoChoice += OnPlayerDoChoice;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;
        GameEvent.onInitPlayer -= OnInitPlayer;

        GameEvent.onView -= OnView;
        GameEvent.onViewUI -= OnViewUI;

        GameEvent.onPlayerStart -= OnPlayerStart;
        GameEvent.onPlayerTakeRuneStoneFromSupply -= OnPlayerTakeRuneStoneFromSupply;
        GameEvent.onPlayerTakeRuneStoneFromMediation -= OnPlayerTakeRuneStoneFromMediation;
        GameEvent.onPlayerStunnedCheck -= OnPlayerStunnedCheck;

        GameEvent.onPlayerDoChoice -= OnPlayerDoChoice;
    }

    //

    public void BtnViewPlayer(int PlayerIndex)
    {
        GameEvent.ViewPlayer(GameManager.instance.GetPlayer(PlayerIndex), () =>
        {
            OnViewUI(true);
        });
        OnViewUI(false);
    }

    public void BtnViewMediate()
    {

    }

    public void BtnViewCollect()
    {
        GameEvent.View(ViewType.Wild, () =>
        {
            OnViewUI(true);
        });
        OnViewUI(false);
    }

    public void BtnViewBack()
    {
        GameEvent.View(ViewType.Field, () =>
        {
            OnViewUI(true);
        });
        OnViewUI(false);
    }

    //

    private void OnInit()
    {
        OnViewUI(false);

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

            m_playerContent.transform.GetChild(i).gameObject.SetActive(true);

            if (!Player[i].Base)
                continue;

            m_playerBase = Player[i].Player;
            m_tmpRuneStone.text = Player[i].Player.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
        }
    }


    private void OnView(ViewType Type, Action OnComplete)
    {
        m_viewType = Type;
    }

    private void OnViewUI(bool Show)
    {
        if (!Show)
        {
            m_btnMediate.SetActive(false);
            m_btnCollect.SetActive(false);
            m_btnBack.SetActive(false);
            m_playerContent.SetActive(false);
            return;
        }
        bool Field = m_viewType == ViewType.Field;
        bool Wild = m_viewType == ViewType.Wild;
        bool Base = GameManager.instance.PlayerCurrent.Base;
        bool Choice = GameManager.instance.PlayerChoice;
        m_btnMediate.SetActive(Show && Choice && Field && Base);
        m_btnCollect.SetActive(Show && Choice && Field && Base);
        m_btnBack.SetActive(Show && Wild);
        m_playerContent.SetActive(Show && Field && Base);
    }


    private void OnPlayerStart(IPlayer Player, Action OnComplete)
    {
        m_playerContent.transform.GetChild(Player.Index).DOScale(Vector2.one * 1.2f, 0.2f).OnComplete(() =>
        {
            m_playerContent.transform.GetChild(Player.Index).DOScale(Vector2.one, 0.2f).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        });
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
        //...
    }

    private void OnPlayerStunnedCheck(IPlayer Player, Action OnComplete)
    {
        m_playerContent.transform.GetChild(Player.Index).Find("stun").DOScale(Vector2.one * 1.2f, 0.2f).OnComplete(() =>
        {
            m_playerContent.transform.GetChild(Player.Index).Find("stun").DOScale(Vector2.one, 0.2f).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        });
    }


    private void OnPlayerDoChoice(IPlayer Player, Action OnComplete)
    {
        m_btnMediate.SetActive(true);
        m_btnCollect.SetActive(true);
        OnComplete?.Invoke();
    }
}