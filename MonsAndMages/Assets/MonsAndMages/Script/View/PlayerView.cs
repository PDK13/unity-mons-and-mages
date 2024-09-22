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
    [SerializeField] private Transform m_mediateOptionContent;

    [Space]
    [SerializeField] private Transform m_infoView;
    [SerializeField] private CanvasGroup m_infoMask;
    [SerializeField] private GameObject m_btnInfoAccept;
    [SerializeField] private GameObject m_btnInfoCancel;

    [Space]
    [SerializeField] private GameObject m_warnMediate;
    [SerializeField] private GameObject m_warnInfoAccept;
    [SerializeField] private GameObject m_warnFullMana;

    [Space]
    [SerializeField] private TextMeshProUGUI m_tmpExplainOrigin;
    [SerializeField] private TextMeshProUGUI m_tmpExplainClass;

    [Space]
    [SerializeField] private RectTransform m_runeStoneBox;
    [SerializeField] private TextMeshProUGUI m_tmpRuneStone;

    private InfoType m_infoType;
    private ICard m_cardView;
    private int m_mediateOptionIndex = -1;

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

        GameEvent.onViewUI += OnViewUI;
        GameEvent.onViewPlayer += OnViewPlayer;
        GameEvent.onViewInfo += OnViewInfo;

        GameEvent.onPlayerStart += OnPlayerStart;
        GameEvent.onPlayerStunnedCheck += OnPlayerStunnedCheck;
        GameEvent.onPlayerDoChoice += OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate += OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect += OnPlayerDoCollect;
        GameEvent.onPlayerEnd += OnPlayerEnd;
        GameEvent.onPlayerRuneStoneChange += OnPlayerRuneStoneChange;
        GameEvent.onPlayerHealthChange += OnPlayerHealthChange;
        GameEvent.onPlayerStunnedChange += OnPlayerStunnedChange;

        GameEvent.onCardTap += OnCardTap;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onViewUI -= OnViewUI;
        GameEvent.onViewPlayer -= OnViewPlayer;
        GameEvent.onViewInfo -= OnViewInfo;

        GameEvent.onPlayerStart -= OnPlayerStart;
        GameEvent.onPlayerStunnedCheck -= OnPlayerStunnedCheck;
        GameEvent.onPlayerDoChoice -= OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate -= OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect -= OnPlayerDoCollect;
        GameEvent.onPlayerEnd -= OnPlayerEnd;
        GameEvent.onPlayerRuneStoneChange -= OnPlayerRuneStoneChange;
        GameEvent.onPlayerHealthChange -= OnPlayerHealthChange;
        GameEvent.onPlayerStunnedChange -= OnPlayerStunnedChange;

        GameEvent.onCardTap -= OnCardTap;
    }

    private void Start()
    {
        m_btnMediate.SetActive(false);
        m_warnMediate.gameObject.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);

        m_infoMask.gameObject.SetActive(false);
        m_btnInfoAccept.SetActive(false);
        m_btnInfoCancel.SetActive(false);
        m_mediateOptionContent.gameObject.SetActive(false);
        m_warnInfoAccept.SetActive(false);
        m_warnFullMana.SetActive(false);
        m_tmpExplainOrigin.gameObject.SetActive(false);
        m_tmpExplainClass.gameObject.SetActive(false);

        m_playerContent.gameObject.SetActive(false);
        m_runeStoneBox.gameObject.SetActive(false);
    }

    //

    public void BtnViewMediate()
    {
        if (GameManager.instance.PlayerChoice != ChoiceType.Main)
            return;
        m_infoType = InfoType.Mediate;

        for (int i = 0; i < m_mediateOptionContent.childCount; i++)
        {
            var Outline = m_mediateOptionContent.GetChild(i).GetComponent<Outline>();
            Outline.effectColor = Color.black;
            var Button = m_mediateOptionContent.GetChild(i).GetComponent<Button>();
            Button.interactable = GameManager.instance.PlayerCurrent.RuneStone >= i + 1;
        }

        GameEvent.ViewInfo(InfoType.Mediate, true);
    }

    public void BtnViewCollect()
    {
        if (GameManager.instance.PlayerChoice != ChoiceType.Main)
            return;
        m_infoType = InfoType.Collect;
        GameEvent.View(ViewType.Wild, null);
    }

    public void BtnViewBack()
    {
        if (GameManager.instance.PlayerChoice != ChoiceType.Main)
            return;
        GameEvent.View(ViewType.Field, null);
    }

    public void BtnCollectAccept()
    {
        switch (GameManager.instance.PlayerChoice)
        {
            case ChoiceType.Main:
                switch (m_infoType)
                {
                    case InfoType.Mediate:
                        GameManager.instance.PlayerDoMediate(GameManager.instance.PlayerCurrent, m_mediateOptionIndex + 1);
                        GameEvent.ViewUiShow(ViewType.Field, true);
                        GameEvent.ViewInfo(InfoType.Mediate, false);
                        break;
                    case InfoType.Collect:
                        GameManager.instance.PlayerDoCollect(GameManager.instance.PlayerCurrent, m_cardView);
                        m_cardView = null;
                        GameEvent.ViewUiShow(ViewType.Wild, false);
                        m_runeStoneBox.gameObject.SetActive(true);
                        GameEvent.ViewInfo(InfoType.Collect, false);
                        break;
                }
                break;
            case ChoiceType.CardFullMana:
                //...
                break;
        }
    }

    public void BtnCollectCancel()
    {
        switch (GameManager.instance.PlayerChoice)
        {
            case ChoiceType.Main:
                switch (m_infoType)
                {
                    case InfoType.Mediate:
                        GameEvent.ViewUiShow(ViewType.Field, true);
                        GameEvent.ViewInfo(InfoType.Mediate, false);
                        break;
                    case InfoType.Collect:
                        GameEvent.ViewUiShow(ViewType.Wild, false);
                        GameEvent.ViewInfo(InfoType.Collect, false);
                        m_cardView.MoveBack(() => GameEvent.ViewUiShow(ViewType.Wild, true));
                        m_cardView = null;
                        break;
                }
                break;
            case ChoiceType.CardFullMana:
                //...
                break;
        }


    }

    public void BtnMediateOption(int OptionIndex)
    {
        m_mediateOptionIndex = OptionIndex;
        for (int i = 0; i < m_mediateOptionContent.childCount; i++)
        {
            var Outline = m_mediateOptionContent.GetChild(i).GetComponent<Outline>();
            Outline.effectColor = OptionIndex == i ? Color.red : Color.black;
        }
        m_btnInfoAccept.SetActive(true);
    }

    public void BtnViewPlayer(int PlayerIndex)
    {
        if (GameManager.instance.PlayerChoice == ChoiceType.None)
            return;

        GameEvent.ViewPlayer(GameManager.instance.GetPlayer(PlayerIndex), null);
    }

    //

    private void OnInit()
    {
        //...
    }

    private void OnInitPlayer(PlayerData[] Player)
    {
        GameEvent.ViewUiShow(ViewType.Field, true);

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


    private void OnViewUI(ViewType Type, bool Show)
    {
        if (!GameManager.instance.Battle || !Show)
        {
            m_btnMediate.SetActive(false);
            m_btnCollect.SetActive(false);
            m_btnBack.SetActive(false);
            m_playerContent.gameObject.SetActive(Type == ViewType.Field);
            m_warnMediate.SetActive(false);
            m_warnFullMana.SetActive(false);
            m_runeStoneBox.gameObject.SetActive(false);
            return;
        }

        switch (GameManager.instance.PlayerChoice)
        {
            case ChoiceType.Main:
                switch (Type)
                {
                    case ViewType.Field:
                        var MediateAvaible = PlayerCurrent.MediationEmty;
                        m_btnMediate.GetComponent<Button>().interactable = MediateAvaible;
                        m_btnMediate.SetActive(true);
                        m_btnCollect.SetActive(true);
                        m_btnBack.SetActive(false);
                        m_playerContent.gameObject.SetActive(true);
                        m_warnMediate.SetActive(!MediateAvaible);
                        m_warnFullMana.SetActive(false);
                        m_runeStoneBox.gameObject.SetActive(false);
                        break;
                    case ViewType.Wild:
                        m_btnMediate.SetActive(false);
                        m_btnCollect.SetActive(false);
                        m_btnBack.SetActive(true);
                        m_playerContent.gameObject.SetActive(false);
                        m_warnMediate.SetActive(false);
                        m_warnFullMana.SetActive(false);
                        m_runeStoneBox.gameObject.SetActive(true);
                        break;
                }
                break;
            case ChoiceType.CardFullMana:
                switch (Type)
                {
                    case ViewType.Field:
                        m_btnMediate.SetActive(false);
                        m_btnCollect.SetActive(false);
                        m_btnBack.SetActive(false);
                        m_playerContent.gameObject.SetActive(true);
                        m_warnMediate.SetActive(false);
                        m_warnFullMana.SetActive(true);
                        m_runeStoneBox.gameObject.SetActive(false);
                        break;
                    case ViewType.Wild:
                        m_btnMediate.SetActive(false);
                        m_btnCollect.SetActive(false);
                        m_btnBack.SetActive(true);
                        m_playerContent.gameObject.SetActive(false);
                        m_warnMediate.SetActive(false);
                        m_warnFullMana.SetActive(false);
                        m_runeStoneBox.gameObject.SetActive(false);
                        break;
                }
                break;
            default:
                m_btnMediate.SetActive(false);
                m_btnCollect.SetActive(false);
                m_btnBack.SetActive(false);
                m_playerContent.gameObject.SetActive(Type == ViewType.Field);
                m_warnMediate.SetActive(false);
                m_warnFullMana.SetActive(false);
                m_runeStoneBox.gameObject.SetActive(false);
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

            if (i != Player.Index)
                PlayerButton.GetComponent<Outline>().effectColor = Color.black;
            else
            if (PlayerCurrent.Index == Player.Index)
                PlayerButton.GetComponent<Outline>().effectColor = Color.green;
            else
                PlayerButton.GetComponent<Outline>().effectColor = Color.gray;
        }
        //m_tmpRuneStone.text = Player.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
    }

    private void OnViewInfo(InfoType Type, bool Show)
    {
        if (!GameManager.instance.Battle)
        {
            m_infoMask.gameObject.SetActive(false);
            m_btnInfoAccept.SetActive(false);
            m_btnInfoCancel.SetActive(false);
            m_mediateOptionContent.gameObject.SetActive(false);
            m_warnInfoAccept.SetActive(false);
            m_tmpExplainOrigin.gameObject.SetActive(false);
            m_tmpExplainClass.gameObject.SetActive(false);
            return;
        }

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration + 0.02f;

        m_infoMask.DOKill();
        m_infoMask.gameObject.SetActive(true);
        if (Show)
        {
            m_infoMask.alpha = 0;
            m_infoMask
                .DOFade(1f, MoveDuration)
                .SetEase(Ease.Linear);
        }
        else
        {
            m_infoMask.alpha = 1;
            m_infoMask
                .DOFade(0f, MoveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => m_infoMask.gameObject.SetActive(false));
        }

        switch (Type)
        {
            case InfoType.Mediate:
                m_btnInfoAccept.GetComponent<Button>().interactable = true;
                m_btnInfoAccept.SetActive(false);
                m_btnInfoCancel.SetActive(Show);
                m_mediateOptionContent.gameObject.SetActive(Show);
                m_warnInfoAccept.SetActive(false);
                m_tmpExplainOrigin.gameObject.SetActive(false);
                m_tmpExplainClass.gameObject.SetActive(false);
                break;
            case InfoType.Collect:
                if (Show && m_cardView != null)
                {
                    var CollectAvaible = m_cardView.RuneStoneCost <= PlayerCurrent.RuneStone;
                    m_btnInfoAccept.GetComponent<Button>().interactable = CollectAvaible;
                    m_btnInfoAccept.SetActive(Show);
                    m_warnInfoAccept.SetActive(Show && !CollectAvaible);
                }
                else
                {
                    m_infoMask.gameObject.SetActive(false);
                    m_btnInfoAccept.SetActive(false);
                    m_warnInfoAccept.SetActive(false);
                }
                m_btnInfoCancel.SetActive(Show);
                m_mediateOptionContent.gameObject.SetActive(false);
                if (Show && m_cardView != null)
                {
                    m_tmpExplainOrigin.text = GameManager.instance.ExplainConfig.GetExplainOrigin(m_cardView.Origin);
                    m_tmpExplainClass.text = GameManager.instance.ExplainConfig.GetExplainClass(m_cardView.Class);
                }
                m_tmpExplainOrigin.gameObject.SetActive(Show);
                m_tmpExplainClass.gameObject.SetActive(Show);
                break;
            case InfoType.CardFullMana:
                m_btnInfoAccept.GetComponent<Button>().interactable = true;
                m_btnInfoAccept.SetActive(Show);
                m_btnInfoCancel.SetActive(Show);
                m_mediateOptionContent.gameObject.SetActive(false);
                m_warnInfoAccept.SetActive(false);
                m_tmpExplainOrigin.gameObject.SetActive(false);
                m_tmpExplainClass.gameObject.SetActive(false);
                break;
            default:
                m_infoMask.gameObject.SetActive(false);
                m_btnInfoAccept.SetActive(false);
                m_btnInfoCancel.SetActive(false);
                m_mediateOptionContent.gameObject.SetActive(false);
                m_warnInfoAccept.SetActive(false);
                m_tmpExplainOrigin.gameObject.SetActive(false);
                m_tmpExplainClass.gameObject.SetActive(false);
                break;
        }
    }


    private void OnPlayerStart(IPlayer Player, Action OnComplete)
    {
        var PlayerView = m_playerContent.transform.GetChild(GameManager.instance.PlayerIndex);
        PlayerView.DOScale(Vector2.one * 1.2f, 0.2f).OnComplete(() =>
        {
            PlayerView.DOScale(Vector2.one, 0.2f).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        });

        m_tmpRuneStone.text = GameManager.instance.PlayerCurrent.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
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
        m_btnMediate.GetComponent<Button>().interactable = PlayerCurrent.MediationEmty;
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
        GameEvent.ViewInfo(InfoType.Collect, false);

        Card.Renderer.maskable = false;
        var Point = Player.DoCollectReady().transform;
        GameEvent.View(ViewType.Field, () =>
        {
            GameEvent.WildCardFill(null);
            GameEvent.ViewPlayer(PlayerCurrent, () =>
            {
                Card.Pointer(Point);
                Card.MoveBack(() =>
                {
                    Card.Rumble(() =>
                    {
                        Card.Renderer.maskable = true;
                        Card.InfoShow(true);
                        OnComplete?.Invoke();
                    });
                });
            });
        });
    }

    private void OnPlayerEnd(IPlayer Player, Action OnComplete)
    {
        var PlayerView = m_playerContent.transform.GetChild(GameManager.instance.PlayerIndex);
        PlayerView.DOScale(Vector2.one * 1.2f, 0.2f).OnComplete(() =>
        {
            PlayerView.DOScale(Vector2.one, 0.2f).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        });
    }

    private void OnPlayerRuneStoneChange(IPlayer Player, int Value, Action OnComplete)
    {
        m_runeStoneBox.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            m_tmpRuneStone.text = Player.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
            m_runeStoneBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
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
            PlayerStunTmp.text = Player.StunCurrent.ToString();
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