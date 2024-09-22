using DG.Tweening;
using System;
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
    private Transform m_pointer;

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
            case ChoiceType.Main:
                if (Player != null)
                    return;
                GameEvent.ButtonInteractable(false);
                GameEvent.CardTap(this, null);
                GameEvent.ViewInfo(InfoType.Collect, true);
                MoveTop(() => GameEvent.ButtonInteractable(true));
                break;
            case ChoiceType.CardFullMana:
                if (Player != GameManager.instance.PlayerCurrent || !ManaFull)
                    return;
                GameEvent.ButtonInteractable(false);
                GameEvent.CardTap(this, null);
                GameEvent.ViewInfo(InfoType.CardFullMana, true);
                MoveTop(() => GameEvent.ButtonInteractable(true));
                break;
        }
    }

    //ICard

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

    public IPlayer Player => m_data.Player;

    public Image Renderer => m_renderer.GetComponent<Image>();

    public bool Avaible => m_avaible && !m_flip && m_ready && !m_move && !m_top && !m_effectAlpha;


    public void Init(CardData Data)
    {
        m_data = Data;
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

    public void Fill(Transform Point)
    {
        this.Pointer(Point);

        var DurationMove = GameManager.instance.TweenConfig.WildFill.MoveDuration;
        var EaseMove = GameManager.instance.TweenConfig.WildFill.MoveEase;

        transform.SetParent(Point, true);
        transform.DOLocalMove(Vector3.zero, DurationMove).SetEase(EaseMove);

        FlipOpen(null);
    }


    public void Ready()
    {
        m_ready = true;
    }

    public void Pointer(Transform Point)
    {
        m_pointer = Point;
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

        var PointWorld = m_pointer.position;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one, MoveDuration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOMove(PointWorld, MoveDuration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            transform.SetParent(m_pointer, true);
            transform.SetSiblingIndex(0);
            OnComplete?.Invoke();
        });
        CardTween.Play();
    }


    public void Rumble(Action OnComplete)
    {
        if (m_rumble)
            Debug.Log("Card rumble not done yet");

        var RumbleDuration = GameManager.instance.TweenConfig.CardAction.RumbleDuration;

        m_rumble = true;
        Renderer.maskable = false;
        Renderer.transform.DOScale(Vector3.one * 1.35f, RumbleDuration * 0.8f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Renderer.transform.DOScale(Vector3.one, RumbleDuration * 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
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
            var ManaText = Value.ToString() + GameConstant.TMP_ICON_GROW;
            m_tmpGrowth.text = ManaText;
            m_tmpGrowth.transform.DOScale(Vector2.one, 0.1f).OnComplete(() =>
            {
                EffectOutlineMana(() => EffectOutlineNormal(() => OnComplete?.Invoke()));
            });
        });
    }

    private void InfoManaUpdate(int Value, int Max, Action OnComplete)
    {
        EffectOutlineMana(() => EffectOutlineNormal(() =>
        {
            m_tmpMana.transform.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
            {
                var ManaText = string.Format("{0}/{1}{2}", Mana, ManaPoint, GameConstant.TMP_ICON_Mana);
                m_tmpMana.text = ManaText;
                m_tmpMana.transform.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
            });
        }));
    }

    private void InfoDamageUpdate(int Value, Action OnComplete)
    {
        m_tmpDamage.transform.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            var ManaText = GameConstant.TMP_ICON_DAMAGE + " " + Value.ToString();
            m_tmpDamage.text = ManaText;
            m_tmpDamage.transform.DOScale(Vector2.one, 0.1f).OnComplete(() =>
            {
                EffectOutlineMana(() => EffectOutlineNormal(() => OnComplete?.Invoke()));
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
    } //Invoke from GameManager

    public void DoOriginActive(Action OnComplete)
    {
        switch (Origin)
        {
            case CardOriginType.Dragon:
                var DragonAvaibleCount = Player.CardQueue.Count(t => t.Origin == CardOriginType.Dragon);
                DoOriginDragonActive(DragonAvaibleCount, OnComplete);
                break;
            case CardOriginType.Woodland:
                var WoodLandCount = Player.CardQueue.Count(t => t.Origin == CardOriginType.Woodland);
                DoOriginWoodLandActive(WoodLandCount, OnComplete);
                break;
            case CardOriginType.Ghost:
                EffectOrigin(() => OnComplete?.Invoke());
                break;
            case CardOriginType.Insects:
                EffectOrigin(() => OnComplete?.Invoke());
                break;
            case CardOriginType.Siren:
                EffectOrigin(() => OnComplete?.Invoke());
                break;
            case CardOriginType.Neutral:
                EffectOrigin(() => OnComplete?.Invoke());
                break;
            default:
                OnComplete?.Invoke();
                break;
        }
    }

    private void DoOriginDragonActive(int DragonLeft, Action OnComplete)
    {
        var DiceFaceQueue = GameManager.instance.DiceConfig.Data;
        var DiceFaceCount = DiceFaceQueue.Count;
        var DragonDiceIndex = UnityEngine.Random.Range(0, (DiceFaceCount * 10) - 1) / 10;
        var DragonDiceFace = DiceFaceQueue[DragonDiceIndex];
        EffectOrigin(() => Rumble(() =>
        {
            var PlayerQueue = GameManager.instance.PlayerQueue;
            for (int i = 0; i < PlayerQueue.Length; i++)
            {
                if (PlayerQueue[i].Equals(this.Player))
                    PlayerQueue[i].HealthChange(-DragonDiceFace.Bite, () =>
                    {
                        if (DragonLeft > 0)
                            DoOriginDragonActive(DragonLeft, OnComplete);
                        else
                            OnComplete?.Invoke();
                    });
                else
                    PlayerQueue[i].HealthChange(-DragonDiceFace.Dragon, null);
            }
        }));
        DragonLeft--;
    }

    private void DoOriginWoodLandActive(int WoodLandCount, Action OnComplete)
    {
        var ManaGainValue = 1.0f * WoodLandCount / 2 + (WoodLandCount % 2 == 0 ? 0 : 0.5f);
        EffectOrigin(() => DoManaFill((int)ManaGainValue, () => OnComplete?.Invoke()));
    }

    public void DoEnterActive(Action OnComplete)
    {
        if (onEnterActive == null)
            OnComplete?.Invoke();
        else
            EffectAlpha(() => onEnterActive.Invoke(() => OnComplete?.Invoke()));
    }

    public void DoPassiveActive(Action OnComplete)
    {
        if (onPassiveActive == null)
            OnComplete?.Invoke();
        else
            EffectAlpha(() => onPassiveActive.Invoke(() => OnComplete?.Invoke()));
    }


    public void DostaffActive(Action OnComplete)
    {
        EffectAlpha(() => OnComplete?.Invoke());
    }

    public void DoAttackActive(Action OnComplete)
    {
        Rumble(() => OnComplete?.Invoke());
    }

    public void DoManaFill(int Value, Action OnComplete)
    {
        Mana += Value;
        if (Mana > ManaPoint)
            Mana = ManaPoint;
        if (ManaFull)
            InfoManaUpdate(Mana, ManaPoint, () => EffectOutlineMana(() => OnComplete?.Invoke()));
        else
            InfoManaUpdate(Mana, ManaPoint, () => OnComplete?.Invoke());
    }


    public void DoManaActive(Action OnComplete)
    {
        Mana = 0;
        InfoManaUpdate(Mana, ManaPoint, () => EffectOutlineNormal(() =>
        {
            DoClassActive(() => DoSpellActive(() => OnComplete?.Invoke()));
        }));
    } //Invoke from GameManager

    public void DoClassActive(Action OnComplete)
    {
        switch (Class)
        {
            case CardClassType.Fighter:
                EffectClass(() => OnComplete?.Invoke());
                break;
            case CardClassType.MagicAddict:
                EffectClass(() => OnComplete?.Invoke());
                break;
            case CardClassType.Singer:
                EffectClass(() => OnComplete?.Invoke());
                break;
            case CardClassType.Caretaker:
                EffectClass(() => OnComplete?.Invoke());
                break;
            case CardClassType.Diffuser:
                EffectClass(() => OnComplete?.Invoke());
                break;
            case CardClassType.Flying:
                EffectClass(() => OnComplete?.Invoke());
                break;
        }
    }

    public void DoSpellActive(Action OnComplete)
    {
        if (onSpellActive == null)
            OnComplete?.Invoke();
        else
            EffectAlpha(() => onSpellActive.Invoke(() => OnComplete?.Invoke()));
    }
}