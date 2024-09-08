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
    [SerializeField] private TextMeshProUGUI m_tmpGrow;
    [SerializeField] private TextMeshProUGUI m_tmpMana;
    [SerializeField] private TextMeshProUGUI m_tmpDamage;

    private ICard m_card;
    private Tweener m_tween;

    public ICard Card
    {
        get
        {
            if (m_card == null)
                m_card = GetComponent<ICard>();
            return m_card;
        }
    }

    //

    public void BtnTap()
    {
        Debug.Log(Card.Name + " Card Tap");
        GameEvent.CardTap(Card, true);
        //Test
        GameEvent.PlayerDoCollect(GameManager.instance.PlayerCurrent, Card, true);
    }

    //

    public void Init(CardData Card)
    {
        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        m_renderer.GetComponent<Image>().sprite = Card.Image;
        InfoShow(false);
        InfoGrowUpdate(Card.GrowCurrent);
        InfoManaUpdate(Card.ManaCurrent, Card.ManaPoint);
        InfoDamageUpdate(Card.AttackCombine);
    }

    public void Open(float Duration, Action OnComplete)
    {
        m_tween.Kill();
        this.transform.eulerAngles = Vector3.zero;
        m_mask.SetActive(true);
        m_renderer.SetActive(false);
        m_tween = this.transform
            .DOLocalRotate(Vector3.up * 90f, Duration / 2)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_mask.SetActive(false);
                m_renderer.SetActive(true);
                m_tween = this.transform
                    .DOLocalRotate(Vector3.zero, Duration / 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => OnComplete?.Invoke());
            });
    }

    public void Close(float Duration, Action OnComplete)
    {
        m_tween.Kill();
        this.transform.eulerAngles = Vector3.zero;
        m_mask.SetActive(false);
        m_renderer.SetActive(true);
        m_tween = this.transform
            .DOLocalRotate(Vector3.up * 90f, Duration / 2)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                m_mask.SetActive(true);
                m_renderer.SetActive(false);
                m_tween = this.transform
                    .DOLocalRotate(Vector3.zero, Duration / 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => OnComplete?.Invoke());
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