using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public static PlayerView instance;

    [SerializeField] private GameObject m_btnMediate;
    [SerializeField] private GameObject m_btnCollect;
    [SerializeField] private GameObject m_btnBack;
    [SerializeField] private GameObject m_btnEnd;

    [Space]
    [SerializeField] private Transform m_playerContent;

    [SerializeField] private Transform m_mediateOptionContent;

    [Space]
    [SerializeField] private Transform m_infoView;

    [SerializeField] private CanvasGroup m_infoMask;
    [SerializeField] private GameObject m_btnInfoAccept;
    [SerializeField] private GameObject m_btnInfoCancel;
    [SerializeField] private GameObject m_btnInfoCancelFull;

    [Space]
    //Field
    [SerializeField] private GameObject m_hintMediateAction;

    [SerializeField] private GameObject m_hintMediateUnEmty;
    [SerializeField] private GameObject m_hintCollectAction;
    [SerializeField] private GameObject m_hintPlayerContent;
    [SerializeField] private GameObject m_hintManaActive;

    //Info
    [SerializeField] private GameObject m_hintCollectAccept;

    [SerializeField] private GameObject m_hintOriginWoodland;
    [SerializeField] private GameObject m_hintOriginGhost;
    [SerializeField] private GameObject m_hintClassMagicAddict;
    [SerializeField] private GameObject m_hintClassFlying;
    [SerializeField] private GameObject m_hintSpell;

    [Space]
    [SerializeField] private Image m_imgExplainOrigin;

    [SerializeField] private Image m_imgExplainClass;

    [Space]
    [SerializeField] private RectTransform m_runeStoneBox;

    [SerializeField] private TextMeshProUGUI m_tmpRuneStone;

    [Space]
    [SerializeField] private GameObject m_notiPhaseStart;

    [SerializeField] private GameObject m_notiPhaseEnd;

    [Space]
    [SerializeField] private RectTransform m_diceContent;

    [SerializeField] private GameObject m_dicePointer;

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
        //Start
        GameEvent.onStart += OnStart;
        //Init
        GameEvent.onInit += OnInit;
        GameEvent.onInitPlayer += OnInitPlayer;
        //End
        GameEvent.onEnd += OnEnd;
        //View
        GameEvent.onViewPlayer += OnViewPlayer;
        //Ui-Choice
        GameEvent.onUiChoiceHide += OnUiChoiceHide;
        GameEvent.onUiChoiceCurrent += OnUiChoiceCurrent;
        GameEvent.onUiChoiceMediateOrCollect += OnUiChoiceMediateOrCollect;
        GameEvent.onUiChoiceCardFullMana += OnUiChoiceCardFullMana;
        GameEvent.onUiChoiceCardOriginWoodland += OnUiChoiceCardOriginWoodland;
        GameEvent.onUiChoiceCardOriginGhost += OnUiChoiceCardOriginGhost;
        GameEvent.onUiChoiceCardClassMagicAddict += OnUiChoiceCardClassMagicAddict;
        GameEvent.onUiChoiceCardClassFlying += OnUiChoiceCardClassFlying;
        GameEvent.onUiChoiceCardSpell += OnUiChoiceCardSpell;
        GameEvent.onUiChoiceCardEnter += OnUiChoiceCardEnter;
        //Ui-Info
        GameEvent.onUiInfoHide += OnUiInfoHide;
        GameEvent.onUiInfoCollect += OnUiInfoCollect;
        GameEvent.onUiInfoZoom += OnUiInfoZoom;
        GameEvent.onUiInfoMediate += OnUiInfoMediate;
        GameEvent.onUiInfoFullMana += OnUiInfoFullMana;
        GameEvent.onUiInfoOriginWoodland += OnUiInfoOriginWoodland;
        GameEvent.onUiInfoOriginGhost += OnUiInfoOriginGhost;
        GameEvent.onUiInfoClassMagicAddict += OnUiInfoClassMagicAddict;
        GameEvent.onUiInfoClassFlying += OnUiInfoClassFlying;
        GameEvent.onUiInfoCardSpell += OnUiInfoCardSpell;
        GameEvent.onUiInfoCardEnter += OnUiInfoCardEnter;
        //Player
        GameEvent.onPlayerStart += OnPlayerStart;
        GameEvent.onPlayerStunnedCheck += OnPlayerStunnedCheck;
        GameEvent.onPlayerDoChoice += OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate += OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect += OnPlayerDoCollect;
        GameEvent.onCardManaActive += OnCardManaActive;
        GameEvent.onPlayerEnd += OnPlayerEnd;
        GameEvent.onPlayerRuneStoneUpdate += OnPlayerRuneStoneUpdate;
        GameEvent.onPlayerHealthUpdate += OnPlayerHealthUpdate;
        GameEvent.onPlayerStunnedUpdate += OnPlayerStunnedUpdate;
        //Origin
        GameEvent.onOriginDragon += OnOriginDragon;
        //Class
        GameEvent.onClassFighter += OnClassFighter;
    }

    private void OnDisable()
    {
        //Start
        GameEvent.onStart -= OnStart;
        //Init
        GameEvent.onInit -= OnInit;
        GameEvent.onInitPlayer -= OnInitPlayer;
        //End
        GameEvent.onEnd -= OnEnd;
        //View
        GameEvent.onViewPlayer -= OnViewPlayer;
        //Ui-Choice
        GameEvent.onUiChoiceHide -= OnUiChoiceHide;
        GameEvent.onUiChoiceCurrent -= OnUiChoiceCurrent;
        GameEvent.onUiChoiceMediateOrCollect -= OnUiChoiceMediateOrCollect;
        GameEvent.onUiChoiceCardFullMana -= OnUiChoiceCardFullMana;
        GameEvent.onUiChoiceCardOriginWoodland -= OnUiChoiceCardOriginWoodland;
        GameEvent.onUiChoiceCardOriginGhost -= OnUiChoiceCardOriginGhost;
        GameEvent.onUiChoiceCardClassMagicAddict -= OnUiChoiceCardClassMagicAddict;
        GameEvent.onUiChoiceCardClassFlying -= OnUiChoiceCardClassFlying;
        GameEvent.onUiChoiceCardSpell -= OnUiChoiceCardSpell;
        GameEvent.onUiChoiceCardEnter -= OnUiChoiceCardEnter;
        //Ui-Info
        GameEvent.onUiInfoHide -= OnUiInfoHide;
        GameEvent.onUiInfoCollect -= OnUiInfoCollect;
        GameEvent.onUiInfoZoom -= OnUiInfoZoom;
        GameEvent.onUiInfoMediate -= OnUiInfoMediate;
        GameEvent.onUiInfoFullMana -= OnUiInfoFullMana;
        GameEvent.onUiInfoOriginWoodland -= OnUiInfoOriginWoodland;
        GameEvent.onUiInfoOriginGhost -= OnUiInfoOriginGhost;
        GameEvent.onUiInfoClassMagicAddict -= OnUiInfoClassMagicAddict;
        GameEvent.onUiInfoClassFlying -= OnUiInfoClassFlying;
        GameEvent.onUiInfoCardSpell -= OnUiInfoCardSpell;
        GameEvent.onUiInfoCardEnter -= OnUiInfoCardEnter;
        //Player
        GameEvent.onPlayerStart -= OnPlayerStart;
        GameEvent.onPlayerStunnedCheck -= OnPlayerStunnedCheck;
        GameEvent.onPlayerDoChoice -= OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate -= OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect -= OnPlayerDoCollect;
        GameEvent.onCardManaActive -= OnCardManaActive;
        GameEvent.onPlayerEnd -= OnPlayerEnd;
        GameEvent.onPlayerRuneStoneUpdate -= OnPlayerRuneStoneUpdate;
        GameEvent.onPlayerHealthUpdate -= OnPlayerHealthUpdate;
        GameEvent.onPlayerStunnedUpdate -= OnPlayerStunnedUpdate;
        //Origin
        GameEvent.onOriginDragon -= OnOriginDragon;
        //Class
        GameEvent.onClassFighter -= OnClassFighter;
    }

    private void Start()
    {
        m_btnBack.SetActive(false);
        m_btnMediate.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnEnd.SetActive(false);

        m_hintMediateAction.SetActive(false);
        m_hintMediateUnEmty.SetActive(false);
        m_hintCollectAction.SetActive(false);
        m_hintPlayerContent.SetActive(false);
        m_hintManaActive.SetActive(false);
        m_hintOriginWoodland.SetActive(false);
        m_hintOriginGhost.SetActive(false);

        m_mediateOptionContent.gameObject.SetActive(false);
        m_infoMask.gameObject.SetActive(false);
        m_btnInfoAccept.SetActive(false);
        m_btnInfoCancel.SetActive(false);
        m_btnInfoCancelFull.SetActive(false);
        m_hintCollectAccept.SetActive(false);
        m_imgExplainOrigin.gameObject.SetActive(false);
        m_imgExplainClass.gameObject.SetActive(false);

        m_playerContent.gameObject.SetActive(false);
        m_runeStoneBox.gameObject.SetActive(false);

        m_notiPhaseStart.gameObject.SetActive(false);
        m_notiPhaseEnd.gameObject.SetActive(false);

        m_dicePointer.SetActive(false);

        m_btnMediate.GetComponent<Button>().onClick.AddListener(BtnMediate);
        m_btnCollect.GetComponent<Button>().onClick.AddListener(BtnCollect);
        m_btnBack.GetComponent<Button>().onClick.AddListener(BtnBack);
        m_btnInfoAccept.GetComponent<Button>().onClick.AddListener(BtnInfoAccept);
        m_btnInfoCancel.GetComponent<Button>().onClick.AddListener(BtnInfoCancel);
        m_btnInfoCancelFull.GetComponent<Button>().onClick.AddListener(BtnInfoCancel);
        m_btnEnd.GetComponent<Button>().onClick.AddListener(BtnEnd);
    }

    //Button

    public void BtnMediate()
    {
        if (GameManager.instance.PlayerChoice != ChoiceType.MediateOrCollect)
            return;

        if (GameManager.instance.TutorialActive)
        {
            if (GameManager.instance.TutorialStepCurrent.ButtonMeidate)
                GameManager.instance.TutorialContinue(true);
            else
                return;
        }

        m_mediateOptionIndex = -1;

        for (int i = 0; i < m_mediateOptionContent.childCount; i++)
        {
            var Outline = m_mediateOptionContent.GetChild(i).GetComponent<Outline>();
            Outline.effectColor = Color.black;
            var Button = m_mediateOptionContent.GetChild(i).GetComponent<Button>();
            Button.interactable = GameManager.instance.PlayerCurrent.RuneStone >= i + 1;
        }

        GameEvent.UiInfoMediate();

        m_btnMediate.transform.DOKill();
        m_btnMediate.transform.localScale = Vector3.one;

        SoundManager.instance.Play(SoundType.ButtonPress);
    }

    public void BtnCollect()
    {
        if (GameManager.instance.PlayerChoice != ChoiceType.MediateOrCollect)
            return;

        if (GameManager.instance.TutorialActive)
        {
            if (GameManager.instance.TutorialStepCurrent.ButtonCollect)
                GameManager.instance.TutorialContinue(true);
            else
                return;
        }

        GameEvent.ViewArea(ViewType.Wild, null);

        m_btnCollect.transform.DOKill();
        m_btnCollect.transform.localScale = Vector3.one;

        SoundManager.instance.Play(SoundType.ButtonPress);
    }

    public void BtnBack()
    {
        if (GameManager.instance.PlayerChoice != ChoiceType.MediateOrCollect)
            return;

        if (GameManager.instance.TutorialActive)
        {
            if (GameManager.instance.TutorialStepCurrent.ButtonBack)
                GameManager.instance.TutorialContinue(true);
            else
                return;
        }

        GameEvent.ViewArea(ViewType.Field, null);

        m_btnBack.transform.DOKill();
        m_btnBack.transform.localScale = Vector3.one;

        SoundManager.instance.Play(SoundType.ButtonPress);
    }

    public void BtnInfoAccept()
    {
        if (GameManager.instance.PlayerChoice == ChoiceType.None)
            return;

        if (GameManager.instance.TutorialActive)
        {
            if (GameManager.instance.TutorialStepCurrent.ButtonAccept)
                GameManager.instance.TutorialContinue(false);
            else
                return;
        }

        GameEvent.ViewPlayer(PlayerCurrent, () =>
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

                case ChoiceType.CardOriginWoodland:
                    GameManager.instance.CardOriginWoodlandDoStart(m_cardView);
                    break;

                case ChoiceType.CardOriginGhost:
                    GameManager.instance.CardOriginGhostDoStart(m_cardView);
                    break;

                case ChoiceType.CardClassMagicAddict:
                    GameManager.instance.CardClassMagicAddictStart(m_cardView);
                    break;

                case ChoiceType.CardClassFlying:
                    GameManager.instance.CardClassFlyingStart(m_cardView);
                    break;

                case ChoiceType.CardSpell:
                    GameManager.instance.CardSpellStart(m_cardView);
                    break;

                case ChoiceType.CardEnter:
                    GameManager.instance.CardEnterStart(m_cardView);
                    break;
            }
        });

        m_btnInfoAccept.transform.DOKill();
        m_btnInfoAccept.transform.localScale = Vector3.one;

        SoundManager.instance.Play(SoundType.ButtonPress);
    }

    public void BtnInfoCancel()
    {
        if (GameManager.instance.PlayerChoice == ChoiceType.None)
            return;

        if (GameManager.instance.TutorialActive)
        {
            if (GameManager.instance.TutorialStepCurrent.ButtonCancel)
                GameManager.instance.TutorialContinue(false);
            else
                return;
        }

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
            case ChoiceType.CardOriginWoodland:
            case ChoiceType.CardOriginGhost:
            case ChoiceType.CardClassMagicAddict:
            case ChoiceType.CardClassFlying:
            case ChoiceType.CardSpell:
            case ChoiceType.CardEnter:
                GameEvent.UiInfoHide(true, true);
                break;
        }

        m_btnInfoCancel.transform.DOKill();
        m_btnInfoCancel.transform.localScale = Vector3.one;

        SoundManager.instance.Play(SoundType.ButtonPress);
    }

    public void BtnMediateOption(int OptionIndex)
    {
        if (GameManager.instance.PlayerChoice != ChoiceType.MediateOrCollect)
            return;

        if (GameManager.instance.TutorialActive)
        {
            if (GameManager.instance.TutorialStepCurrent.ButtonMeidateOption)
                GameManager.instance.TutorialContinue(true);
            else
                return;
        }

        if (m_mediateOptionIndex == OptionIndex)
        {
            BtnInfoAccept();
            return;
        }

        m_mediateOptionIndex = OptionIndex;
        for (int i = 0; i < m_mediateOptionContent.childCount; i++)
        {
            var Outline = m_mediateOptionContent.GetChild(i).GetComponent<Outline>();
            Outline.effectColor = OptionIndex == i ? Color.white : Color.black;
        }
        m_btnInfoAccept.SetActive(true);

        SoundManager.instance.Play(SoundType.ButtonPress);
    }

    public void BtnViewPlayer(int PlayerIndex)
    {
        if (GameManager.instance.PlayerChoice == ChoiceType.None)
            return;

        if (GameManager.instance.TutorialActive)
        {
            if (GameManager.instance.TutorialStepCurrent.ButtonPlayer)
                GameManager.instance.TutorialContinue(true);
            else
                return;
        }

        GameEvent.ViewPlayer(GameManager.instance.GetPlayer(PlayerIndex), null);

        SoundManager.instance.Play(SoundType.ButtonPress);
    }

    public void BtnEnd()
    {
        GameEvent.EndView();

        SoundManager.instance.Play(SoundType.ButtonPress);
    }

    //GameEvent - Start

    private void OnStart()
    {
        m_btnEnd.SetActive(true);
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
            ViewButton.View = this;
            ViewButton.Health = Player[i].HealthCurrent;
            ViewButton.Stun = Player[i].StunCurrent;
        }
    }

    //GameEvent - End

    private void OnEnd()
    {
        //Choice - Hide
        m_btnMediate.SetActive(false);
        m_btnCollect.SetActive(false);
        m_btnBack.SetActive(false);
        m_btnEnd.SetActive(false);
        //
        m_playerContent.gameObject.SetActive(false);
        m_hintMediateAction.SetActive(false);
        m_hintMediateUnEmty.SetActive(false);
        m_hintCollectAction.SetActive(false);
        m_hintPlayerContent.SetActive(false);
        m_hintManaActive.SetActive(false);
        m_hintOriginWoodland.SetActive(false);
        m_hintOriginGhost.SetActive(false);
        m_hintClassMagicAddict.SetActive(false);
        m_hintClassFlying.SetActive(false);
        m_hintSpell.SetActive(false);
        m_runeStoneBox.gameObject.SetActive(false);
        //Info - Hide
        m_infoMask.DOKill();
        m_infoMask.gameObject.SetActive(false);
        m_btnInfoAccept.SetActive(false);
        m_btnInfoCancel.SetActive(false);
        m_mediateOptionContent.gameObject.SetActive(false);
        m_hintCollectAccept.SetActive(false);
        m_imgExplainOrigin.gameObject.SetActive(false);
        m_imgExplainClass.gameObject.SetActive(false);
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
        m_hintMediateAction.SetActive(false);
        m_hintMediateUnEmty.SetActive(false);
        m_hintCollectAction.SetActive(false);
        m_hintPlayerContent.SetActive(false);
        m_hintManaActive.SetActive(false);
        m_hintOriginWoodland.SetActive(false);
        m_hintOriginGhost.SetActive(false);
        m_hintClassMagicAddict.SetActive(false);
        m_hintClassFlying.SetActive(false);
        m_hintSpell.SetActive(false);
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

            case ChoiceType.CardOriginWoodland:
                OnUiChoiceCardOriginWoodland();
                break;

            case ChoiceType.CardOriginGhost:
                OnUiChoiceCardOriginGhost();
                break;

            case ChoiceType.CardClassMagicAddict:
                OnUiChoiceCardClassMagicAddict();
                break;

            case ChoiceType.CardClassFlying:
                OnUiChoiceCardClassFlying();
                break;

            case ChoiceType.CardSpell:
                OnUiChoiceCardSpell();
                break;

            case ChoiceType.CardEnter:
                OnUiChoiceCardEnter();
                break;
        }
    }

    private void OnUiChoiceMediateOrCollect()
    {
        OnUiChoiceHide();

        var Tutorial = GameManager.instance.TutorialActive;
        var Type = GameView.instance.ViewType;
        switch (Type)
        {
            case ViewType.Field:
                var MediateAvaible = PlayerCurrent.MediationEmty;
                m_btnMediate.GetComponent<Button>().interactable = MediateAvaible;
                m_btnMediate.SetActive(true);
                m_btnCollect.SetActive(true);
                m_hintMediateAction.SetActive(MediateAvaible);
                m_hintMediateUnEmty.SetActive(!MediateAvaible);
                m_hintCollectAction.SetActive(true);
                m_hintPlayerContent.SetActive(true);
                if (Tutorial)
                {
                    var TutorialMeidate = GameManager.instance.TutorialStepCurrent.ButtonMeidate;
                    if (TutorialMeidate)
                        m_btnMediate.transform.DOScale(Vector2.one * 1.05f, 0.2f).SetLoops(-1, LoopType.Yoyo);
                    else
                    {
                        m_btnMediate.transform.DOKill();
                        m_btnMediate.transform.localScale = Vector3.one;
                    }
                    var TutorialCollect = GameManager.instance.TutorialStepCurrent.ButtonCollect;
                    if (TutorialCollect)
                        m_btnCollect.transform.DOScale(Vector2.one * 1.05f, 0.2f).SetLoops(-1, LoopType.Yoyo);
                    else
                    {
                        m_btnCollect.transform.DOKill();
                        m_btnCollect.transform.localScale = Vector3.one;
                    }
                }
                else
                {
                    m_btnMediate.transform.DOKill();
                    m_btnMediate.transform.localScale = Vector3.one;
                    m_btnCollect.transform.DOKill();
                    m_btnCollect.transform.localScale = Vector3.one;
                }
                break;

            case ViewType.Wild:
                m_btnBack.SetActive(true);
                m_runeStoneBox.gameObject.SetActive(true);
                if (Tutorial)
                {
                    var TutorialBack = GameManager.instance.TutorialStepCurrent.ButtonBack;
                    if (TutorialBack)
                        m_btnBack.transform.DOScale(Vector2.one * 1.05f, 0.2f).SetLoops(-1, LoopType.Yoyo);
                    else
                    {
                        m_btnBack.transform.DOKill();
                        m_btnBack.transform.localScale = Vector3.one;
                    }
                }
                else
                {
                    m_btnBack.transform.DOKill();
                    m_btnBack.transform.localScale = Vector3.one;
                }
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

    private void OnUiChoiceCardOriginWoodland()
    {
        OnUiChoiceHide();

        var Type = GameView.instance.ViewType;
        switch (Type)
        {
            case ViewType.Field:
                m_hintOriginWoodland.SetActive(true);
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

    private void OnUiChoiceCardClassFlying()
    {
        OnUiChoiceHide();

        var Type = GameView.instance.ViewType;
        switch (Type)
        {
            case ViewType.Field:
                m_hintClassFlying.SetActive(true);
                break;

            case ViewType.Wild:
                m_btnBack.SetActive(true);
                m_runeStoneBox.gameObject.SetActive(true);
                break;
        }
    }

    private void OnUiChoiceCardSpell()
    {
        OnUiChoiceHide();

        var Type = GameView.instance.ViewType;
        switch (Type)
        {
            case ViewType.Field:
                m_hintSpell.SetActive(true);
                break;

            case ViewType.Wild:
                m_btnBack.SetActive(true);
                m_runeStoneBox.gameObject.SetActive(true);
                break;
        }
    }

    private void OnUiChoiceCardEnter()
    {
        OnUiChoiceHide();

        var Type = GameView.instance.ViewType;
        switch (Type)
        {
            case ViewType.Field:
                m_hintSpell.SetActive(true);
                break;

            case ViewType.Wild:
                m_btnBack.SetActive(true);
                m_runeStoneBox.gameObject.SetActive(true);
                break;
        }
    }

    //GameEvent - Ui Info

    private void UiMaskInfo(bool Show, Action OnComplete)
    {
        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration + 0.02f;

        m_infoMask.DOKill();
        m_infoMask.gameObject.SetActive(true);
        if (Show)
        {
            m_infoMask.alpha = 0;
            m_infoMask
                .DOFade(1f, MoveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => OnComplete?.Invoke());
        }
        else
        {
            m_infoMask.alpha = 1;
            m_infoMask
                .DOFade(0f, MoveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    m_infoMask.gameObject.SetActive(false);
                    OnComplete?.Invoke();
                });
        }
    } //Info MaskTween

    private void OnUiInfoHide(bool MaskTween, bool CardBack)
    {
        if (MaskTween)
            UiMaskInfo(false, null);
        else
        {
            m_infoMask.DOKill();
            m_infoMask.gameObject.SetActive(false);
        }

        if (m_cardView != null)
        {
            if (CardBack)
                m_cardView.DoMoveBack(null);
            m_cardView = null;
        }

        m_btnInfoAccept.SetActive(false);
        m_btnInfoCancel.SetActive(false);
        m_btnInfoCancelFull.SetActive(false);
        m_mediateOptionContent.gameObject.SetActive(false);
        m_hintCollectAccept.SetActive(false);
        m_imgExplainOrigin.gameObject.SetActive(false);
        m_imgExplainClass.gameObject.SetActive(false);
        m_runeStoneBox.gameObject.SetActive(GameView.instance.ViewType == ViewType.Wild);
    } //Info Hide

    private void OnUiInfoCollect(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true, null);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        var CollectAvaible = m_cardView.RuneStoneCost <= PlayerCurrent.RuneStone;
        m_btnInfoAccept.GetComponent<Button>().interactable = CollectAvaible;
        m_btnInfoAccept.SetActive(true);
        m_hintCollectAccept.SetActive(!CollectAvaible);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);
        m_imgExplainOrigin.sprite = GameManager.instance.ExplainConfig.GetExplainOrigin(m_cardView.Origin);
        m_imgExplainClass.sprite = GameManager.instance.ExplainConfig.GetExplainClass(m_cardView.Class);
        m_imgExplainOrigin.SetNativeSize();
        m_imgExplainClass.SetNativeSize();
        m_imgExplainOrigin.gameObject.SetActive(m_imgExplainOrigin.sprite != null);
        m_imgExplainClass.gameObject.SetActive(m_imgExplainClass.sprite != null);
        m_runeStoneBox.gameObject.SetActive(true);

        UiInfoTutorial();
    } //Info Collect

    private void OnUiInfoZoom(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true, null);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        m_btnInfoAccept.SetActive(false);
        m_hintCollectAccept.SetActive(false);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);
        m_imgExplainOrigin.sprite = GameManager.instance.ExplainConfig.GetExplainOrigin(m_cardView.Origin);
        m_imgExplainClass.sprite = GameManager.instance.ExplainConfig.GetExplainClass(m_cardView.Class);
        m_imgExplainOrigin.SetNativeSize();
        m_imgExplainClass.SetNativeSize();
        m_imgExplainOrigin.gameObject.SetActive(m_imgExplainOrigin.sprite != null);
        m_imgExplainClass.gameObject.SetActive(m_imgExplainClass.sprite != null);
        m_runeStoneBox.gameObject.SetActive(true);

        UiInfoTutorial();
    } //Info Zoom

    private void OnUiInfoMediate()
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true, null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(false);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);
        m_mediateOptionContent.gameObject.SetActive(true);
        m_runeStoneBox.gameObject.SetActive(true);

        UiInfoTutorial();
    } //Info Meidate

    private void OnUiInfoFullMana(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true, null);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);
        m_imgExplainOrigin.sprite = GameManager.instance.ExplainConfig.GetExplainOrigin(m_cardView.Origin);
        m_imgExplainClass.sprite = GameManager.instance.ExplainConfig.GetExplainClass(m_cardView.Class);
        m_imgExplainOrigin.SetNativeSize();
        m_imgExplainClass.SetNativeSize();
        m_imgExplainOrigin.gameObject.SetActive(m_imgExplainOrigin.sprite != null);
        m_imgExplainClass.gameObject.SetActive(m_imgExplainClass.sprite != null);

        UiInfoTutorial();
    } //Info Full ManaCurrent

    private void OnUiInfoOriginWoodland(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true, null);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);

        UiInfoTutorial();
    } //Info Origin Woodland

    private void OnUiInfoOriginGhost(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true, null);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);

        UiInfoTutorial();
    } //Info Origin Ghost

    private void OnUiInfoClassMagicAddict(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true, null);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);

        UiInfoTutorial();
    } //Info Class Magic Addict

    private void OnUiInfoClassFlying(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true, null);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);

        UiInfoTutorial();
    } //Info Class Magic Addict

    private void OnUiInfoCardSpell(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true, null);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);

        UiInfoTutorial();
    } //Info Card Spell

    private void OnUiInfoCardEnter(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true, null);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);

        UiInfoTutorial();
    } //Info Class Magic Addict

    private void UiInfoTutorial()
    {
        var Tutorial = GameManager.instance.TutorialActive;
        if (Tutorial)
        {
            var TutorialAccept = GameManager.instance.TutorialStepCurrent.ButtonAccept;
            if (TutorialAccept)
                m_btnInfoAccept.transform.DOScale(Vector2.one * 1.05f, 0.2f).SetLoops(-1, LoopType.Yoyo);
            else
                m_btnInfoAccept.transform.localScale = Vector3.one;
            var TutorialCancel = GameManager.instance.TutorialStepCurrent.ButtonCancel;
            if (TutorialCancel)
                m_btnInfoCancel.transform.DOScale(Vector2.one * 1.05f, 0.2f).SetLoops(-1, LoopType.Yoyo);
            else
                m_btnInfoCancel.transform.localScale = Vector3.one;
        }
        else
        {
            m_btnInfoAccept.transform.DOKill();
            m_btnInfoAccept.transform.localScale = Vector3.one;
            m_btnInfoCancel.transform.DOKill();
            m_btnInfoCancel.transform.localScale = Vector3.one;
        }
    }

    private void UiInfoDice(DiceConfigData[] DiceResult, Action OnComplete)
    {
        UiMaskInfo(true, null);
        var DiceScale = m_dicePointer.transform.GetChild(0).localScale;
        var DiceDuration = 1f;
        var DicePoint = new List<GameObject>();
        for (int i = 0; i < DiceResult.Length; i++)
        {
            var DiceClone = Instantiate(m_dicePointer, m_diceContent);
            DiceClone.SetActive(true);
            DicePoint.Add(DiceClone);
            var DiceIcon = DiceClone.transform.GetChild(0);
            DiceIcon.GetComponent<Image>().sprite = DiceResult[i].Face;
            DiceIcon.localScale = Vector3.zero;
            DiceIcon.DOScale(DiceScale, DiceDuration);

            if (i != 0)
            {
                DiceIcon.DOLocalJump(Vector3.zero, 150f, 1, DiceDuration).OnComplete(() =>
                DiceIcon.DOScale(DiceScale + Vector3.one * 0.25f, 0.1f).SetDelay(1.5f).OnComplete(() =>
                    DiceIcon.DOScale(DiceScale, 0.1f).OnComplete(() =>
                        DiceIcon.DOScale(DiceScale, 0.5f))));
                continue;
            }

            SoundManager.instance.Play(SoundType.TurnHint);
            DiceIcon.DOLocalJump(Vector3.zero, 150f, 1, DiceDuration).OnComplete(() =>
            {
                DiceIcon
                    .DOScale(DiceScale + Vector3.one * 0.25f, 0.1f)
                    .SetDelay(1.5f)
                    .OnStart(() => SoundManager.instance.Play(SoundType.CardChoice))
                    .OnComplete(() =>
                    {
                        DiceIcon.DOScale(DiceScale, 0.1f).OnComplete(() =>
                        {
                            DiceIcon.DOScale(DiceScale, 0.5f).OnComplete(() =>
                            {
                                UiMaskInfo(false, null);
                                foreach (var DicePointCheck in DicePoint)
                                    Destroy(DicePointCheck);
                                OnComplete?.Invoke();
                            });
                        });
                    });
            });
        }
    }

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

        SoundManager.instance.Play(SoundType.CardChoice);
    }

    private void OnPlayerStunnedCheck(IPlayer Player, Action OnComplete)
    {
        OnPlayerStunnedUpdate(Player, OnComplete);
    }

    private void OnPlayerDoChoice(IPlayer Player, Action OnComplete)
    {
        NotiPhase(m_notiPhaseStart, OnComplete);
    }

    private void OnPlayerDoMediate(IPlayer Player, int Value, Action OnComplete)
    { OnComplete?.Invoke(); }

    private void OnPlayerDoCollect(IPlayer Player, ICard Card, Action OnComplete)
    { OnComplete?.Invoke(); }

    private void OnCardManaActive(ICard Card, Action OnComplete)
    { OnComplete?.Invoke(); }

    private void OnPlayerEnd(IPlayer Player, Action OnComplete)
    {
        NotiPhase(m_notiPhaseEnd, () =>
        {
            var PlayerView = m_playerContent.transform.GetChild(GameManager.instance.PlayerIndex);
            PlayerView.DOScale(Vector2.one * 0.8f, 0.2f).OnComplete(() =>
            {
                PlayerView.DOScale(Vector2.one, 0.2f).OnComplete(() =>
                {
                    OnComplete?.Invoke();
                });
            });
        });
    }

    private void OnPlayerRuneStoneUpdate(IPlayer Player, Action OnComplete)
    {
        m_runeStoneBox.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            m_tmpRuneStone.text = Player.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
            m_runeStoneBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
        });
    }

    private void OnPlayerHealthUpdate(IPlayer Player, Action OnComplete)
    {
        m_playerContent.transform.GetChild(Player.Index).GetComponent<PlayerViewButton>().HealthUpdate(Player.HealthCurrent, OnComplete);
    }

    private void OnPlayerStunnedUpdate(IPlayer Player, Action OnComplete)
    {
        m_playerContent.transform.GetChild(Player.Index).GetComponent<PlayerViewButton>().StunUpdate(Player.StunCurrent, OnComplete);
        SoundManager.instance.Play(SoundType.CardChoice);
    }

    //GameEvent - Origin

    private void OnOriginDragon(DiceConfigData[] DiceResult, Action OnComplete)
    {
        UiInfoDice(DiceResult, OnComplete);
    } //Roll a Dice for Dragon

    //GameEvent - Class

    private void OnClassFighter(DiceConfigData[] DiceResult, Action OnComplete)
    {
        UiInfoDice(DiceResult, OnComplete);
    } //Roll a Dice for Fighter

    //Noti

    private void NotiPhase(GameObject Noti, Action OnComplete)
    {
        var NotiAlpha = Noti.GetComponent<CanvasGroup>();
        Noti.transform.localScale = Vector3.one * 2f;
        NotiAlpha.alpha = 1;
        Noti.SetActive(true);
        Noti.transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
        {
            NotiAlpha.DOFade(0f, 0.2f).SetDelay(1f).OnStart(() =>
            {
                SoundManager.instance.Play(SoundType.TurnHint);
            });
            Noti.transform.DOScale(Vector3.one * 2f, 0.2f).SetDelay(1f).OnComplete(() =>
            {
                OnComplete?.Invoke();
                Noti.SetActive(false);
            });
        });
        SoundManager.instance.Play(SoundType.TurnHint);
    }
}