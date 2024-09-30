using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour, ICard
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
                    Rumble(null);
                else
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
            case ChoiceType.CardClassFlying:
                if (Player != GameManager.instance.PlayerCurrent || !m_classFlyingReady)
                    return;
                GameEvent.UiInfoClassFlying(this);
                break;
        }
    }

    //ICard

    public IPlayer Player => m_data.Player;


    public CardNameType Name => m_data.Name;

    public CardOriginType Origin => m_data.Origin;

    public CardClassType Class => m_data.Class;

    public int RuneStoneCost => m_data.RuneStoneCost;

    public int RuneStoneTake { get; private set; }

    public int ManaPoint => m_data.ManaPoint;

    public int Mana { get; private set; }

    public bool ManaFull => m_data.ManaPoint > 0 && Mana >= m_data.ManaPoint;

    public int Attack => m_data.AttackPoint;

    public int Growth { get; private set; }

    public int AttackCombine => m_data.AttackPoint + Growth;


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

    public void Fill(RectTransform Point, RectTransform Centre)
    {
        this.Pointer(Point, Centre, false, false);

        var DurationMove = GameManager.instance.TweenConfig.WildFill.MoveDuration;
        var EaseMove = GameManager.instance.TweenConfig.WildFill.MoveEase;

        transform.SetParent(Point, true);
        transform.GetComponent<RectTransform>().DOLocalMove(CentreInPointer, DurationMove).SetEase(EaseMove);

        FlipOpen(null);
    }


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
        Renderer.maskable = false;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one * 2.35f, MoveDuration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalMove(Vector3.zero, MoveDuration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            Renderer.maskable = true;
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
        Renderer.maskable = false;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one, MoveDuration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalMove(CentreInPointer, MoveDuration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            transform.SetSiblingIndex(m_centre.GetSiblingIndex());
            Renderer.maskable = true;
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


    public void EffectOrigin(Action OnComplete)
    {
        m_effectOrigin = true;

        var AlphaDuration = GameManager.instance.TweenConfig.CardAction.AlphaDuration;

        var AlphaGroup = m_originIcon.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(1f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_effectOrigin = false;
                OnComplete?.Invoke();
            });
        });
    }

    public void EffectClass(Action OnComplete)
    {
        m_effectClass = true;

        var AlphaDuration = GameManager.instance.TweenConfig.CardAction.AlphaDuration;

        var AlphaGroup = m_classIcon.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(1f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_effectClass = false;
                OnComplete?.Invoke();
            });
        });
    }


    public void InfoShow(bool Show)
    {
        m_tmpGrowth.gameObject.SetActive(Show);
        m_tmpMana.gameObject.SetActive(Show);
        m_tmpDamage.gameObject.SetActive(Show);
    }

    private void InfoGrowUpdate(int Value, Action OnComplete)
    {
        m_tmpGrowth.transform.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            var ValueText = Value.ToString() + GameConstant.TMP_ICON_GROW;
            m_tmpGrowth.text = ValueText;
            m_tmpGrowth.transform.DOScale(Vector2.one, 0.1f).OnComplete(() =>
            {
                EffectOutlineMana(() => EffectOutlineNormal(OnComplete));
            });
        });
        InfoDamageUpdate(AttackCombine, null);
    }

    private void InfoManaUpdate(int Value, int Max, Action OnComplete)
    {
        EffectOutlineMana(() => EffectOutlineNormal(() =>
        {
            m_tmpMana.transform.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
            {
                var ValueText = string.Format("{0}/{1} {2}", Mana, ManaPoint, GameConstant.TMP_ICON_Mana);
                m_tmpMana.text = ValueText;
                m_tmpMana.transform.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
            });
        }));
    }

    private void InfoDamageUpdate(int Value, Action OnComplete)
    {
        m_tmpDamage.transform.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            var ValueText = GameConstant.TMP_ICON_DAMAGE + " " + Value.ToString();
            m_tmpDamage.text = ValueText;
            m_tmpDamage.transform.DOScale(Vector2.one, 0.1f).OnComplete(() =>
            {
                EffectOutlineMana(() => EffectOutlineNormal(OnComplete));
            });
        });
    }


    public void DoCollectActive(IPlayer Player, Action OnComplete)
    {
        m_data.Player = Player;
        EffectAlpha(() =>
        {
            DoOriginActive(() =>
            {
                DoEnterActive(() =>
                {
                    DoPassiveActive(() =>
                    {
                        OnComplete?.Invoke();
                    });
                });
            });
        });
    } //Collect Event


    public void DoOriginActive(Action OnComplete)
    {
        EffectOrigin(() =>
        {
            switch (Origin)
            {
                case CardOriginType.Dragon:
                    var DragonCount = Player.CardQueue.Count(t => t.Origin == CardOriginType.Dragon);
                    DoOriginDragonActive(DragonCount, OnComplete);
                    break;
                case CardOriginType.Woodland:
                    var WoodlandCount = Player.CardQueue.Count(t => t.Origin == CardOriginType.Woodland);
                    DoOriginWoodlandActive(WoodlandCount, OnComplete);
                    break;
                case CardOriginType.Ghost:
                    var GhostCount = Player.CardQueue.Count(t => t.Origin == CardOriginType.Ghost);
                    DoOriginGhostActive(GhostCount);
                    break;
                case CardOriginType.Insects:
                    DoOriginInsectActive(OnComplete);
                    break;
                case CardOriginType.Siren:
                    DoOriginSirenActive(OnComplete);
                    break;
                case CardOriginType.Neutral:
                    DoOriginNeutralActive(OnComplete);
                    break;
                default:
                    OnComplete?.Invoke();
                    break;
            }
        });
    } //Origin Event


    public void DoOriginDragonActive(int DragonCount, Action OnComplete)
    {
        DoOriginDragonProgess(DragonCount, OnComplete);
    } //Origin Dragon Event

    private void DoOriginDragonProgess(int DragonLeft, Action OnComplete)
    {
        var DiceQueue = GameManager.instance.DiceConfig.Data;
        var DiceCount = DiceQueue.Count;
        var DiceIndex = UnityEngine.Random.Range(0, (DiceCount * 10) - 1) / 10;
        var DiceFace = DiceQueue[DiceIndex];

        DragonLeft--;

        GameEvent.OriginDragon(() =>
        {
            Rumble(() =>
            {
                var PlayerQueue = GameManager.instance.PlayerQueue;
                for (int i = 0; i < PlayerQueue.Length; i++)
                {
                    if (PlayerQueue[i].Equals(this.Player))
                        PlayerQueue[i].HealthChange(-DiceFace.Bite, () =>
                        {
                            if (DragonLeft > 0)
                                DoOriginDragonProgess(DragonLeft, OnComplete);
                            else
                                OnComplete?.Invoke();
                        });
                    else
                        PlayerQueue[i].HealthChange(-DiceFace.Dragon, null);
                }
            });
        });
    }


    public void DoOriginGhostActive(int GhostCount)
    {
        var StaffCurrent = Player.StaffStep;
        var StaffAvaible = GhostCount;
        while (StaffAvaible > 0)
        {
            StaffCurrent++;
            if (StaffCurrent > Player.CardQueue.Length - 1)
                StaffCurrent = 0;
            Player.CardQueue[StaffCurrent].DoOriginGhostReady();
            StaffAvaible--;
        }
        GameManager.instance.CardOriginGhostDoChoice(this);
    } //Origin Ghost Event

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


    public void DoOriginInsectActive(Action OnComplete) { EffectOrigin(OnComplete); } //Origin Insect Event


    public void DoOriginSirenActive(Action OnComplete) { EffectOrigin(OnComplete); } //Origin Siren Event


    public void DoOriginWoodlandActive(int WoodlandCount, Action OnComplete)
    {
        var ManaGainValue = 1.0f * WoodlandCount / 2 + (WoodlandCount % 2 == 0 ? 0 : 0.5f);
        DoManaFill((int)ManaGainValue, OnComplete);
    } //Origin Woodland Event


    public void DoOriginNeutralActive(Action OnComplete)
    {
        EffectOrigin(OnComplete);
    } //Origin Neutral Active


    public void DoEnterActive(Action OnComplete)
    {
        if (onEnterActive == null)
            OnComplete?.Invoke();
        else
            EffectAlpha(() => onEnterActive.Invoke(OnComplete));
    } //Enter Event

    public void DoPassiveActive(Action OnComplete)
    {
        if (onPassiveActive == null)
            OnComplete?.Invoke();
        else
            EffectAlpha(() => onPassiveActive.Invoke(OnComplete));
    } //Passive Event


    public void DostaffActive(Action OnComplete)
    {
        EffectAlpha(() => DoAttackActive(() => DoManaFill(1, OnComplete)));
    }

    public void DoAttackActive(Action OnComplete)
    {
        Rumble(() =>
        {
            var PlayerQueue = GameManager.instance.PlayerQueue;
            for (int i = 0; i < PlayerQueue.Length; i++)
            {
                if (PlayerQueue[i] == Player)
                    continue;
                PlayerQueue[i].HealthChange(-AttackCombine, null);
            }
            OnComplete?.Invoke();
        });
    }

    public void DoManaFill(int Value, Action OnComplete)
    {
        Mana += Value;
        if (Mana > ManaPoint)
            Mana = ManaPoint;
        if (ManaFull)
            InfoManaUpdate(Mana, ManaPoint, () => EffectOutlineMana(OnComplete));
        else
            InfoManaUpdate(Mana, ManaPoint, OnComplete);
    }

    public void DoGrowthAdd(int Value, Action OnComplete)
    {
        Growth += Value;
        InfoGrowUpdate(Growth, () => OnComplete());
    }


    public void DoManaActive(Action OnComplete)
    {
        Mana = 0;
        InfoManaUpdate(Mana, ManaPoint, () => EffectOutlineNormal(() =>
        {
            DoClassActive(() => DoSpellActive(OnComplete));
        }));
    } //Invoke from GameManager


    public void DoClassActive(Action OnComplete)
    {
        EffectClass(() =>
        {
            switch (Class)
            {
                case CardClassType.Fighter:
                    DoClassFighterActive(this.AttackCombine, 0, OnComplete);
                    break;
                case CardClassType.MagicAddict:
                    DoClassMagicAddictActive(() => OnComplete?.Invoke());
                    break;
                case CardClassType.Singer:
                    var SingerCount = Player.CardQueue.Count(t => t.Class == CardClassType.Singer);
                    DoClassSingerActive(SingerCount, OnComplete);
                    break;
                case CardClassType.Caretaker:
                    DoClassCareTakerActive(OnComplete);
                    break;
                case CardClassType.Diffuser:
                    DoClassDiffuserActive(OnComplete);
                    break;
                case CardClassType.Flying:
                    DoClassFlyingActive(OnComplete);
                    break;
                default:
                    OnComplete?.Invoke();
                    break;
            }
        });
    } //Class Event


    public void DoClassFighterActive(int AttackCombineLeft, int DiceDotSumRolled, Action OnComplete)
    {
        var DiceQueue = GameManager.instance.DiceConfig.Data;
        var DiceCount = DiceQueue.Count;
        var DiceIndex = UnityEngine.Random.Range(0, (DiceCount * 10) - 1) / 10;
        var DiceFace = DiceQueue[DiceIndex];

        AttackCombineLeft--;
        DiceDotSumRolled += DiceFace.Dot;

        GameEvent.ClassFighter(() =>
        {
            if (AttackCombineLeft > 0)
                DoClassFighterActive(AttackCombineLeft, DiceDotSumRolled, OnComplete);
            else
            {
                var DiceDotSumAttack = (int)(DiceDotSumRolled / 2);
                Rumble(() =>
                {
                    var OnCompleteEvent = false;
                    var PlayerQueue = GameManager.instance.PlayerQueue;
                    for (int i = 0; i < PlayerQueue.Length; i++)
                    {
                        if (PlayerQueue[i].Equals(this.Player))
                            continue;

                        if (!OnCompleteEvent)
                        {
                            OnCompleteEvent = true;
                            PlayerQueue[i].HealthChange(-DiceDotSumAttack, OnComplete);
                        }
                        else
                            PlayerQueue[i].HealthChange(-DiceDotSumAttack, null);
                    }
                });
            }
        });

    } //Class Fighter Event


    public void DoClassMagicAddictActive(Action OnComplete)
    {
        bool CardGotMana = false;
        for (int i = 0; i < Player.CardQueue.Length; i++)
        {
            if (Player.CardQueue[i].Equals(this) || Player.CardQueue[i].Mana == 0)
                continue;
            CardGotMana = true;
            Player.CardQueue[i].DoClassMagicAddictReady();
        }
        EffectClass(() =>
        {
            if (CardGotMana)
                //Found monster(s) got mana for this monster cast spell once more time
                GameManager.instance.CardClassMagicAddictDoChoice(this);
            else
                //If no monster got mana for this monster cast spell once more time, skip this
                OnComplete?.Invoke();
        });
    } //Class Magic Addict Event

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
        EffectAlpha(() => DoManaFill(-1, () =>
        {
            Player.CardManaActiveCurrent.Player.CardManaActiveCurrent.DoSpellActive(() => Player.CardManaActiveCurrent.DoSpellActive(() =>
            {
                Player.DoCardSpecialActiveCurrent(null);
                GameManager.instance.CardManaCheck(Player);
            }));
        }));
    }

    public void DoClassMagicAddictUnReady()
    {
        m_classMagicAddictReady = false;
        if (ManaFull)
            EffectOutlineMana(null);
        else
            EffectOutlineNormal(null);
    }


    public void DoClassSingerActive(int SingerCount, Action OnComplete)
    {
        Rumble(() =>
        {
            var OnCompleteEvent = false;
            var PlayerQueue = GameManager.instance.PlayerQueue;
            for (int i = 0; i < PlayerQueue.Length; i++)
            {
                if (PlayerQueue[i].Equals(this.Player))
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


    public void DoClassCareTakerActive(Action OnComplete) { EffectClass(OnComplete); } //Class Care Taker Event


    public void DoClassDiffuserActive(Action OnComplete)
    {
        Rumble(() =>
        {
            var CardCurrentIndex = Player.CardQueue.ToList().IndexOf(this);
            var CardL = CardCurrentIndex - 1 >= 0 ? Player.CardQueue[CardCurrentIndex - 1] : null;
            var CardR = CardCurrentIndex + 1 <= Player.CardQueue.Length - 1 ? Player.CardQueue[CardCurrentIndex + 1] : null;
            EffectOutlineMana(() =>
            {
                EffectOutlineNormal(OnComplete);
                if (CardL != null)
                    CardL.DoManaFill(1, null);
                if (CardR != null)
                    CardR.DoManaFill(1, null);
            });
        });
    } //Class Diffuser Event


    public void DoClassFlyingActive(Action OnComplete)
    {
        var CardIndex = Player.CardQueue.ToList().IndexOf(this);
        for (int i = 0; i < Player.CardQueue.Length; i++)
        {
            if (Mathf.Abs(CardIndex - i) <= 1)
                continue;
            Player.CardQueue[i].DoClassFlyingReady();
        }
        GameManager.instance.CardClassFlyingDoChoice(this);
    } //Class Flying Event

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
                Rumble(() => DoManaFill(1, () =>
                {
                    Player.DoCardSpecialActiveCurrent(null);
                    GameManager.instance.CardManaCheck(Player);
                }));
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


    public void DoSpellActive(Action OnComplete)
    {
        if (onSpellActive == null)
            OnComplete?.Invoke();
        else
            EffectAlpha(() => onSpellActive.Invoke(OnComplete));
    }
}