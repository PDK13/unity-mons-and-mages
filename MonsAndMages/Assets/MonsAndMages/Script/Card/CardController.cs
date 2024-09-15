using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [SerializeField] private GameObject m_mask;
    [SerializeField] private GameObject m_renderer;
    [SerializeField] private GameObject m_rendererAlpha;
    [SerializeField] private TextMeshProUGUI m_tmpGrow;
    [SerializeField] private TextMeshProUGUI m_tmpMana;
    [SerializeField] private TextMeshProUGUI m_tmpDamage;

    private bool m_avaible = false;
    private bool m_flip = false;
    private bool m_ready = false;
    private bool m_move = false;
    private bool m_top = false;
    private bool m_rumble = false;
    private bool m_effect = false;
    private Button m_button;
    private Transform m_point;

    public ICard Card => GetComponent<ICard>();

    public Image Renderer => m_renderer.GetComponent<Image>();

    public bool Avaible => m_avaible && !m_flip && m_ready && !m_move && !m_top && !m_effect;

    //

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    public void Start()
    {
        m_rendererAlpha.GetComponent<CanvasGroup>().alpha = 0;
    }

    //

    public void BtnTap()
    {
        if (!Avaible)
            return;

        GameEvent.ButtonInteractable(false);
        GameEvent.CardTap(Card);
        GameEvent.ViewInfo(InfoType.CardCollect, true);
        MoveTop(1f, () => GameEvent.ButtonInteractable(true));
    }

    //

    public void Init(CardData Card)
    {
        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        m_renderer.GetComponent<Image>().sprite = Card.Image;
        m_rendererAlpha.GetComponent<Image>().sprite = Card.Image;
        m_rendererAlpha.GetComponent<CanvasGroup>().alpha = 0;
        InfoShow(false);
        InfoGrowUpdate(Card.GrowCurrent);
        InfoManaUpdate(Card.ManaCurrent, Card.ManaPoint);
        InfoDamageUpdate(Card.AttackCombine);

        m_avaible = false;
    }

    public void Ready()
    {
        m_ready = true;
    }

    public void Point(Transform Point)
    {
        m_point = Point;
    }


    public void Open(float Duration, Action OnComplete)
    {
        if (m_flip)
            return;
        m_flip = true;

        this.transform.eulerAngles = Vector3.zero;
        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        this.transform
            .DOLocalRotate(Vector3.up * 90f, Duration / 2)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_mask.SetActive(false);
                m_renderer.SetActive(true);
                this.transform
                    .DOLocalRotate(Vector3.zero, Duration / 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        OnComplete?.Invoke();
                        m_avaible = true;
                        m_flip = false;
                    });
            });
    }

    public void Close(float Duration, Action OnComplete)
    {
        if (m_flip)
            return;
        m_flip = true;

        m_avaible = false;

        this.transform.eulerAngles = Vector3.zero;
        m_mask.SetActive(false);
        m_renderer.SetActive(true);
        this.transform
            .DOLocalRotate(Vector3.up * 90f, Duration / 2)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_mask.SetActive(true);
                m_renderer.SetActive(false);
                this.transform
                    .DOLocalRotate(Vector3.zero, Duration / 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        OnComplete?.Invoke();
                        m_flip = false;
                    });
            });
    }


    public void MoveTop(float Duration, Action OnComplete)
    {
        if (m_top || this.m_move)
            return;
        this.m_top = true;
        this.m_move = true;

        this.transform.SetParent(PlayerView.instance.InfoView, true);

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, this.transform.DOScale(Vector3.one * 2.5f, Duration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, this.transform.DOLocalMove(Vector3.zero, Duration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            OnComplete?.Invoke();
            this.m_move = false;
        });
        CardTween.Play();
    }

    public void MoveBack(float Duration, Action OnComplete)
    {
        if (!m_top || this.m_move)
            return;
        this.m_top = false;
        this.m_move = true;

        var PointWorld = m_point.position;

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, this.transform.DOScale(Vector3.one, Duration * 0.7f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, this.transform.DOMove(PointWorld, Duration).SetEase(Ease.OutQuad));
        CardTween.OnComplete(() =>
        {
            this.transform.SetParent(m_point, true);
            OnComplete?.Invoke();
            this.m_move = false;
        });
        CardTween.Play();
    }


    public void Rumble(Action OnComplete)
    {
        if (m_rumble)
            return;

        m_rumble = true;
        this.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            this.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                GameEvent.CardRumble(Card, () =>
                {
                    OnComplete?.Invoke();
                    m_rumble = false;
                });
            });
        });
    }


    public void Effect(CardEffectType Type, float Duration, Action OnComplete)
    {
        if (m_effect)
            return;

        switch (Type)
        {
            case CardEffectType.Alpha:
                EffectAlpha(Duration, OnComplete);
                break;
        }
    }

    public void EffectAlpha(float Duration, Action OnComplete)
    {
        if (m_effect)
            return;
        m_effect = true;

        var AlphaGroup = m_rendererAlpha.GetComponent<CanvasGroup>();
        AlphaGroup.DOFade(0.25f, Duration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            AlphaGroup.DOFade(0f, Duration * 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                OnComplete?.Invoke();
                m_effect = false;
            });
        });
    }


    public void InfoShow(bool Show)
    {
        m_tmpGrow.gameObject.SetActive(Show);
        m_tmpMana.gameObject.SetActive(Show);
        m_tmpDamage.gameObject.SetActive(Show);
    }

    public void InfoGrowUpdate(int Value, bool Effect = false)
    {
        m_tmpGrow.text = Value.ToString() + GameConstant.TMP_ICON_GROW;
    }

    public void InfoManaUpdate(int Value, int Max, bool Effect = false)
    {
        m_tmpMana.text = Value.ToString() + "/" + Max.ToString() + GameConstant.TMP_ICON_MANA;
    }

    public void InfoDamageUpdate(int Value, bool Effect = false)
    {
        m_tmpDamage.text = GameConstant.TMP_ICON_DAMAGE + " " + Value.ToString();
    }
}