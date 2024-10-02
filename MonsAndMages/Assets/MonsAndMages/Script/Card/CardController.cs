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

    //

    private RectTransform m_recTransform;
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

    private bool m_open = false;
    private bool m_top = false; //Card view on top of screen
    private bool m_ready = false; //Ready for choice in special case
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
    private int m_runeStoneTake; //When staff move to card, get Rune Stone
    private int m_manaPoint;
    private int m_manaCurrent;
    private int m_attackPoint;
    private int m_growthCurrent;

    //

    private void Awake()
    {
        m_recTransform = GetComponent<RectTransform>();

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
                if (m_player != null)
                    DoRumble(null);
                else
                    GameEvent.UiInfoCollect(this);
                break;
            case ChoiceType.CardFullMana:
                if (m_player != GameManager.instance.PlayerCurrent || !ManaFull)
                    return;
                GameEvent.UiInfoFullMana(this);
                break;
            case ChoiceType.CardOriginGhost:
                if (m_player != GameManager.instance.PlayerCurrent || !m_ready)
                    return;
                GameEvent.UiInfoOriginGhost(this);
                break;
            case ChoiceType.CardClassMagicAddict:
                if (m_player != GameManager.instance.PlayerCurrent || !m_ready)
                    return;
                GameEvent.UiInfoClassMagicAddict(this);
                break;
            case ChoiceType.CardClassFlying:
                if (m_player != GameManager.instance.PlayerCurrent || !m_ready)
                    return;
                GameEvent.UiInfoClassFlying(this);
                break;
        }
    }

    //ICard

    public IPlayer Player => m_player;

    public CardNameType Name => m_name;

    public CardOriginType Origin => m_origin;

    public CardClassType Class => m_class;

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

    public Image Renderer => m_renderer.GetComponent<Image>();

    public bool Avaible =>
        m_open &&
        !m_flip &&
        !m_move &&
        !m_top &&
        !m_effect;

    public int Index => Centre.GetSiblingIndex();


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
        }
        else
        {
            m_player = this.GetComponentInParent<IPlayer>();
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
    }

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

    public void DoMoveBack(Action OnComplete)
    {
        if (!m_top || m_move)
            Debug.Log("Card move (back) not done yet");
        m_top = false;
        m_move = true;

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        transform.SetParent(Pointer, true);
        transform.SetSiblingIndex(Pointer.childCount - 2);
        Renderer.maskable = false;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one, MoveDuration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalMove(CentreInPointer, MoveDuration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            transform.SetSiblingIndex(Centre.GetSiblingIndex());
            Renderer.maskable = true;
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

        transform
            .DOLocalMove(CentreInPointer, MoveDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_move = false;
                transform.SetSiblingIndex(Centre.GetSiblingIndex());
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
            transform.SetSiblingIndex(Centre.GetSiblingIndex());
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

        transform.SetSiblingIndex(Pointer.childCount - 2);
        Renderer.maskable = false;
        Renderer.transform.DOScale(Vector3.one * 1.35f, RumbleDuration * 0.8f).SetEase(Ease.OutQuad).OnComplete((TweenCallback)(() =>
        {
            Renderer.transform.DOScale(Vector3.one, RumbleDuration * 0.2f).SetEase(Ease.Linear).OnComplete((TweenCallback)(() =>
            {
                this.m_effect = false;
                transform.SetSiblingIndex(0);
                Renderer.maskable = true;
                GameEvent.CardRumble(this, () => OnComplete?.Invoke());
            }));
        }));

        if (Player.CardStaffCurrent.Equals(this))
            Player.DoStaffRumble(null);
    }


    public void DoEffectAlpha(Action OnComplete)
    {
        m_effect = true;

        var AlphaDuration = GameManager.instance.TweenConfig.CardAction.AlphaDuration;

        var AlphaGroup = m_rendererAlpha.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(1f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete((TweenCallback)(() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete((TweenCallback)(() =>
            {
                this.m_effect = false;
                OnComplete?.Invoke();
            }));
        }));
    }

    public void DoEffectOutlineNormal(Action OnComplete)
    {
        m_effect = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 2f, OutlineDuration);
        m_outline.DOColor(Color.black, OutlineDuration).OnComplete((TweenCallback)(() =>
        {
            this.m_effect = false;
            OnComplete?.Invoke();
        }));
    }

    public void DoEffectOutlineMana(Action OnComplete)
    {
        m_effect = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.cyan, OutlineDuration).OnComplete((TweenCallback)(() =>
        {
            this.m_effect = false;
            OnComplete?.Invoke();
        }));
    }


    public void DoEffectOrigin(Action OnComplete)
    {
        m_effect = true;

        var AlphaDuration = GameManager.instance.TweenConfig.CardAction.AlphaDuration;

        var AlphaGroup = m_originIcon.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(1f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete((TweenCallback)(() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete((TweenCallback)(() =>
            {
                this.m_effect = false;
                OnComplete?.Invoke();
            }));
        }));
    }

    public void DoEffectClass(Action OnComplete)
    {
        m_effect = true;

        var AlphaDuration = GameManager.instance.TweenConfig.CardAction.AlphaDuration;

        var AlphaGroup = m_classIcon.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(1f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete((TweenCallback)(() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete((TweenCallback)(() =>
            {
                this.m_effect = false;
                OnComplete?.Invoke();
            }));
        }));
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
                DoEffectOutlineMana(() => DoEffectOutlineNormal(OnComplete));
            });
        });
        InfoDamageUpdate(AttackCombine, null);
    }

    private void InfoManaUpdate(int Value, int Max, Action OnComplete)
    {
        DoEffectOutlineMana(() => DoEffectOutlineNormal(() =>
        {
            m_tmpMana.transform.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
            {
                var ValueText = string.Format("{0}/{1} {2}", m_manaCurrent, m_manaPoint, GameConstant.TMP_ICON_Mana);
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
                DoEffectOutlineMana(() => DoEffectOutlineNormal(OnComplete));
            });
        });
    }


    public void DoCollectActive(IPlayer Player, Action OnComplete)
    {
        m_player = Player;
        DoEffectAlpha(() =>
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
        DoEffectOrigin(() =>
        {
            switch (Origin)
            {
                case CardOriginType.Dragon:
                    var DragonCount = m_player.CardQueue.Count(t => t.Origin == CardOriginType.Dragon);
                    DoOriginDragonActive(DragonCount, OnComplete);
                    break;
                case CardOriginType.Woodland:
                    var WoodlandCount = m_player.CardQueue.Count(t => t.Origin == CardOriginType.Woodland);
                    DoOriginWoodlandActive(WoodlandCount, OnComplete);
                    break;
                case CardOriginType.Ghost:
                    var GhostCount = m_player.CardQueue.Count(t => t.Origin == CardOriginType.Ghost);
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
            DoRumble(() =>
            {
                var PlayerQueue = GameManager.instance.PlayerQueue;
                for (int i = 0; i < PlayerQueue.Length; i++)
                {
                    if (PlayerQueue[i].Equals(this.m_player))
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
        var StaffCurrent = m_player.StaffStep;
        var StaffAvaible = GhostCount;
        while (StaffAvaible > 0)
        {
            StaffCurrent++;
            if (StaffCurrent > m_player.CardQueue.Length - 1)
                StaffCurrent = 0;
            m_player.CardQueue[StaffCurrent].DoOriginGhostReady();
            StaffAvaible--;
        }
        GameManager.instance.CardOriginGhostDoChoice(this);
    } //Origin Ghost Event

    public void DoOriginGhostReady()
    {
        m_ready = true;
        m_effect = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.magenta, OutlineDuration).OnComplete(() => m_effect = false);
    }

    public void DoOriginGhostStart()
    {
        foreach (var Card in m_player.CardQueue)
            Card.DoOriginGhostUnReady();
        DoOriginGhostProgess();
    }

    private void DoOriginGhostProgess()
    {
        DoEffectAlpha(() =>
        {
            m_player.DoStaffNext(() =>
            {
                if (m_player.CardStaffCurrent.Equals(this.GetComponent<ICard>()))
                    //Active staff when land on card choosed
                    m_player.DoStaffActive(() =>
                    {
                        //Continue resolve current card before return to main progess
                        DoEnterActive(() => DoPassiveActive(() =>
                        {
                            GameManager.instance.PlayerDoStaffNext(m_player, true);
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
        m_ready = false;
        if (ManaFull)
            DoEffectOutlineMana(null);
        else
            DoEffectOutlineNormal(null);
    }


    public void DoOriginInsectActive(Action OnComplete) { DoEffectOrigin(OnComplete); } //Origin Insect Event


    public void DoOriginSirenActive(Action OnComplete) { DoEffectOrigin(OnComplete); } //Origin Siren Event


    public void DoOriginWoodlandActive(int WoodlandCount, Action OnComplete)
    {
        var ManaGainValue = 1.0f * WoodlandCount / 2 + (WoodlandCount % 2 == 0 ? 0 : 0.5f);
        DoManaFill((int)ManaGainValue, OnComplete);
    } //Origin Woodland Event


    public void DoOriginNeutralActive(Action OnComplete)
    {
        DoEffectOrigin(OnComplete);
    } //Origin Neutral Active


    public void DoEnterActive(Action OnComplete)
    {
        if (onEnterActive == null)
            OnComplete?.Invoke();
        else
            DoEffectAlpha(() => onEnterActive.Invoke(OnComplete));
    } //Enter Event

    public void DoPassiveActive(Action OnComplete)
    {
        if (onPassiveActive == null)
            OnComplete?.Invoke();
        else
            DoEffectAlpha(() => onPassiveActive.Invoke(OnComplete));
    } //Passive Event


    public void DostaffActive(Action OnComplete)
    {
        DoEffectAlpha(() => DoAttackActive(() => DoManaFill(1, OnComplete)));
    }

    public void DoAttackActive(Action OnComplete)
    {
        DoRumble(() =>
        {
            var PlayerQueue = GameManager.instance.PlayerQueue;
            for (int i = 0; i < PlayerQueue.Length; i++)
            {
                if (PlayerQueue[i] == m_player)
                    continue;
                PlayerQueue[i].HealthChange(-AttackCombine, null);
            }
            OnComplete?.Invoke();
        });
    }

    public void DoManaFill(int Value, Action OnComplete)
    {
        m_manaCurrent += Value;
        if (m_manaCurrent > m_manaPoint)
            m_manaCurrent = m_manaPoint;
        if (ManaFull)
            InfoManaUpdate(m_manaCurrent, m_manaPoint, () => DoEffectOutlineMana(OnComplete));
        else
            InfoManaUpdate(m_manaCurrent, m_manaPoint, OnComplete);
    }

    public void DoGrowthAdd(int Value, Action OnComplete)
    {
        m_growthCurrent += Value;
        InfoGrowUpdate(m_growthCurrent, () => OnComplete());
    }


    public void DoManaActive(Action OnComplete)
    {
        m_manaCurrent = 0;
        InfoManaUpdate(m_manaCurrent, m_manaPoint, () => DoEffectOutlineNormal(() =>
        {
            DoClassActive(() => DoSpellActive(OnComplete));
        }));
    } //Invoke from GameManager


    public void DoClassActive(Action OnComplete)
    {
        DoEffectClass(() =>
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
                    var SingerCount = m_player.CardQueue.Count(t => t.Class == CardClassType.Singer);
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
                DoRumble(() =>
                {
                    var OnCompleteEvent = false;
                    var PlayerQueue = GameManager.instance.PlayerQueue;
                    for (int i = 0; i < PlayerQueue.Length; i++)
                    {
                        if (PlayerQueue[i].Equals(this.m_player))
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
        for (int i = 0; i < m_player.CardQueue.Length; i++)
        {
            if (m_player.CardQueue[i].Equals(this) || m_player.CardQueue[i].ManaCurrent == 0)
                continue;
            CardGotMana = true;
            m_player.CardQueue[i].DoClassMagicAddictReady();
        }
        DoEffectClass(() =>
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
        m_ready = true;
        m_effect = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.red, OutlineDuration).OnComplete(() => m_effect = false);
    }

    public void DoClassMagicAddictStart()
    {
        foreach (var Card in m_player.CardQueue)
            Card.DoClassMagicAddictUnReady();
        DoEffectAlpha(() => DoManaFill(-1, () =>
        {
            m_player.CardManaActiveCurrent.Player.CardManaActiveCurrent.DoSpellActive(() => m_player.CardManaActiveCurrent.DoSpellActive(() =>
            {
                m_player.DoCardSpecialActiveCurrent(null);
                GameManager.instance.CardManaCheck(Player);
            }));
        }));
    }

    public void DoClassMagicAddictUnReady()
    {
        m_ready = false;
        if (ManaFull)
            DoEffectOutlineMana(null);
        else
            DoEffectOutlineNormal(null);
    }


    public void DoClassSingerActive(int SingerCount, Action OnComplete)
    {
        DoRumble(() =>
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


    public void DoClassCareTakerActive(Action OnComplete) { DoEffectClass(OnComplete); } //Class Care Taker Event


    public void DoClassDiffuserActive(Action OnComplete)
    {
        DoRumble(() =>
        {
            var CardCurrentIndex = Player.CardQueue.ToList().IndexOf(this);
            var CardL = CardCurrentIndex - 1 >= 0 ? Player.CardQueue[CardCurrentIndex - 1] : null;
            var CardR = CardCurrentIndex + 1 <= Player.CardQueue.Length - 1 ? Player.CardQueue[CardCurrentIndex + 1] : null;
            DoEffectOutlineMana(() =>
            {
                DoEffectOutlineNormal(OnComplete);
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
        m_ready = true;
        m_effect = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration);
        m_outline.DOColor(Color.green, OutlineDuration).OnComplete(() => m_effect = false);
    }

    public void DoClassFlyingStart()
    {
        foreach (var Card in Player.CardQueue)
            Card.DoClassFlyingUnReady();
        DoEffectAlpha(() =>
        {
            var CardFromIndex = Player.CardQueue.ToList().IndexOf(Player.CardManaActiveCurrent);
            var CardToIndex = Player.CardQueue.ToList().IndexOf(this);
            var MoveDirection = CardFromIndex < CardToIndex ? 1 : -1;
            Player.DoCardSwap(CardFromIndex, CardToIndex + (MoveDirection * -1), () =>
            {
                DoRumble(() => DoManaFill(1, () =>
                {
                    Player.DoCardSpecialActiveCurrent(null);
                    GameManager.instance.CardManaCheck(Player);
                }));
            });
        });
    }

    public void DoClassFlyingUnReady()
    {
        m_ready = false;
        if (ManaFull)
            DoEffectOutlineMana(null);
        else
            DoEffectOutlineNormal(null);
    }


    public void DoSpellActive(Action OnComplete)
    {
        if (onSpellActive == null)
            OnComplete?.Invoke();
        else
            DoEffectAlpha(() => onSpellActive.Invoke(OnComplete));
    }
}