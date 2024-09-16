using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

    [Space]
    [SerializeField] private Transform m_playerContent;

    [Space]
    [SerializeField] private Transform m_infoView;
    [SerializeField] private CanvasGroup m_infoMask;
    [SerializeField] private GameObject m_btnInfoAccept;
    [SerializeField] private GameObject m_btnInfoCancel;

    [Space]
    [SerializeField] private GameObject m_runeStoneShow;
    [SerializeField] private GameObject m_runeStoneSupply;

    [Space]
    [SerializeField] private TextMeshProUGUI m_tmpRuneStone;

    private IPlayer m_playerBase;
    private ViewType m_viewType = ViewType.None;
    private ICard m_cardView;

    public Transform InfoView => m_infoView;

    //

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
        GameEvent.OnPlayerTakeRuneStoneFromMediation += OnPlayerTakeRuneStoneFromMediation;
        GameEvent.onPlayerDoChoice += OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate += OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect += OnPlayerDoCollect;
        GameEvent.onPlayerEnd += OnPlayerEnd;

        GameEvent.onCardTap += OnCardTap;
        GameEvent.onCardInfo += OnViewInfo;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onView -= OnView;
        GameEvent.onViewUI -= OnViewUI;

        GameEvent.onPlayerStart -= OnPlayerStart;
        GameEvent.onPlayerTakeRuneStoneFromSupply -= OnPlayerTakeRuneStoneFromSupply;
        GameEvent.OnPlayerTakeRuneStoneFromMediation -= OnPlayerTakeRuneStoneFromMediation;
        GameEvent.onPlayerDoChoice -= OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate -= OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect -= OnPlayerDoCollect;
        GameEvent.onPlayerEnd -= OnPlayerEnd;

        GameEvent.onCardTap -= OnCardTap;
        GameEvent.onCardInfo -= OnViewInfo;
    }

    private void Start()
    {
        m_btnMediate.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);

        m_playerContent.gameObject.SetActive(false);

        m_infoMask.gameObject.SetActive(false);
        m_btnInfoAccept.SetActive(false);
        m_btnInfoCancel.SetActive(false);

        m_runeStoneSupply.SetActive(false);
        m_runeStoneShow.SetActive(false);
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

    public void BtnCollectAccept()
    {
        GameManager.instance.PlayerDoCollect(m_playerBase, m_cardView);
        m_cardView = null;
    } //Player Collect Card

    public void BtnCollectCancel()
    {
        GameEvent.CardViewInfo(InfoType.CardCollect, false);
        m_cardView.MoveBack(1f, null);
        m_cardView = null;
    }

    //

    private void OnInit()
    {
        //...
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

            var ViewButton = m_playerContent.transform.GetChild(i).GetComponent<PlayerViewButton>();
            ViewButton.Base = Player[i].Base;
            ViewButton.Health = Player[i].HealthCurrent;
            ViewButton.Stun = Player[i].StunCurrent;

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
            m_runeStoneShow.SetActive(false);
            m_btnMediate.SetActive(false);
            m_btnCollect.SetActive(false);
            m_btnBack.SetActive(false);
            m_playerContent.gameObject.SetActive(false);
            return;
        }
        bool Field = m_viewType == ViewType.Field;
        bool Wild = m_viewType == ViewType.Wild;
        bool Base = GameManager.instance.PlayerCurrent.Base;
        bool Choice = GameManager.instance.PlayerChoice;
        m_runeStoneShow.SetActive(Show && Base);
        m_btnMediate.SetActive(Show && Choice && Field && Base);
        m_btnCollect.SetActive(Show && Choice && Field && Base);
        m_btnBack.SetActive(Show && Wild);
        m_playerContent.gameObject.SetActive(Show && Field && Base);
    }

    private void OnViewInfo(InfoType Type, bool Show)
    {
        m_infoMask.gameObject.SetActive(true);
        if (Show)
        {
            m_infoMask.alpha = 0;
            m_infoMask
                .DOFade(1f, 1f)
                .SetEase(Ease.Linear);
        }
        else
        {
            m_infoMask.alpha = 1;
            m_infoMask
                .DOFade(0f, 1f)
                .SetEase(Ease.Linear)
                .OnComplete(() => m_infoMask.gameObject.SetActive(false));
        }

        m_btnInfoAccept.SetActive(false);
        m_btnInfoCancel.SetActive(false);
        switch (Type)
        {
            case InfoType.CardCollect:
                m_btnInfoAccept.SetActive(Show);
                m_btnInfoCancel.SetActive(Show);
                break;
        }
    }


    private void OnPlayerStart(IPlayer Player, Action OnComplete)
    {
        m_playerContent.transform.GetChild(Player.Index).DOScale(Vector2.one * 1.2f, 0.2f).OnComplete(() =>
        {
            OnComplete?.Invoke();

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

    private void OnPlayerTakeRuneStoneFromMediation(IPlayer Player, int Value, Action OnComplete)
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


    private void OnCardTap(ICard Card)
    {
        if (m_cardView != null)
            return;
        m_cardView = Card;
    }


    private void OnPlayerDoMediate(IPlayer Player, int Value, Action OnComplete)
    {
        //...
    }

    private void OnPlayerDoCollect(IPlayer Player, ICard Card, Action OnComplete)
    {
        GameEvent.CardViewInfo(InfoType.CardCollect, false);
        GameEvent.ViewUi(false);

        Card.Renderer.maskable = false;
        var Point = Player.DoCollectReady().transform;
        GameEvent.View(ViewType.Field, () =>
        {
            Card.Point(Point);
            Card.MoveBack(1f, () =>
            {
                Card.Rumble(() =>
                {
                    Card.Renderer.maskable = true;
                    OnComplete?.Invoke();
                });
            });
        });
    }


    private void OnPlayerEnd(IPlayer Player, Action OnComplete)
    {
        m_playerContent.transform.GetChild(Player.Index).DOScale(Vector2.one, 0.2f).OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }
}