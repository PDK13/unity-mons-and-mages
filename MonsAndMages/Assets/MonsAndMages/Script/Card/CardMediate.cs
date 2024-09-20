using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardMediate : MonoBehaviour
{
    private Button m_button;
    private GameObject m_mask;
    private GameObject m_renderer;
    private GameObject m_rendererAlpha;
    private Outline m_outline;
    private Transform m_runeStoneBox;
    private TextMeshProUGUI m_tmpRuneStone;

    private bool m_effect = false;

    //

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_mask = transform.Find("mask").gameObject;
        m_renderer = transform.Find("renderer").gameObject;
        m_rendererAlpha = transform.Find("alpha-mask").gameObject;
        m_outline = m_renderer.GetComponent<Outline>();
        m_tmpRuneStone = transform.Find("rune-stone.show").GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Start()
    {
        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        m_rendererAlpha.GetComponent<CanvasGroup>().alpha = 0;

        m_button.onClick.AddListener(BtnTap);
    }

    //

    public void BtnTap() { }

    //

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


    public void InfoRuneStoneUpdate(int Value, Action OnComplete)
    {
        EffectAlpha(() =>
        {
            m_runeStoneBox.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
            {
                m_tmpRuneStone.text = Value + GameConstant.TMP_ICON_RUNE_STONE;
                m_runeStoneBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
            });
        });
    }
}