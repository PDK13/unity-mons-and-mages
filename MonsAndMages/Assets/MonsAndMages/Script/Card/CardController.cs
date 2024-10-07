﻿using DG.Tweening;
using System;
using System.Collections;
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
    private bool m_top = false; //Đang được hiển thị trên cùng của màn hình
    private bool m_choice = false; //Có thể được lựa chọn trong trường hợp đặc biệt
    private bool m_choiceOnce = false; //Chỉ có thể được lựa chọn một lần duy nhật trong trường hợp đặc biệt
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
            case ChoiceType.CardManaFill:
                if (m_player != GameManager.instance.PlayerCurrent || !m_choice)
                    return;
                GameEvent.UiInfoManaFill(this);
                break;
        }
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

    public ProgessCollectType ProgessCollect => m_progessCollectCurrent;


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
    }

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
        transform.SetSiblingIndex(Pointer.childCount - 1);
        Renderer.maskable = false;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one, MoveDuration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalMove(CentreInPointer, MoveDuration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            transform.SetSiblingIndex(0);
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

        transform.SetSiblingIndex(Pointer.childCount - 2);
        Renderer.maskable = false;
        Renderer.transform.DOScale(Vector3.one * 1.35f, RumbleDuration * 0.8f).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            Renderer.transform.DOScale(Vector3.one, RumbleDuration * 0.2f).SetEase(Ease.Linear).OnComplete((() =>
            {
                m_effect = false;
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
        AlphaGroup.DOFade(1f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete((() =>
            {
                m_effect = false;
                OnComplete?.Invoke();
            }));
        }));
    }

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
    }

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
    }


    public void InfoShow(bool Show)
    {
        m_tmpGrowth.gameObject.SetActive(Type == CardType.Mons && Show);
        m_tmpMana.gameObject.SetActive(Type == CardType.Mons && Show);
        m_tmpDamage.gameObject.SetActive(Type == CardType.Mons && Show);
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
            m_progessCollectCurrent = ProgessCollectType.Start;
            m_progessEvent = OnComplete;
            DoCollectProgess();
        });
    } //Collect Event

    public void DoCollectProgess()
    {
        switch (m_progessCollectCurrent)
        {
            case ProgessCollectType.Start:
                m_progessCollectCurrent = ProgessCollectType.Origin;
                DoCollectProgess();
                break;
            case ProgessCollectType.Origin:
                m_progessCollectCurrent = ProgessCollectType.Enter;
                DoOriginActive(() => DoCollectProgess());
                break;
            case ProgessCollectType.Enter:
                m_progessCollectCurrent = ProgessCollectType.Passive;
                DoEnterActive(() => DoCollectProgess());
                break;
            case ProgessCollectType.Passive:
                m_progessCollectCurrent = ProgessCollectType.End;
                DoPassiveActive(() => DoCollectProgess());
                break;
            case ProgessCollectType.End:
                m_player.ProgessCardDone(this);
                m_progessCollectCurrent = ProgessCollectType.None;
                m_progessEvent?.Invoke();
                m_progessEvent = null;
                break;
        }
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
                    DoOriginWoodlandReady();
                    break;
                case CardOriginType.Ghost:
                    DoOriginGhostReady();
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

        GameEvent.OriginDragon(() =>
        {
            DoRumble(() =>
            {
                var PlayerQueue = GameManager.instance.PlayerQueue;
                for (int i = 0; i < PlayerQueue.Length; i++)
                {
                    if (PlayerQueue[i].Equals(m_player))
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

    public void DoOriginWoodlandReady()
    {
        var WoodlandCount = m_player.CardQueue.Count(t => t.Origin == CardOriginType.Woodland);
        var ManaGainValue = 1.0f * WoodlandCount / 2 + (WoodlandCount % 2 == 0 ? 0 : 0.5f);

        foreach (var CardCheck in m_player.CardQueue)
        {
            if (CardCheck.Origin != CardOriginType.Woodland || CardCheck.Type != CardType.Mons)
                continue;
            CardCheck.DoChoiceReady();
        }

        m_player.ProgessMana = Mathf.Max(1, (int)ManaGainValue);
        m_player.ProgessCardChoice = this;
        m_player.ProgessCard(this);
        GameManager.instance.CardOriginWoodlandReady(this);
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

        DoEffectAlpha(() => DoManaFill(1, () =>
        {
            if (m_player.ProgessMana > 0)
            {
                foreach (var CardCheck in m_player.CardQueue)
                {
                    if (CardCheck.Origin != CardOriginType.Woodland || CardCheck.Type != CardType.Mons)
                        continue;
                    CardCheck.DoChoiceReady();
                }
                GameManager.instance.CardOriginWoodlandReady(m_player.ProgessCardChoice);
            }
            else
            {
                m_player.ProgessCardChoice = null;
                m_player.ProgessCheck();
            }
        }));
    }

    public void DoOriginGhostReady()
    {
        var GhostCount = m_player.CardQueue.ToArray().Count(t => t.Origin == CardOriginType.Ghost);
        var StaffCurrent = m_player.StaffStep;
        var StaffAvaible = GhostCount;

        while (StaffAvaible > 0)
        {
            StaffCurrent++;
            if (StaffCurrent > m_player.CardQueue.Length - 1)
                StaffCurrent = 0;
            m_player.CardQueue[StaffCurrent].DoChoiceReady();
            StaffAvaible--;
        }

        m_player.ProgessCardChoice = this;
        m_player.ProgessCard(this);
        GameManager.instance.CardOriginGhostReady(this);
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
        if (m_name == CardNameType.Stage)
        {
            OnComplete?.Invoke();
            return;
        }
        DoEffectAlpha(() => DoAttackActive(() => DoManaFill(1, OnComplete)));
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

    public void DoGrowthAdd(int Value, Action OnComplete)
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

    public void DoManaProgess()
    {
        switch (m_progessManaCurrent)
        {
            case ProgessManaType.Start:
                m_progessManaCurrent = ProgessManaType.Class;
                DoManaProgess();
                break;
            case ProgessManaType.Class:
                m_progessManaCurrent = ProgessManaType.Spell;
                DoClassActive(() => DoManaProgess());
                break;
            case ProgessManaType.Spell:
                m_progessManaCurrent = ProgessManaType.End;
                DoSpellActive(() => DoManaProgess());
                break;
            case ProgessManaType.End:
                m_player.ProgessCardDone(this);
                m_progessManaCurrent = ProgessManaType.None;
                m_progessEvent?.Invoke();
                m_progessEvent = null;
                break;
        }
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
                    DoClassFlyingReady();
                    break;
                default:
                    OnComplete?.Invoke();
                    break;
            }
        });
    } //Class Event

    public void DoClassFighter(Action OnComplete)
    {
        DoClassFighterProgess(AttackCombine, 0, OnComplete);
    } //Class Fighter Event

    private void DoClassFighterProgess(int AttackCombineLeft, int DiceDotSumRolled, Action OnComplete)
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
                DoClassFighterProgess(AttackCombineLeft, DiceDotSumRolled, OnComplete);
            else
            {
                var DiceDotSumAttack = (int)(DiceDotSumRolled / 2);
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
                            PlayerQueue[i].HealthChange(-DiceDotSumAttack, OnComplete);
                        }
                        else
                            PlayerQueue[i].HealthChange(-DiceDotSumAttack, null);
                    }
                });
            }
        });
    }

    public void DoClassMagicAddictReady(Action OnComplete)
    {
        bool CardGotMana = false;
        for (int i = 0; i < m_player.CardQueue.Length; i++)
        {
            if (m_player.CardQueue[i].Equals(this) || m_player.CardQueue[i].ManaCurrent == 0)
                continue;
            CardGotMana = true;
            m_player.CardQueue[i].DoChoiceReady();
        }
        if (CardGotMana)
        {
            //Found monster(s) got mana for this monster cast spell once more time
            m_player.ProgessCardChoice = this;
            m_player.ProgessCard(this);
            GameManager.instance.CardClassMagicAddictReady(this);
        }
        else
            //If no monster got mana for this monster cast spell once more time, skip this
            OnComplete?.Invoke();
    } //Class Magic Addict Event

    public void DoClassMagicAddictStart()
    {
        foreach (var Card in m_player.CardQueue)
            Card.DoChoiceUnReady();

        DoEffectAlpha(() => DoManaFill(-1, () =>
        {
            m_player.ProgessCardChoice.DoSpellActive(() =>
            {
                m_player.ProgessCardChoice = null;
                m_player.ProgessCheck();
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
                CardL.DoManaFill(1, null);
            if (CardR != null)
                CardR.DoManaFill(1, null);
        });
    } //Class Diffuser Event

    public void DoClassFlyingReady()
    {
        var CardIndex = m_player.CardQueue.ToList().IndexOf(this);

        for (int i = 0; i < m_player.CardQueue.Length; i++)
        {
            if (Mathf.Abs(CardIndex - i) <= 1)
                continue;
            m_player.CardQueue[i].DoChoiceReady();
        }

        m_player.ProgessCardChoice = this;
        m_player.ProgessCard(this);
        GameManager.instance.CardClassFlyingReady(this);
    } //Class Flying Event

    public void DoClassFlyingStart()
    {
        foreach (var Card in m_player.CardQueue)
            Card.DoChoiceUnReady();

        DoEffectAlpha(() =>
        {
            var CardFromIndex = m_player.CardQueue.ToList().IndexOf(m_player.ProgessCardChoice);
            var CardToIndex = m_player.CardQueue.ToList().IndexOf(this);
            var MoveDirection = CardFromIndex < CardToIndex ? 1 : -1;
            m_player.DoClassFlyingProgess(CardFromIndex, CardToIndex + (MoveDirection * -1), () =>
             {
                 m_player.ProgessCardChoice.DoRumble(() => DoManaFill(1, () =>
                 {
                     m_player.ProgessCardChoice = null;
                     m_player.ProgessCheck();
                 }));
             });
        });
    }

    public void DoSpellActive(Action OnComplete)
    {
        if (onSpellActive == null)
            OnComplete?.Invoke();
        else
            DoEffectAlpha(() => onSpellActive.Invoke(OnComplete));
    } //Spell Event
}