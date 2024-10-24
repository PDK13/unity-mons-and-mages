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
        m_tmpExplainOrigin.gameObject.SetActive(false);
        m_tmpExplainClass.gameObject.SetActive(false);

        m_playerContent.gameObject.SetActive(false);
        m_runeStoneBox.gameObject.SetActive(false);

        m_btnMediate.GetComponent<Button>().onClick.AddListener(BtnMediate);
        m_btnCollect.GetComponent<Button>().onClick.AddListener(BtnCollect);
        m_btnBack.GetComponent<Button>().onClick.AddListener(BtnBack);
        m_btnInfoAccept.GetComponent<Button>().onClick.AddListener(BtnInfoAccept);
        m_btnInfoCancel.GetComponent<Button>().onClick.AddListener(BtnInfoCancel);
        m_btnInfoCancelFull.GetComponent<Button>().onClick.AddListener(BtnInfoCancel);
        m_btnEnd.GetComponent<Button>().onClick.AddListener(BtnEndGame);
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

        m_mediateOptionIndex = OptionIndex;
        for (int i = 0; i < m_mediateOptionContent.childCount; i++)
        {
            var Outline = m_mediateOptionContent.GetChild(i).GetComponent<Outline>();
            Outline.effectColor = OptionIndex == i ? Color.white : Color.black;
        }
        m_btnInfoAccept.SetActive(true);
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
    }

    public void BtnEndGame()
    {
        GameManager.instance.GameEnd();
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
            ViewButton.Base = Player[i].Base;
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
        m_tmpExplainOrigin.gameObject.SetActive(false);
        m_tmpExplainClass.gameObject.SetActive(false);
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
                m_cardView.DoMoveBack(null);
            m_cardView = null;
        }

        m_btnInfoAccept.SetActive(false);
        m_btnInfoCancel.SetActive(false);
        m_btnInfoCancelFull.SetActive(false);
        m_mediateOptionContent.gameObject.SetActive(false);
        m_hintCollectAccept.SetActive(false);
        m_tmpExplainOrigin.gameObject.SetActive(false);
        m_tmpExplainClass.gameObject.SetActive(false);
        m_runeStoneBox.gameObject.SetActive(GameView.instance.ViewType == ViewType.Wild);
    } //Info Hide

    private void OnUiInfoCollect(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        var CollectAvaible = m_cardView.RuneStoneCost <= PlayerCurrent.RuneStone;
        m_btnInfoAccept.GetComponent<Button>().interactable = CollectAvaible;
        m_btnInfoAccept.SetActive(true);
        m_hintCollectAccept.SetActive(!CollectAvaible);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);
        m_tmpExplainOrigin.text = GameManager.instance.ExplainConfig.GetExplainOrigin(m_cardView.Origin);
        m_tmpExplainClass.text = GameManager.instance.ExplainConfig.GetExplainClass(m_cardView.Class);
        m_tmpExplainOrigin.gameObject.SetActive(true);
        m_tmpExplainClass.gameObject.SetActive(true);
        m_runeStoneBox.gameObject.SetActive(true);

        UiInfoTutorial();
    } //Info Collect

    private void OnUiInfoZoom(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        m_btnInfoAccept.SetActive(false);
        m_hintCollectAccept.SetActive(false);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);
        m_tmpExplainOrigin.text = GameManager.instance.ExplainConfig.GetExplainOrigin(m_cardView.Origin);
        m_tmpExplainClass.text = GameManager.instance.ExplainConfig.GetExplainClass(m_cardView.Class);
        m_tmpExplainOrigin.gameObject.SetActive(true);
        m_tmpExplainClass.gameObject.SetActive(true);
        m_runeStoneBox.gameObject.SetActive(true);

        UiInfoTutorial();
    } //Info Zoom

    private void OnUiInfoMediate()
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true);

        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);
        m_mediateOptionContent.gameObject.SetActive(true);
        m_runeStoneBox.gameObject.SetActive(true);

        UiInfoTutorial();
    } //Info Meidate

    private void OnUiInfoFullMana(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true);
        m_cardView = Card;
        m_cardView.DoMoveTop(null);

        m_btnInfoAccept.GetComponent<Button>().interactable = true;
        m_btnInfoAccept.SetActive(true);
        m_btnInfoCancel.SetActive(true);
        m_btnInfoCancelFull.SetActive(true);
        m_tmpExplainOrigin.text = GameManager.instance.ExplainConfig.GetExplainOrigin(m_cardView.Origin);
        m_tmpExplainClass.text = GameManager.instance.ExplainConfig.GetExplainClass(m_cardView.Class);
        m_tmpExplainOrigin.gameObject.SetActive(true);
        m_tmpExplainClass.gameObject.SetActive(true);

        UiInfoTutorial();
    } //Info Full ManaCurrent

    private void OnUiInfoOriginWoodland(ICard Card)
    {
        OnUiInfoHide(false, false);
        UiMaskInfo(true);
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
        UiMaskInfo(true);
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
        UiMaskInfo(true);
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
        UiMaskInfo(true);
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
        UiMaskInfo(true);
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
        UiMaskInfo(true);
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
        OnPlayerStunnedUpdate(Player, OnComplete);
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
        var PlayerCurrent = m_playerContent.transform.GetChild(Player.Index);
        var PlayerHealth = PlayerCurrent.Find("health");
        var PlayerHealthTmp = PlayerHealth.Find("tmp-health").GetComponent<TextMeshProUGUI>();
        PlayerHealth.DOScale(Vector2.one * 1.2f, 0.1f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            PlayerHealthTmp.text = Player.HealthCurrent.ToString();
            PlayerHealth.DOScale(Vector2.one, 0.3f).SetEase(Ease.Linear).OnComplete(() => OnComplete?.Invoke());
        });
    }

    private void OnPlayerStunnedUpdate(IPlayer Player, Action OnComplete)
    {
        var PlayerCurrent = m_playerContent.transform.GetChild(Player.Index);
        var PlayerStun = PlayerCurrent.Find("stun");
        var PlayerStunTmp = PlayerStun.Find("tmp-stun").GetComponent<TextMeshProUGUI>();
        PlayerStun.DOScale(Vector2.one * 1.2f, 0.1f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            PlayerStunTmp.text = Player.StunCurrent.ToString();
            PlayerStun.DOScale(Vector2.one, 0.3f).SetEase(Ease.Linear).OnComplete(() => OnComplete?.Invoke());
        });
    }

    //GameEvent - Origin

    private void OnOriginDragon(Action OnComplete)
    {
        OnComplete?.Invoke();
    } //Roll a Dice for Dragon

    //GameEvent - Class

    private void OnClassFighter(Action OnComplete)
    {
        OnComplete?.Invoke();
    } //Roll a Dice for Fighter
}