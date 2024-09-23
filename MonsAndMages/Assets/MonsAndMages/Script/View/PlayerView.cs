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
    [SerializeField] private GameObject m_hintChoiceMediate;
    [SerializeField] private GameObject m_hintCollectAccept;
    [SerializeField] private GameObject m_hintManaActive;
    [SerializeField] private GameObject m_hintOriginGhost;
    [SerializeField] private GameObject m_hintClassMagicAddict;

    [Space]
    [SerializeField] private TextMeshProUGUI m_tmpExplainOrigin;
    [SerializeField] private TextMeshProUGUI m_tmpExplainClass;

    [Space]
    [SerializeField] private RectTransform m_runeStoneBox;
    [SerializeField] private TextMeshProUGUI m_tmpRuneStone;

    [Space]
    [SerializeField] private RectTransform m_diceSample;

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
        //Init
        GameEvent.onInit += OnInit;
        GameEvent.onInitPlayer += OnInitPlayer;
        //View
        GameEvent.onViewPlayer += OnViewPlayer;
        //Ui-Choice
        GameEvent.onUiChoiceHide += OnUiChoiceHide;
        GameEvent.onUiChoiceCurrent += OnUiChoiceCurrent;
        GameEvent.onUiChoiceMediateOrCollect += OnUiChoiceMediateOrCollect;
        GameEvent.onUiChoiceCardFullMana += OnUiChoiceCardFullMana;
        GameEvent.onUiChoiceCardOriginGhost += OnUiChoiceCardOriginGhost;
        GameEvent.onUiChoiceCardClassMagicAddict += OnUiChoiceCardClassMagicAddict;
        //Ui-Info
        GameEvent.onUiInfoHide += OnUiInfoHide;
        GameEvent.onUiInfoCollect += OnUiInfoCollect;
        GameEvent.onUiInfoMediate += OnUiInfoMediate;
        GameEvent.onUiInfoFullMana += OnUiInfoFullMana;
        GameEvent.onUiInfoOriginGhost += OnUiInfoOriginGhost;
        GameEvent.onUiInfoClassMagicAddict += OnUiInfoClassMagicAddict;
        //Player
        GameEvent.onPlayerStart += OnPlayerStart;
        GameEvent.onPlayerStunnedCheck += OnPlayerStunnedCheck;
        GameEvent.onPlayerDoChoice += OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate += OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect += OnPlayerDoCollect;
        GameEvent.onCardManaActive += OnCardManaActive;
        GameEvent.onPlayerEnd += OnPlayerEnd;
        GameEvent.onPlayerRuneStoneChange += OnPlayerRuneStoneChange;
        GameEvent.onPlayerHealthChange += OnPlayerHealthChange;
        GameEvent.onPlayerStunnedChange += OnPlayerStunnedChange;
        //Origin
        GameEvent.onOriginDragon += OnOriginDragon;
        GameEvent.onOriginGhost += OnOriginGhost;
        //Class
        GameEvent.onClassFighter += OnClassFighter;
        GameEvent.onClassMagicAddict += OnClassMagicAddict;
    }

    private void OnDisable()
    {
        //Init
        GameEvent.onInit -= OnInit;
        GameEvent.onInitPlayer -= OnInitPlayer;
        //View
        GameEvent.onViewPlayer -= OnViewPlayer;
        //Ui-Choice
        GameEvent.onUiChoiceHide -= OnUiChoiceHide;
        GameEvent.onUiChoiceCurrent -= OnUiChoiceCurrent;
        GameEvent.onUiChoiceMediateOrCollect -= OnUiChoiceMediateOrCollect;
        GameEvent.onUiChoiceCardFullMana -= OnUiChoiceCardFullMana;
        GameEvent.onUiChoiceCardOriginGhost -= OnUiChoiceCardOriginGhost;
        GameEvent.onUiChoiceCardClassMagicAddict -= OnUiChoiceCardClassMagicAddict;
        //Ui-Info
        GameEvent.onUiInfoHide -= OnUiInfoHide;
        GameEvent.onUiInfoCollect -= OnUiInfoCollect;
        GameEvent.onUiInfoMediate -= OnUiInfoMediate;
        GameEvent.onUiInfoFullMana -= OnUiInfoFullMana;
        GameEvent.onUiInfoOriginGhost -= OnUiInfoOriginGhost;
        GameEvent.onUiInfoClassMagicAddict -= OnUiInfoClassMagicAddict;
        //Player
        GameEvent.onPlayerStart -= OnPlayerStart;
        GameEvent.onPlayerStunnedCheck -= OnPlayerStunnedCheck;
        GameEvent.onPlayerDoChoice -= OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate -= OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect -= OnPlayerDoCollect;
        GameEvent.onCardManaActive -= OnCardManaActive;
        GameEvent.onPlayerEnd -= OnPlayerEnd;
        GameEvent.onPlayerRuneStoneChange -= OnPlayerRuneStoneChange;
        GameEvent.onPlayerHealthChange -= OnPlayerHealthChange;
        GameEvent.onPlayerStunnedChange -= OnPlayerStunnedChange;
        //Origin
        GameEvent.onOriginDragon -= OnOriginDragon;
        GameEvent.onOriginGhost -= OnOriginGhost;
        //Class
        GameEvent.onClassFighter -= OnClassFighter;
        GameEvent.onClassMagicAddict -= OnClassMagicAddict;
    }

    private void Start()
    {
        m_btnMediate.SetActive(false);
        m_hintChoiceMediate.gameObject.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);

        m_infoMask.gameObject.SetActive(false);
        m_btnInfoAccept.SetActive(false);
        m_btnInfoCancel.SetActive(false);
        m_mediateOptionContent.gameObject.SetActive(false);
        m_hintCollectAccept.SetActive(false);
        m_hintManaActive.SetActive(false);
        m_hintOriginGhost.SetActive(false);
        m_tmpExplainOrigin.gameObject.SetActive(false);
        m_tmpExplainClass.gameObject.SetActive(false);

        m_playerContent.gameObject.SetActive(false);
        m_runeStoneBox.gameObject.SetActive(false);
    }

    //Button

    public void BtnViewMediate()
    {
        if (GameManager.instance.PlayerChoice != ChoiceType.MediateOrCollect)
            return;

        for (int i = 0; i < m_mediateOptionContent.childCount; i++)
        {
            var Outline = m_mediateOptionContent.GetChild(i).GetComponent<Outline>();
            Outline.effectColor = Color.black;
            var Button = m_mediateOptionContent.GetChild(i).GetComponent<Button>();
            Button.interactable = GameManager.instance.PlayerCurrent.RuneStone >= i + 1;
        }

        GameEvent.UiInfoMediate();
    }

    public void BtnViewCollect()
    {
        if (GameManager.instance.PlayerChoice != ChoiceType.MediateOrCollect)
            return;
        GameEvent.ViewArea(ViewType.Wild, null);
    }

    public void BtnViewBack()
    {
        if (GameManager.instance.PlayerChoice != ChoiceType.MediateOrCollect)
            return;
        GameEvent.ViewArea(ViewType.Field, null);
    }

    public void BtnCollectAccept()
    {
        switch (GameManager.instance.PlayerChoice)
        {
            case ChoiceType.MediateOrCollect:
                switch (GameView.instance.ViewType)
                {
                    case ViewType.Field: //Mediate
                        GameManager.instance.PlayerDoMediateStart(GameManager.instance.PlayerCurrent, m_mediateOptionIndex + 1);
                        break;
                    case ViewType.Wild: //Collect
                        GameManager.instance.PlayerDoCollectStart(GameManager.instance.PlayerCurrent, m_cardView);
                        break;
                }
                break;
            case ChoiceType.CardFullMana:
                GameManager.instance.CardManaActiveStart(m_cardView);
                break;
            case ChoiceType.CardOriginGhost:
                GameManager.instance.CardOriginGhostDoStart(m_cardView);
                break;
            case ChoiceType.CardClassMagicAddict:
                GameManager.instance.CardClassMagicAddictStart(m_cardView);
                break;
        }
    }

    public void BtnCollectCancel()
    {
        switch (GameManager.instance.PlayerChoice)
        {
            case ChoiceType.MediateOrCollect:
                switch (GameView.instance.ViewType)
                {
                    case ViewType.Field: //Mediate
                        GameEvent.UiInfoHide(true, true);
                        break;
                    case ViewType.Wild: //Collect
                        GameEvent.UiInfoHide(true, true);
                        break;
                }
                break;
            case ChoiceType.CardFullMana:
            case ChoiceType.CardOriginGhost:
            case ChoiceType.CardClassMagicAddict:
                GameEvent.UiInfoHide(true, true);
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

    //GameEvent - Init

    private void OnInit()
    {
        //...
    }

    private void OnInitPlayer(PlayerData[] Player)
    {
        OnUiChoiceCurrent();

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

    //GameEvent - View

    private void OnViewCard(ICard Card)
    {

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
        //m_tmpRuneStone.text = PlayerQueue.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
    }

    //GameEvent - Ui Choice

    private void OnUiChoiceHide()
    {
        var Type = GameView.instance.ViewType;
        m_btnMediate.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_playerContent.gameObject.SetActive(Type == ViewType.Field);
        m_hintChoiceMediate.SetActive(false);
        m_hintManaActive.SetActive(false);
        m_hintOriginGhost.SetActive(false);
        m_hintClassMagicAddict.SetActive(false);
        m_runeStoneBox.gameObject.SetActive(Type == ViewType.Wild);
    }

    private void OnUiChoiceCurrent()
    {
        switch (GameManager.instance.PlayerChoice)
        {
            case ChoiceType.None:
                OnUiChoiceHide();
                break;
            case ChoiceType.MediateOrCollect:
                OnUiChoiceMediateOrCollect();
                break;
            case ChoiceType.CardFullMana:
                OnUiChoiceCardFullMana();
                break;
            case ChoiceType.CardOriginGhost:
                OnUiChoiceCardOriginGhost();
                break;
            case ChoiceType.CardClassMagicAddict:
                OnUiChoiceCardClassMagicAddict();
                break;
        }
    }

    private void OnUiChoiceMediateOrCollect()
    {
        OnUiChoiceHide();

        var Type = GameView.instance.ViewType;
        switch (Type)
        {
            case ViewType.Field:
                var MediateAvaible = PlayerCurrent.MediationEmty;
                m_btnMediate.GetComponent<Button>().interactable = MediateAvaible;
                m_btnMediate.SetActive(true);
                m_btnCollect.SetActive(true);
                m_hintChoiceMediate.SetActive(!MediateAvaible);
                break;
            case ViewType.Wild:
                m_btnBack.SetActive(true);
                m_runeStoneBox.gameObject.SetActive(true);
                break;
        }
    }

    private void OnUiChoiceCardFullMana()
    {
        OnUiChoiceHide();

        var Type = GameView.instance.ViewType;
        switch (Type)
        {
            case ViewType.Field:
                m_hintManaActive.SetActive(true);
                break;
            case ViewType.Wild:
                m_btnBack.SetActive(true);
                m_runeStoneBox.gameObject.SetActive(true);
                break;
        }
    }

    private void OnUiChoiceCardOriginGhost()
    {
        OnUiChoiceHide();

        var Type = GameView.instance.ViewType;
        switch (Type)
        {
            case ViewType.Field:
                m_hintOriginGhost.SetActive(true);
                break;
            case ViewType.Wild:
                m_btnBack.SetActive(true);
                m_runeStoneBox.gameObject.SetActive(true);
                break;
        }
    }

    private void OnUiChoiceCardClassMagicAddict()
    {
        OnUiChoiceHide();

        var Type = GameView.instance.ViewType;
        switch (Type)
        {
            case ViewType.Field:
                m_hintClassMagicAddict.SetActive(true);
                break;
            case ViewType.Wild:
                m_btnBack.SetActive(true);
                m_runeStoneBox.gameObject.SetActive(true);
                break;
        }
    }

    //GameEvent - Ui Info

    private void UiMaskInfo(bool Show)
    {
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
    } //Info MaskTween

    private void OnUiInfoHide(bool MaskTween, bool CardBack)
    {
        if (MaskTween)
            UiMaskInfo(false);
        else
        {
            m_infoMask.DOKill();
            m_infoMask.gameObject.SetActive(false);
        }

        if (m_cardView != null)
        {
            if (CardBack)
                m_cardView.MoveBack(null);
            m_cardView = null;
        }

        m_btnInfoAccept.SetActive(false);
        m_btnInfoCancel.SetActive(false);
        m_mediateOptionContent.gameObject.SetActive(false);
        m_hintCollectAccept.SetActive(false);
        m_tmpExplainOrigin.gameObject.SetActive(false);
        m_tmpExplainClass.gameObject.SetActive(false);
    } //Info Hide

    private void OnUiInfoCollect(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true);
        m_cardView = Card;
        m_cardView.MoveTop(null);

        var CollectAvaible = m_cardView.RuneStoneCost <= PlayerCurrent.RuneStone;

        m_btnInfoAccept.GetComponent<Button>().interactable = CollectAvaible;
        m_btnInfoAccept.SetActive(true);
        m_hintCollectAccept.SetActive(!CollectAvaible);
        m_btnInfoCancel.SetActive(true);
        m_tmpExplainOrigin.text = GameManager.instance.ExplainConfig.GetExplainOrigin(m_cardView.Origin);
        m_tmpExplainClass.text = GameManager.instance.ExplainConfig.GetExplainClass(m_cardView.Class);
        m_tmpExplainOrigin.gameObject.SetActive(true);
        m_tmpExplainClass.gameObject.SetActive(true);
    } //Info Collect

    private void OnUiInfoMediate()
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true);

        m_btnInfoCancel.SetActive(true);
        m_mediateOptionContent.gameObject.SetActive(true);
    } //Info Meidate

    private void OnUiInfoFullMana(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true);
        m_cardView = Card;
        m_cardView.MoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
        m_tmpExplainOrigin.text = GameManager.instance.ExplainConfig.GetExplainOrigin(m_cardView.Origin);
        m_tmpExplainClass.text = GameManager.instance.ExplainConfig.GetExplainClass(m_cardView.Class);
        m_tmpExplainOrigin.gameObject.SetActive(true);
        m_tmpExplainClass.gameObject.SetActive(true);
    } //Info Full Mana

    private void OnUiInfoOriginGhost(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true);
        m_cardView = Card;
        m_cardView.MoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
    } //Info Origin Ghost

    private void OnUiInfoClassMagicAddict(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true);
        m_cardView = Card;
        m_cardView.MoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
    } //Info Class Magic Addict

    //GameEvent - Player

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

    private void OnPlayerDoMediate(IPlayer Player, int Value, Action OnComplete) { OnComplete?.Invoke(); }

    private void OnPlayerDoCollect(IPlayer Player, ICard Card, Action OnComplete) { OnComplete?.Invoke(); }

    private void OnCardManaActive(ICard Card, Action OnComplete) { OnComplete?.Invoke(); }

    private void OnPlayerEnd(IPlayer Player, Action OnComplete)
    {
        var PlayerView = m_playerContent.transform.GetChild(GameManager.instance.PlayerIndex);
        PlayerView.DOScale(Vector2.one * 0.8f, 0.2f).OnComplete(() =>
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

    //GameEvent - Origin

    private void OnOriginDragon(Action OnComplete)
    {
        OnComplete?.Invoke();
    }

    private void OnOriginGhost(ICard Card)
    {
        GameEvent.UiChoiceCardOriginGhost();
    }

    //GameEvent - Class

    private void OnClassFighter(Action OnComplete)
    {
        OnComplete?.Invoke();
    }

    private void OnClassMagicAddict(ICard Card)
    {
        GameEvent.UiChoiceCardClassMagicAddict();
    }
}