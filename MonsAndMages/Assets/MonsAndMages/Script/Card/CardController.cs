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

    private bool m_effect = false;
    private bool m_ready = false;
    private Button m_button;

    public ICard Card => GetComponent<ICard>();

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        GameEvent.onButtonPress += OnButtonPress;
        GameEvent.onPlayerDoCollect += OnPlayerDoCollect;
    }

    private void OnDisable()
    {
        GameEvent.onButtonPress -= OnButtonPress;
        GameEvent.onPlayerDoCollect -= OnPlayerDoCollect;
    }

    public void Start()
    {
        m_rendererAlpha.GetComponent<CanvasGroup>().alpha = 0;
    }

    //

    private void OnButtonPress(Button Button)
    {
        if (!Button.Equals(m_button))
            m_ready = false;
    }

    private void OnPlayerDoCollect(IPlayer Player, ICard Card, Action OnComplete)
    {
        if (!Card.Equals(this.Card))
            return;

        var CardRenderer = m_renderer.GetComponent<Image>();
        var TopView = PlayerView.instance.transform;
        var DownView = Player.DoCollectReady().transform;

        CardRenderer.maskable = false;
        this.transform.SetParent(TopView, true);
        this.transform.DOScale(Vector3.one * 2.5f, 0.7f).SetEase(Ease.OutQuad).SetDelay(0.3f);
        this.transform.DOLocalMove(Vector3.zero, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            GameEvent.ViewField(() =>
            {
                GameEvent.onWildFill();
                transform.SetParent(DownView.transform, true);
                this.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    this.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        this.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            CardRenderer.maskable = true;
                            GameEvent.CardRumble(Card, () =>
                            {
                                OnComplete?.Invoke();
                            });
                        });
                    });
                });
                this.transform.DOLocalMove(Vector3.zero, 1f).SetEase(Ease.OutQuad);
            });
        });
    } //Card do collect tween here

    //

    public void BtnTap()
    {
        GameEvent.ButtonInteractable(false);
        EffectAlpha(1f, () =>
        {
            switch (Card.Name)
            {
                case CardNameType.None:
                    Debug.LogError("Card controller found " + this.gameObject.name + " got name None");
                    break;
                case CardNameType.Stage:
                    GameEvent.ButtonInteractable(true);
                    break;
                default:
                    if (!m_ready)
                    {
                        m_ready = true;
                        GameEvent.ButtonInteractable(true);
                    }
                    else
                    {
                        GameEvent.CardTap(Card, null);

                        if (Card.Player == null)
                        {
                            //Test
                            var Player = GameManager.instance.PlayerCurrent;

                            GameEvent.PlayerDoCollect(Player, Card, () =>
                            {
                                GameManager.instance.OnPlayerDoCollect(Player, Card);
                                GameEvent.ButtonInteractable(true);
                            });
                        }
                        else
                        {
                            //...
                            GameEvent.ButtonInteractable(true);
                        }

                        m_ready = false;
                    }
                    break;
            }
        });
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
    }

    public void Open(float Duration, Action OnComplete)
    {
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
                    .OnComplete(() => OnComplete?.Invoke());
            });
    }

    public void Close(float Duration, Action OnComplete)
    {
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
                    .OnComplete(() => OnComplete?.Invoke());
            });
    }


    public void Shake(DirectorType Director)
    {
        this.transform.DOScale(Vector2.one * 1.2f, 0.02f).SetEase(Ease.OutExpo);
        //this.transform.DORotate(Vector3.up * )

        switch (Director)
        {
            case DirectorType.Up:
            case DirectorType.Down:
                break;
            case DirectorType.Left:
            case DirectorType.Right:
                break;
        }
    }


    public void Effect(CardEffectType Type, float Duration, Action OnComplete)
    {
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
                m_effect = false;
                OnComplete?.Invoke();
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