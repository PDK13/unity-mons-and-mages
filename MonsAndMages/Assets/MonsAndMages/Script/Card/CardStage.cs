using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardStage : MonoBehaviour, ICard
{
    private CardData m_data;

    private Button m_button;
    private GameObject m_mask;
    private GameObject m_renderer;
    private GameObject m_rendererAlpha;
    private Outline m_outline;

    private bool m_avaible = true;
    private bool m_flip = false;
    private bool m_ready = true;
    private bool m_move = false;
    private bool m_top = false;
    private bool m_rumble = false;
    private bool m_effectAlpha = false;
    private bool m_effectOutline = false;
    private bool m_effectOrigin = false;
    private bool m_effectClass = false;
    private Transform m_pointer;
    private bool m_originGhostReady = false;

    //

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_mask = transform.Find("mask").gameObject;
        m_renderer = transform.Find("renderer").gameObject;
        m_rendererAlpha = transform.Find("alpha-mask").gameObject;
        m_outline = m_renderer.GetComponent<Outline>();
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
            case ChoiceType.CardOriginGhost:
                if (Player != GameManager.instance.PlayerCurrent || !m_originGhostReady)
                    return;
                GameEvent.ButtonInteractable(false);
                GameEvent.ShowUiArea(ViewType.Field, false);
                //GameEvent.ShowUiInfo(InfoType.CardOriginGhost, false);
                for (int i = 0; i < Player.CardQueue.Length; i++)
                    Player.CardQueue[i].DoOriginGhostUnReady();
                DoOriginGhostStart();
                break;
        }
    }

    //ICard

    public CardNameType Name => CardNameType.Stage;

    public CardOriginType Origin => CardOriginType.None;

    public CardClassType Class => CardClassType.None;

    public int RuneStoneCost => 0;

    public int ManaPoint => 0;

    public int Mana => 0;

    public bool ManaFull => false;

    public int Attack => 0;

    public int Growth => 0;

    public int AttackCombine => 0;

    public IPlayer Player => GetComponentInParent<IPlayer>();

    public Image Renderer => m_renderer.GetComponent<Image>();

    public bool Avaible => m_avaible && !m_flip && m_ready && !m_move && !m_top && !m_effectAlpha;


    public void Init(CardData Data) { }

    public void Fill(Transform Point) { }


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
        transform.DOScale(Vector3.one * 1.35f, RumbleDuration * 0.8f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, RumbleDuration * 0.2f).SetEase(Ease.Linear).OnComplete(() =>
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

    public void EffectOrigin(Action OnComplete) { }

    public void EffectClass(Action OnComplete) { }


    public void InfoShow(bool Show) { }

    //

    public void DoCollectActive(IPlayer Player, Action OnComplete) { }

    public void DoOriginActive(Action OnComplete) { }

    public void DoOriginDragonActive(int DragonLeft, Action OnComplete) { }

    public void DoOriginWoodlandActive(int WoodlandCount, Action OnComplete) { }

    public void DoOriginGhostActive(int GhostCount, Action OnComplete) { }

    public void DoOriginGhostReady()
    {
        m_originGhostReady = true;
        m_effectOutline = true;
        var OutlineDuration = GameManager.instance.TweenConfig.CardAction.OutlineDuration;
        m_outline.DOScale(Vector2.one * 5f, OutlineDuration).SetLoops(-1);
        m_outline.DOColor(Color.magenta, OutlineDuration).OnComplete(() => m_effectOutline = false);
    }

    public void DoOriginGhostStart()
    {
        EffectAlpha(() =>
        {
            Player.DoStaffNext(() =>
            {
                if (Player.CardCurrent.Equals(this.GetComponent<ICard>()))
                    Player.DoStaffActive(() => GameManager.instance.PlayerDostaffNext(Player, true));
                else
                    DoOriginGhostStart();
            });
        });
    }

    public void DoOriginGhostUnReady()
    {
        m_originGhostReady = false;
        m_outline.DOKill();
        EffectOutlineNormal(null);
    }

    public void DoEnterActive(Action OnComplete) { }

    public void DoPassiveActive(Action OnComplete) { }

    //

    public void DostaffActive(Action OnComplete)
    {
        EffectAlpha(() => OnComplete?.Invoke());
    }

    public void DoAttackActive(Action OnComplete) { }

    public void DoManaFill(int Value, Action OnComplete) { }

    //

    public void DoManaActive(Action OnComplete) { }

    public void DoClassActive(Action OnComplete) { }

    public void DoSpellActive(Action OnComplete) { } //Update...!
}