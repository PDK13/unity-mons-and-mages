using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour, ICard
{
    private RectTransform m_recTransform;
    private Button m_button;
    private GameObject m_mask;
    private GameObject m_renderer;
    private ManaController m_manaGroup;
    private GameObject m_rendererAlpha;
    private TextMeshProUGUI m_tmpGrowth;
    private TextMeshProUGUI m_tmpMana;
    private TextMeshProUGUI m_tmpDamage;
    private Outline m_outline;
    private GameObject m_originIcon;
    private GameObject m_classIcon;
    private GameObject m_runeStoneIcon;

    private bool m_open = false;
    private bool m_top = false; //Đang được hiển thị trên cùng của màn hình
    private bool m_choice = false; //Có thể được lựa chọn trong trường hợp đặc biệt
    private bool m_choiceOnce = false; //Chỉ có thể được lựa chọn một lần duy nhật trong trường hợp đặc biệt
    private bool m_tutorial = false;
    private bool m_flip = false;
    private bool m_move = false;
    private bool m_effect = false;

    //

    public Vector2 CentreInPointer => Pointer.InverseTransformPoint(Centre.position);

    //

    private IPlayer m_player;
    private string m_named = "";
    private CardNameType m_name;
    private CardOriginType m_origin;
    private CardClassType m_class;
    private Sprite m_image;
    private int m_runeStoneCost;
    private int m_runeStoneTake; //Khi gật phép tiến vào, nhận xu triệu hồi
    private int m_manaPoint;
    private int m_manaCurrent;
    private int m_attackPoint;
    private int m_growthCurrent;

    private Action m_progessEvent;
    private ProgessCollectType m_progessCollectCurrent = ProgessCollectType.None;
    private ProgessManaType m_progessManaCurrent = ProgessManaType.None;

    //

    private void Awake()
    {
        m_recTransform = GetComponent<RectTransform>();

        m_button = GetComponent<Button>();
        m_mask = transform.Find("mask").gameObject;
        m_renderer = transform.Find("renderer").gameObject;
        for (int i = 0; i < m_renderer.transform.childCount; i++)
            m_renderer.transform.GetChild(i).gameObject.SetActive(false);
        m_rendererAlpha = transform.Find("alpha-mask").gameObject;
        m_tmpGrowth = transform.Find("tmp-growth").GetComponent<TextMeshProUGUI>();
        m_tmpMana = transform.Find("tmp-mana").GetComponent<TextMeshProUGUI>();
        m_tmpDamage = transform.Find("tmp-damage").GetComponent<TextMeshProUGUI>();
        m_outline = m_renderer.GetComponent<Outline>();
        m_originIcon = transform.Find("origin-icon").gameObject;
        m_classIcon = transform.Find("class-icon").gameObject;
        m_runeStoneIcon = transform.Find("rune-stone").gameObject;
    }

    private void OnEnable()
    {
        GameEvent.onTutorialCard += OnTutorialCard;
        GameEvent.onEnd += OnEnd;
    }

    private void OnDisable()
    {
        GameEvent.onTutorialCard -= OnTutorialCard;
        GameEvent.onEnd -= OnEnd;
    }

    private void Start()
    {
        m_button.onClick.AddListener(BtnTap);
    }

    //

    public void BtnTap()
    {
        if (!Avaible)
            return;

        if (GameManager.instance.TutorialActive)
        {
            if (GameManager.instance.TutorialStepCurrent.Card)
            {
                if (!m_tutorial)
                    return;
                DoTutorialUnReady();
                GameManager.instance.TutorialContinue(true);
            }
        }

        switch (GameManager.instance.PlayerChoice)
        {
            case ChoiceType.MediateOrCollect:
                if (m_player != null)
                {
                    GameEvent.UiInfoZoom(this);
                    return;
                }
                GameEvent.UiInfoCollect(this);
                break;
            case ChoiceType.CardFullMana:
                if (m_player != GameManager.instance.PlayerCurrent || !ManaFull)
                    return;
                GameEvent.UiInfoFullMana(this);
                break;
            case ChoiceType.CardOriginWoodland:
                if (m_player != GameManager.instance.PlayerCurrent || !m_choice)
                    return;
                GameEvent.UiInfoOriginWoodland(this);
                break;
            case ChoiceType.CardOriginGhost:
                if (m_player != GameManager.instance.PlayerCurrent || !m_choice)
                    return;
                GameEvent.UiInfoOriginGhost(this);
                break;
            case ChoiceType.CardClassMagicAddict:
                if (m_player != GameManager.instance.PlayerCurrent || !m_choice)
                    return;
                GameEvent.UiInfoClassMagicAddict(this);
                break;
            case ChoiceType.CardClassFlying:
                if (m_player != GameManager.instance.PlayerCurrent || !m_choice)
                    return;
                GameEvent.UiInfoClassFlying(this);
                break;
            case ChoiceType.CardSpell:
                if (m_player != GameManager.instance.PlayerCurrent || !m_choice)
                    return;
                GameEvent.UiInfoCardSpell(this);
                break;
            case ChoiceType.CardEnter:
                if (m_player != GameManager.instance.PlayerCurrent || !m_choice)
                    return;
                GameEvent.UiInfoCardEnter(this);
                break;
        }
    }

    //

    private void OnTutorialCard(CardNameType CardName)
    {
        if (this.Name == CardName)
            DoTutorialReady();
        else
            DoTutorialUnReady();
    }

    private void OnEnd()
    {
        this.transform.DOKill();
        this.m_renderer.transform.DOKill();
        Destroy(this.gameObject);
    }

    //ICard

    public IPlayer Player => m_player;

    public CardNameType Name => m_name;

    public CardOriginType Origin => m_origin;

    public CardClassType Class => m_class;

    public CardType Type => m_name == CardNameType.Stage ? CardType.None : m_manaPoint != 0 ? CardType.Mons : CardType.Landmark;

    public int RuneStoneCost => m_runeStoneCost;

    public int RuneStoneTake => m_runeStoneTake;

    public int ManaPoint => m_manaPoint;

    public int ManaCurrent => m_manaCurrent;

    public bool ManaFull => m_manaPoint > 0 && m_manaCurrent >= m_manaPoint;

    public int Attack => m_attackPoint;

    public int Growth => m_growthCurrent;

    public int AttackCombine => m_attackPoint + m_growthCurrent;


    public RectTransform Pointer { get; set; }

    public RectTransform Centre { get; set; }

    public RectTransform Body => GetComponent<RectTransform>();

    public Image Renderer => m_renderer.GetComponent<Image>();

    public bool Avaible =>
        m_open &&
        !m_flip &&
        !m_move &&
        !m_top &&
        !m_effect;

    public int Index => Centre.GetSiblingIndex();

    public List<DiceConfigData> OriginDragonDice { get; set; } = new List<DiceConfigData>();

    public int OriginDragonDiceBite { get; set; }

    public int OriginDragonDiceDragon { get; set; }

    public List<DiceConfigData> ClassFighterDice { get; set; } = new List<DiceConfigData>();

    public int ClassFighterDiceDot { get; set; }

    public int ClassFlyingMoveDir { get; set; }

    public bool ClassFlyingManaFull { get; set; }


    public void Init(CardData Data)
    {
        m_player = null;
        m_named = Data.Named;
        m_name = Data.Name;
        m_origin = Data.Origin;
        m_class = Data.Class;
        m_image = Data.Image;
        m_runeStoneCost = Data.RuneStoneCost;
        m_runeStoneTake = Data.RuneStoneTake;
        m_manaPoint = Data.ManaPoint;
        m_manaCurrent = Data.ManaStart;
        m_attackPoint = Data.AttackPoint;
        m_growthCurrent = Data.GrowthStart;

        if (m_name != CardNameType.Stage)
        {
            m_open = false;
            m_mask.SetActive(true);
            m_renderer.SetActive(false);
            m_renderer.GetComponent<Image>().sprite = m_image;
            //
            switch (m_manaPoint)
            {
                case 3:
                    m_manaGroup = m_renderer.transform.Find("mana-group-3").GetComponent<ManaController>();
                    break;
                case 2:
                    m_manaGroup = m_renderer.transform.Find("mana-group-2").GetComponent<ManaController>();
                    break;
            }
            if (m_manaGroup != null)
                m_manaGroup.gameObject.SetActive(true);
            //
            m_rendererAlpha.GetComponent<Image>().sprite = m_image;
            m_rendererAlpha.GetComponent<CanvasGroup>().alpha = 0;
            InfoShow(false);
            InfoDamageUpdate(AttackCombine, null);
            InfoManaUpdate(m_manaCurrent, m_manaPoint, null);
            InfoGrowUpdate(m_growthCurrent, null);
            m_originIcon.GetComponent<Image>().sprite = GameManager.instance.CardConfig.GetIconOrigin(Origin);
            m_originIcon.GetComponent<CanvasGroup>().alpha = 0;
            m_classIcon.GetComponent<Image>().sprite = GameManager.instance.CardConfig.GetIconClass(Class);
            m_classIcon.GetComponent<CanvasGroup>().alpha = 0;
            m_runeStoneIcon.SetActive(false);
        }
        else
        {
            m_player = GetComponentInParent<IPlayer>();
            m_open = true;
            m_mask.SetActive(true);
            m_rendererAlpha.GetComponent<CanvasGroup>().alpha = 0;
            InfoShow(false);
        }
    }

    public void DoFill(RectTransform Pointer, RectTransform Centre)
    {
        this.Pointer = Pointer;
        this.Centre = Centre;

        var DurationMove = GameManager.instance.TweenConfig.WildFill.MoveDuration;
        var EaseMove = GameManager.instance.TweenConfig.WildFill.MoveEase;

        transform.SetParent(Pointer, true);
        transform.GetComponent<RectTransform>().DOLocalMove(CentreInPointer, DurationMove).SetEase(EaseMove);

        DoFlipOpen(null);
    }

    public void DoFixed()
    {
        transform.SetParent(Pointer, true);
        transform.localPosition = CentreInPointer;
        transform.SetSiblingIndex(Mathf.Min(Centre.GetSiblingIndex(), Pointer.childCount - 2));
    }


    public void DoChoiceReady()
    {
        if (m_choiceOnce)
            return;
        m_choice = true;
        DoEffectOutlineChoice(null);
    } //Choice Event

    public void DoChoiceUnReady()
    {
        m_choice = false;
        if (ManaFull)
            DoEffectOutlineMana(null);
        else
            DoEffectOutlineNormal(null);
    }

    public void DoChoiceOnce(bool Stage)
    {
        m_choiceOnce = Stage;
    }


    public void DoTutorialReady()
    {
        m_tutorial = true;
        m_renderer.transform.DOScale(Vector2.one * 1.02f, 0.2f).SetLoops(-1, LoopType.Yoyo);
    }

    public void DoTutorialUnReady()
    {
        m_tutorial = false;
        m_renderer.transform.DOKill();
        m_renderer.transform.localScale = Vector3.one;
    }


    public void DoFlipOpen(Action OnComplete)
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
                        m_flip = false;
                        m_open = true;
                        OnComplete?.Invoke();
                    });
            });
    } //Flip Event

    public void DoFlipClose(Action OnComplete)
    {
        if (m_flip)
            Debug.Log("Card close not done yet");
        m_flip = true;
        m_open = false;

        var FlipDuration = GameManager.instance.TweenConfig.CardAction.FlipDuration;

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


    public void DoMoveTop(Action OnComplete)
    {
        if (m_top || m_move)
            Debug.Log("Card move (top) not done yet");
        m_top = true;
        m_move = true;

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        transform.SetParent(PlayerView.instance.InfoView, true);
        Renderer.maskable = false;
        m_tmpDamage.maskable = false;
        m_tmpGrowth.maskable = false;
        m_tmpMana.maskable = false;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one * 2.35f, MoveDuration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalMove(Vector3.zero, MoveDuration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            OnComplete?.Invoke();
        });
        CardTween.Play();
    } //Move Event

    public void DoMoveBack(Action OnComplete)
    {
        if (!m_top || m_move)
            Debug.Log("Card move (back) not done yet");
        m_top = false;
        m_move = true;

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        transform.SetParent(Pointer, true);
        transform.SetSiblingIndex(Pointer.childCount - 1);

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one, MoveDuration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalMove(CentreInPointer, MoveDuration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            transform.SetSiblingIndex(0);
            Renderer.maskable = true;
            m_tmpDamage.maskable = true;
            m_tmpGrowth.maskable = true;
            m_tmpMana.maskable = true;
            OnComplete?.Invoke();
        });
        CardTween.Play();
    }

    public void DoMoveCentreLinear(RectTransform Centre, Action OnComplete)
    {
        if (!m_top || m_move)
            Debug.Log("Card move (linear) not done yet");
        this.Centre = Centre;
        m_move = true;

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        transform.SetSiblingIndex(0);
        Renderer.maskable = false;

        transform
            .DOLocalMove(CentreInPointer, MoveDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_move = false;
                transform.SetSiblingIndex(0);
                OnComplete?.Invoke();
            });
    }

    public void DoMoveCentreJump(RectTransform Centre, Action OnComplete)
    {
        if (!m_top || m_move)
            Debug.Log("Card move (back) not done yet");
        this.Centre = Centre;
        m_move = true;

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        transform.SetSiblingIndex(Pointer.childCount - 2);
        Renderer.maskable = false;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one * 1.35f, MoveDuration * 0.5f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalJump(CentreInPointer, 50, 1, MoveDuration).SetEase(Ease.Linear));
        CardTween.Insert(MoveDuration * 0.5f, transform.DOScale(Vector3.one, MoveDuration * 0.5f).SetEase(Ease.InCirc));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            transform.SetSiblingIndex(0);
            Renderer.maskable = true;
            OnComplete?.Invoke();
        });
        CardTween.Play();
    }


    public void DoRumble(Action OnComplete)
    {
        if (m_effect)
            Debug.Log("Card rumble not done yet");
        m_effect = true;

        var RumbleDuration = GameManager.instance.TweenConfig.CardAction.RumbleDuration;

        transform.SetAsLastSibling();
        Renderer.maskable = false;
        m_tmpDamage.maskable = false;
        m_tmpGrowth.maskable = false;
        m_tmpMana.maskable = false;
        Renderer.transform.DOScale(Vector3.one * 1.35f, RumbleDuration * 0.8f).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            Renderer.transform.DOScale(Vector3.one, RumbleDuration * 0.2f).SetEase(Ease.Linear).OnComplete((() =>
            {
                m_effect = false;
                GameEvent.CardRumble(this, () =>
                {
                    Renderer.maskable = true;
                    m_tmpDamage.maskable = true;
                    m_tmpGrowth.maskable = true;
                    m_tmpMana.maskable = true;
                    OnComplete?.Invoke();
                });
            }));
        }));

        if (Player.CardStaffCurrent.Equals(this))
            Player.DoStaffRumble(null);
    } //Rumble Event


    public void DoEffectAlpha(Action OnComplete)
    {
        m_effect = true;

        var AlphaDuration = GameManager.instance.TweenConfig.CardAction.AlphaDuration;

        var AlphaGroup = m_rendererAlpha.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(1f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete((() =>
            {
                m_effect = false;
                OnComplete?.Invoke();
            }));
        }));
    } //Effect Event

    public void DoEffectOutlineNormal(Action OnComplete)
    {
        m_effect = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 2f, OutlineDuration);
        m_outline.DOColor(Color.black, OutlineDuration).OnComplete((() =>
        {
            m_effect = false;
            OnComplete?.Invoke();
        }));
    }

    public void DoEffectOutlineMana(Action OnComplete)
    {
        m_effect = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.cyan, OutlineDuration).OnComplete((() =>
        {
            m_effect = false;
            OnComplete?.Invoke();
        }));
    }

    public void DoEffectOutlineChoice(Action OnComplete)
    {
        m_effect = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.green, OutlineDuration).OnComplete((() =>
        {
            m_effect = false;
            OnComplete?.Invoke();
        }));
    }

    public void DoEffectOutlineProgess(Action OnComplete)
    {
        m_effect = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.red, OutlineDuration).OnComplete((() =>
        {
            m_effect = false;
            OnComplete?.Invoke();
        }));
    }


    public void DoEffectOrigin(Action OnComplete)
    {
        m_effect = true;

        var AlphaDuration = GameManager.instance.TweenConfig.CardAction.AlphaDuration;

        var AlphaGroup = m_originIcon.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(1f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete((() =>
            {
                m_effect = false;
                OnComplete?.Invoke();
            }));
        }));
    } //Effect Origin Event

    public void DoEffectClass(Action OnComplete)
    {
        m_effect = true;

        var AlphaDuration = GameManager.instance.TweenConfig.CardAction.AlphaDuration;

        var AlphaGroup = m_classIcon.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(1f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete((() =>
            {
                m_effect = false;
                OnComplete?.Invoke();
            }));
        }));
    }  //Effect Class Event


    public void DoTextAttack()
    {
        if (AttackCombine == 0)
            return;
        var TmpClone = Instantiate(m_tmpDamage, m_tmpDamage.transform.parent);
        TmpClone.GetComponent<TextMeshProUGUI>().DOFade(0f, 1f).SetDelay(0.5f);
        TmpClone.transform.position = m_tmpDamage.transform.position;
        TmpClone.transform.localScale = Vector3.one;
        TmpClone.gameObject.SetActive(true);
        var TmpRecTransform = TmpClone.GetComponent<RectTransform>();
        TmpRecTransform.anchoredPosition += Vector2.up * 25;
        TmpRecTransform.DOScale(Vector2.one * 3f, 0.1f).OnComplete(() => TmpRecTransform.DOScale(Vector2.one, 0.1f));
        TmpRecTransform.DOAnchorPosY(TmpRecTransform.anchoredPosition.y + 50f, 1.5f).OnComplete(() => Destroy(TmpClone));
    }


    public void InfoShow(bool Show)
    {
        m_tmpGrowth.gameObject.SetActive(Type == CardType.Mons && Show);
        m_tmpMana.gameObject.SetActive(Type == CardType.Mons && Show);
        m_tmpDamage.gameObject.SetActive(Type == CardType.Mons && Show);
    } //Info Event

    private void InfoGrowUpdate(int Value, Action OnComplete)
    {
        m_tmpGrowth.transform.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            var ValueText = Value.ToString() + GameConstant.TMP_ICON_GROW;
            m_tmpGrowth.text = ValueText;
            m_tmpGrowth.transform.DOScale(Vector2.one, 0.1f).OnComplete(() =>
            {
                DoEffectOutlineMana(() => DoEffectOutlineNormal(OnComplete));
            });
        });
        InfoDamageUpdate(AttackCombine, null);
    }

    private void InfoManaUpdate(int Value, int Max, Action OnComplete)
    {
        DoEffectOutlineMana(() =>
        {
            DoEffectOutlineNormal(() =>
            {
                m_tmpMana.transform.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
                {
                    var ValueText = string.Format("{0}/{1} {2}", Value, Max, GameConstant.TMP_ICON_Mana);
                    m_tmpMana.text = ValueText;
                    m_tmpMana.transform.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
                });
            });
            m_manaGroup.InfoManaUpdate(Value);
        });
    }

    private void InfoDamageUpdate(int Value, Action OnComplete)
    {
        m_tmpDamage.transform.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            var ValueText = GameConstant.TMP_ICON_DAMAGE + " " + Value.ToString();
            m_tmpDamage.text = ValueText;
            m_tmpDamage.transform.DOScale(Vector2.one, 0.1f).OnComplete(() =>
            {
                DoEffectOutlineMana(() => DoEffectOutlineNormal(OnComplete));
            });
        });
    }


    public void DoCollectActive(IPlayer Player, Action OnComplete)
    {
        m_player = Player;
        //
        m_runeStoneIcon.SetActive(m_runeStoneTake > 0);
        var RuneStoneIconScale = m_runeStoneIcon.transform.localScale;
        m_runeStoneIcon.transform.DOScale(RuneStoneIconScale * 1.01f, 0.1f).OnComplete(() =>
            m_runeStoneIcon.transform.DOScale(RuneStoneIconScale, 0.1f));
        //
        DoEffectAlpha(() =>
        {
            m_progessCollectCurrent = ProgessCollectType.Start;
            m_progessEvent = OnComplete;
            DoCollectProgess();
        });
    } //Collect Event

    public bool DoCollectProgess()
    {
        switch (m_progessCollectCurrent)
        {
            case ProgessCollectType.Start:
                m_progessCollectCurrent = ProgessCollectType.Origin;
                DoCollectProgess();
                return true;
            case ProgessCollectType.Origin:
                m_progessCollectCurrent = ProgessCollectType.Enter;
                DoOriginActive(() => DoCollectProgess());
                return true;
            case ProgessCollectType.Enter:
                m_progessCollectCurrent = ProgessCollectType.End;
                DoEnterActive(() => DoCollectProgess());
                return true;
            case ProgessCollectType.End:
                m_player.ProgessCardDone(this);
                m_progessCollectCurrent = ProgessCollectType.None;
                m_progessEvent?.Invoke();
                m_progessEvent = null;
                OriginDragonDice.Clear();
                OriginDragonDiceBite = 0;
                OriginDragonDiceDragon = 0;
                ClassFighterDice.Clear();
                ClassFighterDiceDot = 0;
                m_choice = false;
                m_choiceOnce = false;
                return true;
        }
        return false;
    }


    public void DoOriginActive(Action OnComplete)
    {
        DoEffectOrigin(() =>
        {
            switch (Origin)
            {
                case CardOriginType.Dragon:
                    DoOriginDragon(OnComplete);
                    break;
                case CardOriginType.Woodland:
                    DoOriginWoodlandReady(OnComplete);
                    break;
                case CardOriginType.Ghost:
                    DoOriginGhostReady(OnComplete);
                    break;
                case CardOriginType.Insects:
                    DoOriginInsect(OnComplete);
                    break;
                case CardOriginType.Siren:
                    DoOriginSiren(OnComplete);
                    break;
                case CardOriginType.Neutral:
                    DoOriginNeutral(OnComplete);
                    break;
                default:
                    OnComplete?.Invoke();
                    break;
            }
        });
    } //Origin Event

    public void DoOriginDragon(Action OnComplete)
    {
        var DragonCount = m_player.CardQueue.Count(t => t.Origin == CardOriginType.Dragon);
        DoOriginDragonProgess(DragonCount, OnComplete);
    } //Origin Dragon Event

    private void DoOriginDragonProgess(int DragonLeft, Action OnComplete)
    {
        var DiceQueue = GameManager.instance.DiceConfig.Data;
        var DiceCount = DiceQueue.Count;
        var DiceIndex = UnityEngine.Random.Range(0, (DiceCount * 10) - 1) / 10;
        var DiceFace = DiceQueue[DiceIndex];

        DragonLeft--;
        OriginDragonDice.Add(DiceFace);
        OriginDragonDiceBite += DiceFace.Bite;
        OriginDragonDiceDragon += DiceFace.Dragon;

        if (DragonLeft > 0)
        {
            DoOriginDragonProgess(DragonLeft, OnComplete);
            return;
        }

        GameEvent.OriginDragon(OriginDragonDice.ToArray(), () => DoRumble(() =>
        {
            var DiceBite = OriginDragonDiceBite;
            var DiceDragon = OriginDragonDiceDragon;
            var PlayerQueue = GameManager.instance.PlayerQueue;
            for (int i = 0; i < PlayerQueue.Length; i++)
            {
                if (PlayerQueue[i].Equals(m_player))
                    PlayerQueue[i].HealthChange(-DiceBite, () => OnComplete?.Invoke());
                else
                    PlayerQueue[i].HealthChange(-DiceDragon, null);
            }
        }));
    }

    public void DoOriginWoodlandReady(Action OnComplete)
    {
        bool ProgessReady = false;
        var WoodlandCount = m_player.CardQueue.Count(t => t.Origin == CardOriginType.Woodland);
        var ManaGainValue = 1.0f * WoodlandCount / 2 + (WoodlandCount % 2 == 0 ? 0 : 0.5f);
        for (int i = 0; i < m_player.CardQueue.Length; i++)
        {
            var CardCheck = m_player.CardQueue[i];
            if (CardCheck.Type != CardType.Mons)
                continue;
            CardCheck.DoChoiceReady();
            ProgessReady = true;
        }
        if (ProgessReady)
        {
            m_player.ProgessMana = Mathf.Max(1, (int)ManaGainValue);
            m_player.ProgessCardChoice = this;
            m_player.ProgessCard(this);
            GameManager.instance.CardOriginWoodlandChoice(this);
        }
        else
            OnComplete?.Invoke();
    } //Origin Woodland Event

    public void DoOriginWoodlandStart()
    {
        m_player.ProgessMana -= 1;

        if (m_player.ProgessMana > 0)
        {
            foreach (var Card in m_player.CardQueue)
                Card.DoChoiceUnReady();
            DoChoiceOnce(true);
        }
        else
        {
            foreach (var Card in m_player.CardQueue)
            {
                Card.DoChoiceUnReady();
                Card.DoChoiceOnce(false);
            }
        }

        DoEffectAlpha(() => DoManaChange(1, () =>
        {
            if (m_player.ProgessMana > 0)
            {
                for (int i = 0; i < m_player.CardQueue.Length; i++)
                {
                    var CardCheck = m_player.CardQueue[i];
                    if (CardCheck.Type != CardType.Mons)
                        continue;
                    CardCheck.DoChoiceReady();
                }
                GameManager.instance.CardOriginWoodlandChoice(m_player.ProgessCardChoice);
            }
            else
            {
                m_player.ProgessCardChoice = null;
                m_player.ProgessCheck();
            }
        }));
    }

    public void DoOriginGhostReady(Action OnComplete)
    {
        bool ProgessReady = false;
        var GhostCount = m_player.CardQueue.ToArray().Count(t => t.Origin == CardOriginType.Ghost);
        var StaffCurrent = m_player.StaffStep;
        var StaffAvaible = GhostCount;
        while (StaffAvaible > 0)
        {
            StaffCurrent++;
            if (StaffCurrent > m_player.CardQueue.Length - 1)
                StaffCurrent = 0;
            m_player.CardQueue[StaffCurrent].DoChoiceReady();
            ProgessReady = true;
            StaffAvaible--;
        }
        if (ProgessReady)
        {
            m_player.ProgessCardChoice = this;
            m_player.ProgessCard(this);
            GameManager.instance.CardOriginGhostChoice(this);
        }
        else
            OnComplete?.Invoke();
    } //Origin Ghost Event

    public void DoOriginGhostStart()
    {
        foreach (var Card in m_player.CardQueue)
            Card.DoChoiceUnReady();
        DoEffectAlpha(() => DoOriginGhostProgess());
    }

    private void DoOriginGhostProgess()
    {
        m_player.DoStaffNext(() =>
        {
            if (m_player.CardStaffCurrent.Equals(this))
                //Active staff when land on card choosed
                m_player.DoStaffActive(() =>
                {
                    m_player.ProgessCardChoice = null;
                    m_player.ProgessCheck();
                });
            else
                //Continue move staff if not land on chossed card
                DoOriginGhostProgess();
        });
    }

    public void DoOriginInsect(Action OnComplete) { } //Origin Insect Event

    public void DoOriginSiren(Action OnComplete) { } //Origin Siren Event

    public void DoOriginNeutral(Action OnComplete)
    {
        OnComplete?.Invoke();
    } //Origin Neutral Event


    public void DoEnterActive(Action OnComplete)
    {
        switch (m_name)
        {
            case CardNameType.FlowOfTheEssential:
                DoEffectAlpha(() => DoEnterActiveFlowOfTheEssentialReady(OnComplete));
                break;
            case CardNameType.Pott:
                DoEffectAlpha(() => DoEnterActivePottReady(OnComplete));
                break;
            default:
                OnComplete?.Invoke();
                break;
        }
    } //Enter Event

    private void DoEnterActiveFlowOfTheEssentialReady(Action OnComplete)
    {
        bool ProgessReady = false;
        for (int i = 0; i < m_player.CardQueue.Length; i++)
        {
            var CardCheck = m_player.CardQueue[i];
            if (CardCheck.Equals(this) || CardCheck.Type != CardType.Mons)
                continue;
            CardCheck.DoChoiceReady();
            ProgessReady = true;
        }
        if (ProgessReady)
        {
            m_player.ProgessCardChoice = this;
            m_player.ProgessCard(this);
            GameManager.instance.CardEnterChoice(this);
        }
        else
            OnComplete?.Invoke();
    }

    private void DoEnterActivePottReady(Action OnComplete)
    {
        bool ProgessReady = false;
        for (int i = 0; i < m_player.CardQueue.Length; i++)
        {
            var CardCheck = m_player.CardQueue[i];
            if (CardCheck.Type == CardType.None || CardCheck.Equals(this))
                continue;
            CardCheck.DoChoiceReady();
            ProgessReady = true;
        }
        if (ProgessReady)
        {
            m_player.ProgessCardChoice = this;
            m_player.ProgessCard(this);
            GameManager.instance.CardEnterChoice(this);
        }
        else
            OnComplete?.Invoke();
    }

    public void DoEnterStart()
    {
        foreach (var Card in m_player.CardQueue)
            Card.DoChoiceUnReady();

        DoEffectAlpha(() =>
        {
            switch (m_player.ProgessCardChoice.Name)
            {
                case CardNameType.FlowOfTheEssential:
                    DoEnterStartFlowOfTheEssential();
                    break;
                case CardNameType.Pott:
                    DoEnterStartPott();
                    break;
            }
        });
    }

    private void DoEnterStartFlowOfTheEssential()
    {
        DoGrowthChange(1, () =>
        {
            m_player.ProgessCardChoice = null;
            m_player.ProgessCheck();
        });
    }

    private void DoEnterStartPott()
    {
        DoEffectOrigin(() =>
        {
            switch (Origin)
            {
                case CardOriginType.Dragon:
                    DoOriginDragon(() =>
                    {
                        m_player.ProgessCardChoice = null;
                        m_player.ProgessCheck();
                    });
                    break;
                case CardOriginType.Woodland:
                    DoOriginWoodlandReady(() =>
                    {
                        m_player.ProgessCardChoice = null;
                        m_player.ProgessCheck();
                    });
                    break;
                case CardOriginType.Ghost:
                    DoOriginGhostReady(() =>
                    {
                        m_player.ProgessCardChoice = null;
                        m_player.ProgessCheck();
                    });
                    break;
                case CardOriginType.Insects:
                    DoOriginInsect(() =>
                    {
                        m_player.ProgessCardChoice = null;
                        m_player.ProgessCheck();
                    });
                    break;
                case CardOriginType.Siren:
                    DoOriginSiren(() =>
                    {
                        m_player.ProgessCardChoice = null;
                        m_player.ProgessCheck();
                    });
                    break;
            }
        });
    }


    public void DoStaffActive(Action OnComplete)
    {
        if (m_name == CardNameType.Stage)
        {
            OnComplete?.Invoke();
            return;
        }

        DoStaffTakeRuneStone();

        DoEffectAlpha(() =>
        {
            switch (Type)
            {
                case CardType.Mons:
                    DoAttackActive(() => DoManaChange(1, OnComplete));
                    break;
                case CardType.Landmark:
                    DoStaffActiveSerenity(OnComplete);
                    break;
            }
        });
    } //Staff Event

    public void DoStaffTakeRuneStone()
    {
        m_player.DoStaffTakeRuneStone(m_runeStoneTake, null);
        m_runeStoneTake = 0;
        m_runeStoneIcon.SetActive(false);
    }

    public void DoStaffActiveSerenity(Action OnComplete)
    {
        switch (m_name)
        {
            case CardNameType.PixieSGrove:
                DoStaffActiveSerenityPixieSGrove(OnComplete);
                break;
        }
    } //Staff on Landmask Event

    private void DoStaffActiveSerenityPixieSGrove(Action OnComplete)
    {
        var WoodlandCount = m_player.CardQueue.Count(t => t.Origin == CardOriginType.Woodland);
        Player.HealthChange(1 * WoodlandCount, OnComplete);
    }

    public void DoAttackActive(Action OnComplete)
    {
        if (Type != CardType.Mons)
        {
            OnComplete?.Invoke();
            return;
        }
        DoRumble(() =>
        {
            DoTextAttack();
            //
            var PlayerQueue = GameManager.instance.PlayerQueue;
            for (int i = 0; i < PlayerQueue.Length; i++)
            {
                if (PlayerQueue[i] == m_player)
                    continue;
                PlayerQueue[i].HealthChange(-AttackCombine, null);
            }
            OnComplete?.Invoke();
        });
    } //Attack Event


    public void DoManaChange(int Value, Action OnComplete)
    {
        if (Type != CardType.Mons)
        {
            OnComplete?.Invoke();
            return;
        }
        m_manaCurrent += Value;
        //if (m_manaCurrent > m_manaPoint)
        //    m_manaCurrent = m_manaPoint;
        if (ManaFull)
            InfoManaUpdate(m_manaCurrent, m_manaPoint, () => DoEffectOutlineMana(OnComplete));
        else
            InfoManaUpdate(m_manaCurrent, m_manaPoint, OnComplete);
    }

    public void DoGrowthChange(int Value, Action OnComplete)
    {
        if (Type != CardType.Mons)
        {
            OnComplete?.Invoke();
            return;
        }
        m_growthCurrent += Value;
        InfoGrowUpdate(m_growthCurrent, () => OnComplete());
    }


    public void DoManaActive(Action OnComplete)
    {
        if (Type != CardType.Mons)
        {
            OnComplete?.Invoke();
            return;
        }
        m_manaCurrent -= m_manaPoint;
        InfoManaUpdate(m_manaCurrent, m_manaPoint, () => DoEffectOutlineNormal(() =>
        {
            m_progessManaCurrent = ProgessManaType.Start;
            m_progessEvent = OnComplete;
            DoManaProgess();
        }));
    }

    public bool DoManaProgess()
    {
        switch (m_progessManaCurrent)
        {
            case ProgessManaType.Start:
                m_progessManaCurrent = ProgessManaType.Class;
                DoManaProgess();
                return true;
            case ProgessManaType.Class:
                m_progessManaCurrent = ProgessManaType.Spell;
                DoClassActive(() => DoManaProgess());
                return true;
            case ProgessManaType.Spell:
                m_progessManaCurrent = ProgessManaType.End;
                DoSpellActive(() => DoManaProgess());
                return true;
            case ProgessManaType.End:
                m_player.ProgessCardDone(this);
                m_progessManaCurrent = ProgessManaType.None;
                m_progessEvent?.Invoke();
                m_progessEvent = null;
                OriginDragonDice.Clear();
                OriginDragonDiceBite = 0;
                OriginDragonDiceDragon = 0;
                ClassFighterDice.Clear();
                ClassFighterDiceDot = 0;
                m_choice = false;
                m_choiceOnce = false;
                return true;
        }
        return false;
    }


    public void DoClassActive(Action OnComplete)
    {
        DoEffectClass(() =>
        {
            switch (Class)
            {
                case CardClassType.Fighter:
                    DoClassFighter(OnComplete);
                    break;
                case CardClassType.MagicAddict:
                    DoClassMagicAddictReady(OnComplete);
                    break;
                case CardClassType.Singer:
                    DoClassSinger(OnComplete);
                    break;
                case CardClassType.Caretaker:
                    DoClassCareTaker(OnComplete);
                    break;
                case CardClassType.Diffuser:
                    DoClassDiffuser(OnComplete);
                    break;
                case CardClassType.Flying:
                    DoClassFlyingReady(OnComplete);
                    break;
                default:
                    OnComplete?.Invoke();
                    break;
            }
        });
    } //Class Event

    public void DoClassFighter(Action OnComplete)
    {
        DoClassFighterProgess(AttackCombine + this.GetPassiveEversor(), OnComplete);
    } //Class Fighter Event

    private void DoClassFighterProgess(int AttackCombineLeft, Action OnComplete)
    {
        var DiceQueue = GameManager.instance.DiceConfig.Data;
        var DiceCount = DiceQueue.Count;
        var DiceIndex = UnityEngine.Random.Range(0, (DiceCount * 10) - 1) / 10;
        var DiceFace = DiceQueue[DiceIndex];

        AttackCombineLeft--;
        ClassFighterDice.Add(DiceFace);
        ClassFighterDiceDot += DiceFace.Dot;

        if (AttackCombineLeft > 0)
        {
            DoClassFighterProgess(AttackCombineLeft, OnComplete);
            return;
        }

        GameEvent.ClassFighter(ClassFighterDice.ToArray(), () => DoRumble(() =>
        {
            var DiceDotSumAttack = (int)(ClassFighterDiceDot / 2);
            var OnCompleteEvent = false;
            var PlayerQueue = GameManager.instance.PlayerQueue;
            for (int i = 0; i < PlayerQueue.Length; i++)
            {
                if (PlayerQueue[i].Equals(m_player))
                    continue;

                if (!OnCompleteEvent)
                {
                    OnCompleteEvent = true;
                    PlayerQueue[i].HealthChange(-DiceDotSumAttack, OnComplete);
                }
                else
                    PlayerQueue[i].HealthChange(-DiceDotSumAttack, null);
            }
        }));
    }

    public void DoClassMagicAddictReady(Action OnComplete)
    {
        bool ProgessReady = false;
        for (int i = 0; i < m_player.CardQueue.Length; i++)
        {
            var CardCheck = m_player.CardQueue[i];
            if (CardCheck.Equals(this) || CardCheck.ManaCurrent == 0)
                continue;
            ProgessReady = true;
            CardCheck.DoChoiceReady();
        }
        if (ProgessReady)
        {
            //Found monster(s) got mana for this monster cast spell once more time
            m_player.ProgessCardChoice = this;
            m_player.ProgessCard(this);
            GameManager.instance.CardClassMagicAddictChoice(this);
        }
        else
            //If no monster got mana for this monster cast spell once more time, skip this
            OnComplete?.Invoke();
    } //Class Magic Addict Event

    public void DoClassMagicAddictStart()
    {
        foreach (var Card in m_player.CardQueue)
            Card.DoChoiceUnReady();

        DoEffectAlpha(() => DoManaChange(-1, () =>
        {
            m_player.ProgessCardChoice.DoSpellActive(() =>
            {
                var CardChoice = m_player.ProgessCardChoice;
                m_player.ProgessCardChoice = null;
                m_player.ProgessCheck(CardChoice);
            });
        }));
    }

    public void DoClassSinger(Action OnComplete)
    {
        var SingerCount = m_player.CardQueue.ToArray().Count(t => t.Class == CardClassType.Singer);
        DoRumble(() =>
        {
            var OnCompleteEvent = false;
            var PlayerQueue = GameManager.instance.PlayerQueue;
            for (int i = 0; i < PlayerQueue.Length; i++)
            {
                if (PlayerQueue[i].Equals(m_player))
                    continue;

                if (!OnCompleteEvent)
                {
                    OnCompleteEvent = true;
                    PlayerQueue[i].HealthChange(-SingerCount, OnComplete);
                }
                else
                    PlayerQueue[i].HealthChange(-SingerCount, null);
            }
        });
    } //Class Singer Event

    public void DoClassCareTaker(Action OnComplete) { } //Class Care Taker Event

    public void DoClassDiffuser(Action OnComplete)
    {
        var CardCurrentIndex = m_player.CardQueue.ToList().IndexOf(this);
        var CardL = CardCurrentIndex - 1 >= 0 ? m_player.CardQueue[CardCurrentIndex - 1] : null;
        var CardR = CardCurrentIndex + 1 <= m_player.CardQueue.Length - 1 ? m_player.CardQueue[CardCurrentIndex + 1] : null;

        DoRumble(() =>
        {
            DoEffectOutlineMana(() => DoEffectOutlineNormal(OnComplete));
            if (CardL != null)
                CardL.DoManaChange(1, null);
            if (CardR != null)
                CardR.DoManaChange(1, null);
        });
    } //Class Diffuser Event

    public void DoClassFlyingReady(Action OnComplete)
    {
        bool ProgessReady = false;
        var CardStageIndexMin = -1;
        while (m_player.CardQueue[CardStageIndexMin + 1].Name == CardNameType.Stage)
            CardStageIndexMin++;
        var CardIndex = m_player.CardQueue.ToList().IndexOf(this);
        for (int i = 0; i < m_player.CardQueue.Length; i++)
        {
            if (m_player.CardQueue[i].Equals(this) || Mathf.Abs(CardIndex - i) <= 1 || i <= CardStageIndexMin)
                continue;
            m_player.CardQueue[i].DoChoiceReady();
            ProgessReady = true;
        }
        if (ProgessReady)
        {
            m_player.ProgessCardChoice = this;
            m_player.ProgessCard(this);
            GameManager.instance.CardClassFlyingChoice(this);
        }
        else
            OnComplete?.Invoke();
    } //Class Flying Event

    public void DoClassFlyingStart()
    {
        foreach (var Card in m_player.CardQueue)
            Card.DoChoiceUnReady();

        DoEffectAlpha(() =>
        {
            var CardFromIndex = m_player.CardQueue.ToList().IndexOf(m_player.ProgessCardChoice);
            var CardToIndex = m_player.CardQueue.ToList().IndexOf(this);
            var CardMoveDir = CardFromIndex < CardToIndex ? 1 : -1;
            m_player.ProgessCardChoice.ClassFlyingMoveDir = CardMoveDir;
            m_player.DoClassFlyingProgess(CardFromIndex, CardToIndex + (CardMoveDir * -1), () =>
             {
                 m_player.ProgessCardChoice.DoRumble(() => DoManaChange(1, () =>
                 {
                     if (this.ManaFull)
                         this.ClassFlyingManaFull = true;
                     var CardChoice = m_player.ProgessCardChoice;
                     m_player.ProgessCardChoice = null;
                     m_player.ProgessCheck(CardChoice);
                 }));
             });
        });
    }


    public void DoSpellActive(Action OnComplete)
    {
        DoEffectAlpha(() =>
        {
            switch (m_name)
            {
                case CardNameType.Cornibus:
                    DoSpellCornibusReady(OnComplete);
                    break;
                case CardNameType.Duchess:
                    DoAttackActive(OnComplete);
                    break;
                case CardNameType.DragonEgg:
                    Player.HealthChange(3, OnComplete);
                    break;
                case CardNameType.Eversor:
                    Player.HealthChange(ClassFighterDiceDot / 4, () => OnComplete?.Invoke());
                    break;
                case CardNameType.FlowOfTheEssential:
                    DoSpellFlowOfTheEssential(OnComplete);
                    break;
                case CardNameType.Forestwing:
                    DoSpellForestwing(OnComplete);
                    break;
                case CardNameType.OneTail:
                    DoGrowthChange(1, () => DoAttackActive(OnComplete));
                    break;
                case CardNameType.Pott:
                    DoSpellPottReady(OnComplete);
                    break;
                case CardNameType.Umbella:
                    DoSpellUmbella(OnComplete);
                    break;
            }
        });
    } //Spell Event

    private void DoSpellCornibusReady(Action OnComplete)
    {
        bool ProgessReady = false;
        for (int i = 0; i < m_player.CardQueue.Length; i++)
        {
            var CardCheck = m_player.CardQueue[i];
            if (CardCheck.Equals(this) || (CardCheck.Origin != CardOriginType.Woodland && CardCheck.Class != CardClassType.Singer) || CardCheck.Type != CardType.Mons)
                continue;
            CardCheck.DoChoiceReady();
            ProgessReady = true;
        }
        if (ProgessReady)
        {
            m_player.ProgessCardChoice = this;
            m_player.ProgessCard(this);
            GameManager.instance.CardSpellChoice(this);
        }
        else
            OnComplete?.Invoke();
    }

    private void DoSpellFlowOfTheEssential(Action OnComplete)
    {
        var CardCurrentIndex = m_player.CardQueue.ToList().IndexOf(this);
        var CardL = CardCurrentIndex - 1 >= 0 ? m_player.CardQueue[CardCurrentIndex - 1] : null;
        var CardR = CardCurrentIndex + 1 <= m_player.CardQueue.Length - 1 ? m_player.CardQueue[CardCurrentIndex + 1] : null;
        var ManaFullCount = 0;
        if (CardL != null ? CardL.ManaFull : false)
            ManaFullCount++;
        if (CardR != null ? CardR.ManaFull : false)
            ManaFullCount++;
        DoGrowthChange(ManaFullCount, OnComplete);
    }

    private void DoSpellForestwing(Action OnComplete)
    {
        var CardCurrentIndex = m_player.CardQueue.ToList().IndexOf(this);
        var CardCheckIndex = CardCurrentIndex + this.ClassFlyingMoveDir;
        if (CardCheckIndex >= 0 && CardCheckIndex <= m_player.CardQueue.Length - 1)
        {
            var Card = m_player.CardQueue[CardCheckIndex];
            if (Card.ClassFlyingManaFull)
                Card.DoAttackActive(() =>
                {
                    this.ClassFlyingMoveDir = 0;
                    Card.ClassFlyingManaFull = false;
                    OnComplete?.Invoke();
                });
            else
            {
                this.ClassFlyingMoveDir = 0;
                Card.ClassFlyingManaFull = false;
                OnComplete?.Invoke();
            }
        }
        else
        {
            this.ClassFlyingMoveDir = 0;
            OnComplete?.Invoke();
        }
    }

    private void DoSpellPottReady(Action OnComplete)
    {
        bool ProgessReady = false;
        for (int i = 0; i < m_player.CardQueue.Length; i++)
        {
            var CardCheck = m_player.CardQueue[i];
            if (CardCheck.Type != CardType.Mons)
                continue;
            CardCheck.DoChoiceReady();
            ProgessReady = true;
        }
        if (ProgessReady)
        {
            m_player.ProgessCardChoice = this;
            m_player.ProgessCard(this);
            GameManager.instance.CardSpellChoice(this);
        }
        else
            OnComplete?.Invoke();
    }

    private void DoSpellUmbella(Action OnComplete)
    {
        var CardCurrentIndex = m_player.CardQueue.ToList().IndexOf(this);
        var CardL = CardCurrentIndex - 1 >= 0 ? m_player.CardQueue[CardCurrentIndex - 1] : null;
        var CardR = CardCurrentIndex + 1 <= m_player.CardQueue.Length - 1 ? m_player.CardQueue[CardCurrentIndex + 1] : null;
        var ManaFullCount = 0;
        if (CardL != null ? CardL.ManaFull : false)
            ManaFullCount++;
        if (CardR != null ? CardR.ManaFull : false)
            ManaFullCount++;
        Player.HealthChange(2 * ManaFullCount, OnComplete);
    }

    public void DoSpellStart()
    {
        foreach (var Card in m_player.CardQueue)
            Card.DoChoiceUnReady();

        switch (m_player.ProgessCardChoice.Name)
        {
            case CardNameType.Cornibus:
                DoManaChange(1, () =>
                {
                    m_player.ProgessCardChoice = null;
                    m_player.ProgessCheck();
                });
                break;
            case CardNameType.Pott:
                DoManaChange(1, () =>
                {
                    m_player.ProgessCardChoice = null;
                    m_player.ProgessCheck();
                });
                break;
        }
    }


    private int GetPassiveEversor()
    {
        return this.m_name == CardNameType.Eversor ? 1 : 0;
    }
}