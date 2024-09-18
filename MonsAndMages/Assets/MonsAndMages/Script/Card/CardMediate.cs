using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CardMediate : MonoBehaviour, ICard
{
    private CardData m_data;

    private Button m_button;
    private GameObject m_mask;
    private GameObject m_renderer;
    private GameObject m_rendererAlpha;
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

        bool Base = GameManager.instance.PlayerCurrent.Base || GameManager.instance.SameDevice;
        bool Choice = GameManager.instance.PlayerChoice;

        if (Base && Choice)
        {
            GameEvent.ButtonInteractable(false);
            GameEvent.CardTap(this, null);
            GameEvent.ViewInfo(InfoType.CardCollect, true);
            MoveTop(1f, () => GameEvent.ButtonInteractable(true));
        }
        else
        {
            Debug.Log("Card Tap by another Player clone");
        }
    }

    //ICard

    public CardNameType Name => CardNameType.Mediate;

    public CardOriginType Origin => CardOriginType.None;

    public CardClassType Class => CardClassType.None;

    public int RuneStoneCost => 0;

    public int Energy => 0;

    public int EnergyCurrent => 0;

    public bool EnergyFull => false;

    public int Attack => 0;

    public int Grow => 0;

    public int AttackCombine => 0;

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
        InfoManaUpdate(m_data.ManaCurrent, m_data.ManaPoint);
        InfoDamageUpdate(m_data.AttackCombine);

        m_avaible = false;
    }

    public void Fill(Transform Point)
    {
        this.Pointer(Point);

        var DurationMove = GameManager.instance.TweenConfig.CardFill.MoveDuration;
        var EaseMove = GameManager.instance.TweenConfig.CardFill.MoveEase;
        var DurationOpen = GameManager.instance.TweenConfig.CardFill.OpenDuration;

        transform.SetParent(Point, true);
        transform.DOLocalMove(Vector3.zero, DurationMove).SetEase(EaseMove);

        Open(DurationOpen, null);
    }


    public void Ready()
    {
        m_ready = true;
    }

    public void Pointer(Transform Point)
    {
        m_pointer = Point;
    }


    public void Open(float Duration, Action OnComplete)
    {
        if (m_flip)
            Debug.Log("Card open not done yet");
        m_flip = true;

        transform.eulerAngles = Vector3.zero;
        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        transform
            .DOLocalRotate(Vector3.up * 90f, Duration / 2)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_mask.SetActive(false);
                m_renderer.SetActive(true);
                transform
                    .DOLocalRotate(Vector3.zero, Duration / 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        m_avaible = true;
                        m_flip = false;
                        OnComplete?.Invoke();
                    });
            });
    }

    public void Close(float Duration, Action OnComplete)
    {
        if (m_flip)
            Debug.Log("Card close not done yet");
        m_flip = true;

        m_avaible = false;

        transform.eulerAngles = Vector3.zero;
        m_mask.SetActive(false);
        m_renderer.SetActive(true);
        transform
            .DOLocalRotate(Vector3.up * 90f, Duration / 2)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_mask.SetActive(true);
                m_renderer.SetActive(false);
                transform
                    .DOLocalRotate(Vector3.zero, Duration / 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        m_flip = false;
                        OnComplete?.Invoke();
                    });
            });
    }


    public void MoveTop(float Duration, Action OnComplete)
    {
        if (m_top || m_move)
            Debug.Log("Card move (top) not done yet");
        m_top = true;
        m_move = true;

        transform.SetParent(PlayerView.instance.InfoView, true);

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one * 2.5f, Duration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOLocalMove(Vector3.zero, Duration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            m_move = false;
            OnComplete?.Invoke();
        });
        CardTween.Play();
    }

    public void MoveBack(float Duration, Action OnComplete)
    {
        if (!m_top || m_move)
            Debug.Log("Card move (back) not done yet");
        m_top = false;
        m_move = true;

        var PointWorld = m_pointer.position;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, transform.DOScale(Vector3.one, Duration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, transform.DOMove(PointWorld, Duration).SetEase(Ease.OutQuad));
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

        m_rumble = true;
        Renderer.maskable = false;
        transform.DOScale(Vector3.one * 1.35f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
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


    public void EffectAlpha(float Duration, Action OnComplete)
    {
        if (m_effect)
            Debug.Log("Card effect alpha not done yet");
        m_effect = true;

        var AlphaGroup = m_rendererAlpha.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(0.25f, Duration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            AlphaGroup.DOFade(0f, Duration * 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_effect = false;
                OnComplete?.Invoke();
            });
        });
    }

    public void EffectOutlineNormal(float Duration, Action OnComplete)
    {
        m_outline.DOColor(Color.black, Duration).OnComplete(() => OnComplete?.Invoke());
    }

    public void EffectOutlineEnergy(float Duration, Action OnComplete)
    {
        m_outline.DOColor(Color.cyan, Duration).OnComplete(() => OnComplete?.Invoke());
    }


    public void InfoShow(bool Show) { }

    public void InfoGrowUpdate(int Value, bool Effect = false) { }

    public void InfoManaUpdate(int Value, int Max, bool Effect = false) { }

    public void InfoDamageUpdate(int Value, bool Effect = false) { }

    //

    public void DoCollectActive(IPlayer Player, Action OnComplete) { }

    public void DoOriginActive(Action OnComplete) { }

    public void DoEnterActive() { }

    public void DoPassiveActive() { }

    //

    public void DoWandActive(Action OnComplete)
    {
        EffectAlpha(1f, () => OnComplete?.Invoke());
    }

    public void DoAttackActive(Action OnComplete) { }

    public void DoEnergyFill(int Value, Action OnComplete) { }

    //

    public void DoEnergyActive(Action OnComplete) { }

    public void DoClassActive(Action OnComplete) { }

    public void DoSpellActive(Action OnComplete) { } //Update...!
}