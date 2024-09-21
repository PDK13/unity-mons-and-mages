using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour, ICard
{
    public Action<Action> onOriginActive;
    public Action<Action> onEnterActive;
    public Action<Action> onPassiveActive;
    public Action<Action> onClassActive;
    public Action<Action> onSpellActive;

    private CardData m_data;

    private Button m_button;
    private GameObject m_mask;
    private GameObject m_renderer;
    private GameObject m_rendererAlpha;
    private TextMeshProUGUI m_tmpGrow;
    private TextMeshProUGUI m_tmpEnergy;
    private TextMeshProUGUI m_tmpDamage;
    private Outline m_outline;

    private bool m_avaible = false;
    private bool m_flip = false;
    private bool m_ready = false;
    private bool m_move = false;
    private bool m_top = false;
    private bool m_rumble = false;
    private bool m_effect = false;
    private Transform m_pointer;

    //

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_mask = transform.Find("mask").gameObject;
        m_renderer = transform.Find("renderer").gameObject;
        m_rendererAlpha = transform.Find("alpha-mask").gameObject;
        m_tmpGrow = transform.Find("tmp-grow").GetComponent<TextMeshProUGUI>();
        m_tmpEnergy = transform.Find("tmp-energy").GetComponent<TextMeshProUGUI>();
        m_tmpDamage = transform.Find("tmp-damage").GetComponent<TextMeshProUGUI>();
        m_outline = m_renderer.GetComponent<Outline>();
    }

    public void Start()
    {
        m_button.onClick.AddListener(BtnTap);
    }

    //

    public void BtnTap()
    {
        if (Player == null)
        {
            if (!Avaible)
                return;

            GameEvent.ButtonInteractable(false);
            GameEvent.CardTap(this, null);
            GameEvent.ViewInfo(InfoType.Collect, true);
            MoveTop(() => GameEvent.ButtonInteractable(true));
        }
    }

    //ICard

    public CardNameType Name => m_data.Name;

    public CardOriginType Origin => m_data.Origin;

    public CardClassType Class => m_data.Class;

    public int RuneStoneCost => m_data.RuneStoneCost;

    public int EnergyPoint => m_data.EnergyPoint;

    public int EnergyCurrent => m_data.EnergyCurrent;

    public bool EnergyFull => m_data.EnergyPoint > 0 && m_data.EnergyCurrent >= m_data.EnergyPoint;

    public int Attack => m_data.AttackPoint;

    public int Grow => m_data.GrowCurrent;

    public int AttackCombine => m_data.AttackCombine;

    public IPlayer Player => m_data.Player;

    public Image Renderer => m_renderer.GetComponent<Image>();

    public bool Avaible => m_avaible && !m_flip && m_ready && !m_move && !m_top && !m_effect;


    public void Init(CardData Data)
    {
        m_data = Data;

        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        m_renderer.GetComponent<Image>().sprite = m_data.Image;
        m_rendererAlpha.GetComponent<Image>().sprite = m_data.Image;
        m_rendererAlpha.GetComponent<CanvasGroup>().alpha = 0;
        InfoShow(false);
        InfoGrowUpdate(m_data.GrowCurrent);
        InfoManaUpdate(m_data.EnergyCurrent, m_data.EnergyPoint);
        InfoDamageUpdate(m_data.AttackCombine);

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
        CardTween.Insert(0f, transform.DOScale(Vector3.one * 2.5f, MoveDuration * 0.7f).SetEase(Ease.OutQuad));
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
        if (m_effect)
            Debug.Log("Card effect alpha not done yet");
        m_effect = true;

        var AlphaDuration = GameManager.instance.TweenConfig.CardAction.AlphaDuration;

        var AlphaGroup = m_rendererAlpha.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(0.25f, AlphaDuration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            AlphaGroup.DOFade(0f, AlphaDuration * 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_effect = false;
                OnComplete?.Invoke();
            });
        });
    }

    public void EffectOutlineNormal(Action OnComplete)
    {
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOColor(Color.black, OutlineDuration).OnComplete(() => OnComplete?.Invoke());
    }

    public void EffectOutlineEnergy(Action OnComplete)
    {
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOColor(Color.cyan, OutlineDuration).OnComplete(() => OnComplete?.Invoke());
    }


    public void InfoShow(bool Show)
    {
        m_tmpGrow.gameObject.SetActive(Show);
        m_tmpEnergy.gameObject.SetActive(Show);
        m_tmpDamage.gameObject.SetActive(Show);
    }

    public void InfoGrowUpdate(int Value, bool Effect = false)
    {
        m_tmpGrow.text = Value.ToString() + GameConstant.TMP_ICON_GROW;
    }

    public void InfoManaUpdate(int Value, int Max, bool Effect = false)
    {
        m_tmpEnergy.text = Value.ToString() + "/" + Max.ToString() + GameConstant.TMP_ICON_ENERGY;
    }

    public void InfoDamageUpdate(int Value, bool Effect = false)
    {
        m_tmpDamage.text = GameConstant.TMP_ICON_DAMAGE + " " + Value.ToString();
    }


    public void DoCollectActive(IPlayer Player, Action OnComplete)
    {
        m_data.Player = Player;
        EffectAlpha(() => OnComplete?.Invoke());
    }

    public virtual void DoOriginActive(Action OnComplete)
    {
        EffectAlpha(() => Rumble(() => OnComplete?.Invoke()));
    } //Virtal

    public virtual void DoEnterActive() { } //Virtal

    public virtual void DoPassiveActive() { } //Virtal


    public void DostaffActive(Action OnComplete)
    {
        EffectAlpha(() => OnComplete?.Invoke());
    }

    public void DoAttackActive(Action OnComplete)
    {
        Rumble(() => OnComplete?.Invoke());
    }

    public void DoEnergyFill(int Value, Action OnComplete)
    {
        m_data.EnergyCurrent += Value;
        m_tmpEnergy.transform.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            var EnergyText = string.Format("{0}/{1}{2}", EnergyCurrent, EnergyPoint, GameConstant.TMP_ICON_ENERGY);
            m_tmpEnergy.text = EnergyText;
            m_tmpEnergy.transform.DOScale(Vector2.one, 0.1f).OnComplete(() =>
            {
                EffectOutlineEnergy(() => EffectOutlineNormal(() => OnComplete?.Invoke()));
            });
        });
    }


    public void DoEnergyActive(Action OnComplete)
    {
        m_data.EnergyCurrent = 0;
        Rumble(() => OnComplete?.Invoke());
    }

    public virtual void DoClassActive(Action OnComplete)
    {
        Rumble(() => OnComplete?.Invoke());
    } //Virtal

    public virtual void DoSpellActive(Action OnComplete)
    {
        Rumble(() => OnComplete?.Invoke());
    } //Virtal
}