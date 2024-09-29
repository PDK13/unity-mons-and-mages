using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardStage : MonoBehaviour, ICard
{
    public Action<Action> onEnterActive;
    public Action<Action> onPassiveActive;

    public Action<Action> onSpellActive;

    private CardData m_data;

    private Button m_button;
    private GameObject m_mask;
    private GameObject m_renderer;
    private GameObject m_rendererAlpha;
    private TextMeshProUGUI m_tmpGrowth;
    private TextMeshProUGUI m_tmpMana;
    private TextMeshProUGUI m_tmpDamage;
    private Outline m_outline;
    private GameObject m_originIcon;
    private GameObject m_classIcon;

    private RectTransform m_pointer;
    private RectTransform m_centre;
    private int m_index = 0;

    private bool m_avaible = false;
    private bool m_flip = false;
    private bool m_ready = false;
    private bool m_move = false;
    private bool m_top = false;
    private bool m_rumble = false;
    private bool m_effectAlpha = false;
    private bool m_effectOutline = false;
    private bool m_effectOrigin = false;
    private bool m_effectClass = false;
    private bool m_originGhostReady = false;
    private bool m_classMagicAddictReady = false;
    private bool m_classFlyingReady = false;

    public Vector2 CentreInPointer => m_pointer.InverseTransformPoint(m_centre.position);

    //

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_mask = transform.Find("mask").gameObject;
        m_renderer = transform.Find("renderer").gameObject;
        m_rendererAlpha = transform.Find("alpha-mask").gameObject;
        m_tmpGrowth = transform.Find("tmp-growth").GetComponent<TextMeshProUGUI>();
        m_tmpMana = transform.Find("tmp-mana").GetComponent<TextMeshProUGUI>();
        m_tmpDamage = transform.Find("tmp-damage").GetComponent<TextMeshProUGUI>();
        m_outline = m_renderer.GetComponent<Outline>();
        m_originIcon = transform.Find("origin-icon").gameObject;
        m_classIcon = transform.Find("class-icon").gameObject;
    }

    public void Start()
    {
        m_button.onClick.AddListener(BtnTap);
        m_avaible = true;
        m_ready = true;
        InfoShow(false);
    }

    //

    public void BtnTap()
    {
        if (!Avaible)
            return;

        switch (GameManager.instance.PlayerChoice)
        {
            case ChoiceType.MediateOrCollect:
                if (Player != null)
                    return;
                GameEvent.UiInfoCollect(this);
                break;
            case ChoiceType.CardFullMana:
                if (Player != GameManager.instance.PlayerCurrent || !ManaFull)
                    return;
                GameEvent.UiInfoFullMana(this);
                break;
            case ChoiceType.CardOriginGhost:
                if (Player != GameManager.instance.PlayerCurrent || !m_originGhostReady)
                    return;
                GameEvent.UiInfoOriginGhost(this);
                break;
            case ChoiceType.CardClassMagicAddict:
                if (Player != GameManager.instance.PlayerCurrent || !m_classMagicAddictReady)
                    return;
                GameEvent.UiInfoClassMagicAddict(this);
                break;
        }
    }

    //ICard

    public IPlayer Player => GetComponentInParent<IPlayer>();

    public CardNameType Name => CardNameType.Stage;

    public CardOriginType Origin => CardOriginType.None;

    public CardClassType Class => CardClassType.None;

    public int RuneStoneCost => 0;

    public int RuneStoneTake { get; private set; }

    public int ManaPoint => 0;

    public int Mana { get; private set; }

    public bool ManaFull => false;

    public int Attack => 0;

    public int Growth { get; private set; }

    public int AttackCombine => 0;


    public Image Renderer => m_renderer.GetComponent<Image>();

    public bool Avaible => m_avaible && !m_flip && m_ready && !m_move && !m_top && !m_effectAlpha;

    public int Index => m_centre.GetSiblingIndex();


    public void Init(CardData Data)
    {
        m_data = new CardData(Data, null, this);
        RuneStoneTake = m_data.RuneStoneTake;
        Mana = m_data.ManaStart;
        Growth = m_data.GrowthStart;

        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        m_renderer.GetComponent<Image>().sprite = m_data.Image;
        m_rendererAlpha.GetComponent<Image>().sprite = m_data.Image;
        m_rendererAlpha.GetComponent<CanvasGroup>().alpha = 0;
        InfoShow(false);
        InfoDamageUpdate(AttackCombine, null);
        InfoManaUpdate(Mana, m_data.ManaPoint, null);
        InfoGrowUpdate(Growth, null);
        m_originIcon.GetComponent<Image>().sprite = GameManager.instance.CardConfig.GetIconOrigin(Origin);
        m_originIcon.GetComponent<CanvasGroup>().alpha = 0;
        m_classIcon.GetComponent<Image>().sprite = GameManager.instance.CardConfig.GetIconClass(Class);
        m_classIcon.GetComponent<CanvasGroup>().alpha = 0;

        m_avaible = false;
    }

    public void Fill(RectTransform Point, RectTransform Centre) { }


    public void Ready()
    {
        m_ready = true;
    }

    public void Pointer(RectTransform Point, RectTransform Centre, bool PointChange, bool CentreChange)
    {
        m_pointer = Point;
        m_centre = Centre;

        if (PointChange)
            transform.SetParent(m_pointer, true);
        if (CentreChange)
            transform.localPosition = CentreInPointer;
        transform.SetSiblingIndex(m_centre.GetSiblingIndex());
    }


    public void FlipOpen(Action OnComplete)
    {
        if (m_flip)
            Debug.Log("Card open not done yet");
        m_flip = true;

        var FlipDuration = GameManager.instance.TweenConfig.CardAction.FlipDuration;

        transform.eulerAngles = Vector3.zero;
        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        transform
            .DOLocalRotate(Vector3.up * 90f, FlipDuration * 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_mask.SetActive(false);
                m_renderer.SetActive(true);
                transform
                    .DOLocalRotate(Vector3.zero, FlipDuration * 0.5f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        m_avaible = true;
                        m_flip = false;
                        OnComplete?.Invoke();
                    });
            });
    }

    public void FlipClose(Action OnComplete)
    {
        if (m_flip)
            Debug.Log("Card close not done yet");
        m_flip = true;

        var FlipDuration = GameManager.instance.TweenConfig.CardAction.FlipDuration;

        m_avaible = false;

        transform.eulerAngles = Vector3.zero;
        m_mask.SetActive(false);
        m_renderer.SetActive(true);
        transform
            .DOLocalRotate(Vector3.up * 90f, FlipDuration * 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_mask.SetActive(true);
                m_renderer.SetActive(false);
                transform
                    .DOLocalRotate(Vector3.zero, FlipDuration * 0.5f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        m_flip = false;
                        OnComplete?.Invoke();
                    });
            });
    }


    public void MoveTop(Action OnComplete)
    {
        if (m_top || m_move)
            Debug.Log("Card move (top) not done yet");
        m_top = true;
        m_move = true;

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        transform.SetParent(PlayerView.instance.InfoView, true);

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one * 2.35f, MoveDuration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalMove(Vector3.zero, MoveDuration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            OnComplete?.Invoke();
        });
        CardTween.Play();
    }

    public void MoveBack(Action OnComplete)
    {
        if (!m_top || m_move)
            Debug.Log("Card move (back) not done yet");
        m_top = false;
        m_move = true;

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        transform.SetParent(m_pointer, true);
        transform.SetSiblingIndex(m_pointer.childCount - 1);

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one, MoveDuration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalMove(CentreInPointer, MoveDuration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            transform.SetSiblingIndex(m_centre.GetSiblingIndex());
            OnComplete?.Invoke();
        });
        CardTween.Play();
    }

    public void MoveCentreLinear(RectTransform Centre, Action OnComplete)
    {
        if (!m_top || m_move)
            Debug.Log("Card move (linear) not done yet");
        m_centre = Centre;
        m_move = true;

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        transform
            .DOLocalMove(CentreInPointer, MoveDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_move = false;
                transform.SetSiblingIndex(m_centre.GetSiblingIndex());
                OnComplete?.Invoke();
            });
    }

    public void MoveCentreJump(RectTransform Centre, Action OnComplete)
    {
        if (!m_top || m_move)
            Debug.Log("Card move (back) not done yet");
        m_centre = Centre;
        m_move = true;

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        transform.SetSiblingIndex(m_pointer.childCount - 1);
        Renderer.maskable = false;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one * 1.35f, MoveDuration * 0.5f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalJump(CentreInPointer, 50, 1, MoveDuration).SetEase(Ease.Linear));
        CardTween.Insert(MoveDuration * 0.5f, transform.DOScale(Vector3.one, MoveDuration * 0.5f).SetEase(Ease.InCirc));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            transform.SetSiblingIndex(m_centre.GetSiblingIndex());
            Renderer.maskable = true;
            OnComplete?.Invoke();
        });
        CardTween.Play();
    }


    public void Rumble(Action OnComplete)
    {
        if (m_rumble)
            Debug.Log("Card rumble not done yet");
        m_rumble = true;

        var RumbleDuration = GameManager.instance.TweenConfig.CardAction.RumbleDuration;

        transform.SetSiblingIndex(m_pointer.childCount - 1);
        Renderer.maskable = false;
        Renderer.transform.DOScale(Vector3.one * 1.35f, RumbleDuration * 0.8f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Renderer.transform.DOScale(Vector3.one, RumbleDuration * 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.SetSiblingIndex(m_centre.GetSiblingIndex());
                GameEvent.CardRumble(this, () =>
                {
                    m_rumble = false;
                    Renderer.maskable = true;
                    OnComplete?.Invoke();
                });
            });
        });
    }


    public void EffectAlpha(Action OnComplete)
    {
        m_effectAlpha = true;

        var AlphaDuration = GameManager.instance.TweenConfig.CardAction.AlphaDuration;

        var AlphaGroup = m_rendererAlpha.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(1f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_effectAlpha = false;
                OnComplete?.Invoke();
            });
        });
    }

    public void EffectOutlineNormal(Action OnComplete)
    {
        m_effectOutline = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 2f, OutlineDuration);
        m_outline.DOColor(Color.black, OutlineDuration).OnComplete(() =>
        {
            m_effectOutline = false;
            OnComplete?.Invoke();
        });
    }

    public void EffectOutlineMana(Action OnComplete)
    {
        m_effectOutline = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.cyan, OutlineDuration).OnComplete(() =>
        {
            m_effectOutline = false;
            OnComplete?.Invoke();
        });
    }


    public void EffectOrigin(Action OnComplete) { OnComplete?.Invoke(); }

    public void EffectClass(Action OnComplete) { OnComplete?.Invoke(); }


    public void InfoShow(bool Show)
    {
        m_tmpGrowth.gameObject.SetActive(Show);
        m_tmpMana.gameObject.SetActive(Show);
        m_tmpDamage.gameObject.SetActive(Show);
    }

    private void InfoGrowUpdate(int Value, Action OnComplete) { OnComplete?.Invoke(); }

    private void InfoManaUpdate(int Value, int Max, Action OnComplete) { OnComplete?.Invoke(); }

    private void InfoDamageUpdate(int Value, Action OnComplete) { OnComplete?.Invoke(); }


    public void DoCollectActive(IPlayer Player, Action OnComplete) { OnComplete?.Invoke(); } //Collect Event


    public void DoOriginActive(Action OnComplete) { OnComplete?.Invoke(); } //Origin Event


    public void DoOriginDragonActive(int DragonLeft, Action OnComplete) { OnComplete?.Invoke(); } //Origin Dragon Event


    public void DoOriginGhostActive(int GhostCount) { } //Origin Ghost Event

    public void DoOriginGhostReady()
    {
        m_originGhostReady = true;
        m_effectOutline = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.magenta, OutlineDuration).OnComplete(() => m_effectOutline = false);
    }

    public void DoOriginGhostStart()
    {
        foreach (var Card in Player.CardQueue)
            Card.DoOriginGhostUnReady();
        DoOriginGhostProgess();
    }

    private void DoOriginGhostProgess()
    {
        EffectAlpha(() =>
        {
            Player.DoStaffNext(() =>
            {
                if (Player.CardStaffCurrent.Equals(this.GetComponent<ICard>()))
                    //Active staff when land on card choosed
                    Player.DoStaffActive(() =>
                    {
                        //Continue resolve current card before return to main progess
                        DoEnterActive(() => DoPassiveActive(() =>
                        {
                            GameManager.instance.PlayerDoStaffNext(Player, true);
                        }));
                    });
                else
                    //Continue move staff if not land on chossed card
                    DoOriginGhostProgess();
            });
        });
    }

    public void DoOriginGhostUnReady()
    {
        m_originGhostReady = false;
        if (ManaFull)
            EffectOutlineMana(null);
        else
            EffectOutlineNormal(null);
    }


    public void DoOriginInsectActive(Action OnComplete) { OnComplete?.Invoke(); } //Origin Insect Event


    public void DoOriginSirenActive(Action OnComplete) { OnComplete?.Invoke(); } //Origin Siren Event


    public void DoOriginWoodlandActive(int WoodlandCount, Action OnComplete) { OnComplete?.Invoke(); } //Origin Woodland Event


    public void DoOriginNeutralActive(Action OnComplete) { OnComplete?.Invoke(); } //Origin Neutral Active


    public void DoEnterActive(Action OnComplete) { OnComplete?.Invoke(); } //Enter Event

    public void DoPassiveActive(Action OnComplete) { OnComplete?.Invoke(); } //Passive Event


    public void DostaffActive(Action OnComplete)
    {
        EffectAlpha(OnComplete);
    }

    public void DoAttackActive(Action OnComplete) { OnComplete?.Invoke(); }

    public void DoManaFill(int Value, Action OnComplete) { OnComplete?.Invoke(); }


    public void DoManaActive(Action OnComplete) { OnComplete?.Invoke(); } //Invoke from GameManager


    public void DoClassActive(Action OnComplete) { OnComplete?.Invoke(); } //Class Event


    public void DoClassFighterActive(int AttackCombineLeft, int DiceDotSumRolled, Action OnComplete) { OnComplete?.Invoke(); } //Class Fighter Event


    public void DoClassMagicAddictActive(Action OnComplete) { OnComplete?.Invoke(); } //Class Magic Addict Event

    public void DoClassMagicAddictReady()
    {
        m_classMagicAddictReady = true;
        m_effectOutline = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.red, OutlineDuration).OnComplete(() => m_effectOutline = false);
    }

    public void DoClassMagicAddictStart()
    {
        foreach (var Card in Player.CardQueue)
            Card.DoClassMagicAddictUnReady();
        EffectAlpha(() => DoManaFill(-1, null));
    }

    public void DoClassMagicAddictUnReady()
    {
        m_classMagicAddictReady = false;
        if (ManaFull)
            EffectOutlineMana(null);
        else
            EffectOutlineNormal(null);
    }


    public void DoClassSingerActive(int SingerCount, Action OnComplete) { OnComplete?.Invoke(); } //Class Singer Event


    public void DoClassCareTakerActive(Action OnComplete) { OnComplete?.Invoke(); } //Class Care Taker Event


    public void DoClassDiffuserActive(Action OnComplete) { OnComplete?.Invoke(); } //Class Diffuser Event


    public void DoClassFlyingActive(Action OnComplete) { OnComplete?.Invoke(); } //Class Flying Event

    public void DoClassFlyingReady()
    {
        if (Name == CardNameType.Stage)
            return;
        m_classFlyingReady = true;
        m_effectOutline = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.green, OutlineDuration).OnComplete(() => m_effectOutline = false);
    }

    public void DoClassFlyingStart()
    {
        foreach (var Card in Player.CardQueue)
            Card.DoClassFlyingUnReady();
        EffectAlpha(() =>
        {
            var CardFromIndex = Player.CardQueue.ToList().IndexOf(Player.CardManaActiveCurrent);
            var CardToIndex = Player.CardQueue.ToList().IndexOf(this);
            var MoveDirection = CardFromIndex < CardToIndex ? 1 : -1;
            Player.DoCardSwap(CardFromIndex, CardToIndex + (MoveDirection * -1), () =>
            {
                Rumble(() =>
                {
                    DoManaFill(1, () =>
                    {
                        Player.DoCardSpecialActiveCurrent(null);
                        GameManager.instance.CardManaCheck(Player);
                    });
                });
            });
        });
    }

    public void DoClassFlyingUnReady()
    {
        m_classFlyingReady = false;
        if (ManaFull)
            EffectOutlineMana(null);
        else
            EffectOutlineNormal(null);
    }


    public void DoSpellActive(Action OnComplete) { OnComplete?.Invoke(); }
}