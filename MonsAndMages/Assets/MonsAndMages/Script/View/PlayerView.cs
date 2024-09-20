using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private ICard m_cardView;

    public Transform InfoView => m_infoView;

    public IPlayer PlayerCurrent => GameManager.instance.PlayerCurrent;

    //

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GameEvent.onInit += OnInit;
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onViewUIHide += OnViewUIHide;
        GameEvent.onViewUIShow += OnViewUIShow;
        GameEvent.onViewPlayer += OnViewPlayer;
        GameEvent.onViewInfo += OnViewInfo;

        GameEvent.onPlayerStart += OnPlayerStart;
        GameEvent.onPlayerStunnedCheck += OnPlayerStunnedCheck;
        GameEvent.onPlayerDoChoice += OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate += OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect += OnPlayerDoCollect;
        GameEvent.onPlayerEnd += OnPlayerEnd;
        GameEvent.onPlayerHealthChange += OnPlayerHealthChange;
        GameEvent.onPlayerStunnedChange += OnPlayerStunnedChange;

        GameEvent.onCardTap += OnCardTap;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onViewUIHide -= OnViewUIHide;
        GameEvent.onViewUIShow -= OnViewUIShow;
        GameEvent.onViewPlayer -= OnViewPlayer;
        GameEvent.onViewInfo -= OnViewInfo;

        GameEvent.onPlayerStart -= OnPlayerStart;
        GameEvent.onPlayerStunnedCheck -= OnPlayerStunnedCheck;
        GameEvent.onPlayerDoChoice -= OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate -= OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect -= OnPlayerDoCollect;
        GameEvent.onPlayerEnd -= OnPlayerEnd;
        GameEvent.onPlayerHealthChange -= OnPlayerHealthChange;
        GameEvent.onPlayerStunnedChange -= OnPlayerStunnedChange;

        GameEvent.onCardTap -= OnCardTap;
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
    }

    //

    public void BtnViewPlayer(int PlayerIndex)
    {
        if (!GameManager.instance.PlayerChoice)
            return;

        GameEvent.ViewPlayer(GameManager.instance.GetPlayer(PlayerIndex), null);
    }

    public void BtnViewMediate()
    {

    }

    public void BtnViewCollect()
    {
        if (!GameManager.instance.PlayerChoice)
            return;

        GameEvent.View(ViewType.Wild, null);
    }

    public void BtnViewBack()
    {
        if (!GameManager.instance.PlayerChoice)
            return;

        GameEvent.View(ViewType.Field, null);
    }

    public void BtnCollectAccept()
    {
        if (!GameManager.instance.PlayerChoice)
            return;

        GameEvent.ViewUiHide();
        GameManager.instance.PlayerDoCollect(GameManager.instance.PlayerCurrent, m_cardView);
        m_cardView = null;
    } //Player Collect Card

    public void BtnCollectCancel()
    {
        if (!GameManager.instance.PlayerChoice)
            return;

        GameEvent.ViewUiHide();
        GameEvent.ViewInfo(InfoType.CardCollect, false);
        m_cardView.MoveBack(1f, () => GameEvent.ViewUiShow(ViewType.Wild));
        m_cardView = null;
    }

    //

    private void OnInit()
    {
        //...
    }

    private void OnInitPlayer(PlayerData[] Player)
    {
        GameEvent.ViewUiShow(ViewType.Field);

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
        }
    }


    private void OnViewUIHide()
    {
        m_btnMediate.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.gameObject.SetActive(false);
    }

    private void OnViewUIShow(ViewType Type)
    {
        bool Choice = GameManager.instance.PlayerChoice;

        switch (Type)
        {
            case ViewType.Field:
                m_btnMediate.GetComponent<Button>().interactable = PlayerCurrent.MediationEmty;
                m_btnMediate.SetActive(Choice);
                m_btnCollect.SetActive(Choice);
                m_btnBack.SetActive(false);
                m_playerContent.gameObject.SetActive(true);
                break;
            case ViewType.Wild:
                m_btnMediate.SetActive(false);
                m_btnCollect.SetActive(false);
                m_btnBack.SetActive(Choice);
                m_playerContent.gameObject.SetActive(false);
                break;
        }
    }

    private void OnViewPlayer(IPlayer Player, Action OnComplete)
    {
        for (int i = 0; i < m_playerContent.childCount; i++)
        {
            var PlayerButton = m_playerContent.GetChild(i);

            if (!PlayerButton.gameObject.activeInHierarchy)
                continue;

            if (i == Player.Index)
            {
                PlayerButton.GetComponent<Outline>().effectColor = Color.green;
            }
            else
            {
                PlayerButton.GetComponent<Outline>().effectColor = Color.black;
            }
        }
        //m_tmpRuneStone.text = Player.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
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
                m_btnInfoAccept.GetComponent<Button>().interactable = Show && m_cardView.RuneStoneCost <= PlayerCurrent.RuneStone;
                m_btnInfoAccept.SetActive(Show);
                m_btnInfoCancel.SetActive(Show);
                break;
        }
    }


    private void OnPlayerStart(IPlayer Player, Action OnComplete)
    {
        var PlayerView = m_playerContent.transform.GetChild(GameManager.instance.PlayerIndex);
        PlayerView.DOScale(Vector2.one * 1.2f, 0.2f).OnComplete(() =>
        {
            //PlayerView.DOScale(Vector2.one, 0.2f).OnComplete(() =>
            //{
            OnComplete?.Invoke();
            //});
        });
    }

    private void OnPlayerStunnedCheck(IPlayer Player, Action OnComplete)
    {
        var PlayerCurrent = m_playerContent.transform.GetChild(GameManager.instance.PlayerIndex);
        var PlayerStun = PlayerCurrent.Find("stun");
        PlayerStun.DOScale(Vector2.one * 1.2f, 0.2f).OnComplete(() =>
        {
            PlayerStun.DOScale(Vector2.one, 0.2f).OnComplete(() =>
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

    private void OnPlayerDoMediate(IPlayer Player, int Value, Action OnComplete)
    {
        //...
    }

    private void OnPlayerDoCollect(IPlayer Player, ICard Card, Action OnComplete)
    {
        GameEvent.ViewInfo(InfoType.CardCollect, false);

        Card.Renderer.maskable = false;
        var Point = Player.DoCollectReady().transform;
        GameEvent.View(ViewType.Field, () =>
        {
            GameEvent.WildCardFill(null);
            GameEvent.ViewPlayer(PlayerCurrent, () =>
            {
                Card.Pointer(Point);
                Card.MoveBack(1f, () =>
                {
                    Card.Rumble(() =>
                    {
                        Card.Renderer.maskable = true;
                        OnComplete?.Invoke();
                    });
                });
            });
        });
    }

    private void OnPlayerEnd(IPlayer Player, Action OnComplete)
    {
        var PlayerView = m_playerContent.transform.GetChild(GameManager.instance.PlayerIndex);
        PlayerView.DOScale(Vector2.one, 0.05f).OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }


    private void OnPlayerHealthChange(IPlayer Player, int Value, Action OnComplete)
    {
        var PlayerCurrent = m_playerContent.transform.GetChild(Player.Index);
        var PlayerHealth = PlayerCurrent.Find("health");
        var PlayerHealthTmp = PlayerHealth.Find("tmp-health").GetComponent<TextMeshProUGUI>();
        PlayerHealth.DOScale(Vector2.one * 1.2f, 0.1f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            PlayerHealthTmp.text = Player.HealthCurrent.ToString();
            PlayerHealth.DOScale(Vector2.one, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        });
    }

    private void OnPlayerStunnedChange(IPlayer Player, int Value, Action OnComplete)
    {
        var PlayerCurrent = m_playerContent.transform.GetChild(Player.Index);
        var PlayerStun = PlayerCurrent.Find("stun");
        var PlayerStunTmp = PlayerStun.Find("tmp-stun").GetComponent<TextMeshProUGUI>();
        PlayerStun.DOScale(Vector2.one * 1.2f, 0.1f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            PlayerStunTmp.text = Player.HealthCurrent.ToString();
            PlayerStun.DOScale(Vector2.one, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        });
    }


    private void OnCardTap(ICard Card, Action OnComplete)
    {
        if (m_cardView != null)
            return;
        m_cardView = Card;
        OnComplete?.Invoke();
    }
}