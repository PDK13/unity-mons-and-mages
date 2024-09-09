using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WildView : MonoBehaviour
{
    [SerializeField] private GameObject m_cardSample;
    [SerializeField] private Transform m_cardDeck;
    [SerializeField] private Transform m_cardContent;

    private void OnEnable()
    {
        GameEvent.onInit += OnInit;
        GameEvent.onWildFill += OnWildFill;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;
        GameEvent.onWildFill -= OnWildFill;
    }

    private void Start()
    {
        m_cardSample.SetActive(false);
    }

    private void OnInit()
    {
        //Generate
        foreach (var CardCheck in GameManager.instance.CardConfig.Card)
        {
            switch (CardCheck.Name)
            {
                case CardNameType.Stage:
                    Debug.LogWarning("Not init Stage card in wild");
                    continue;
                case CardNameType.Cornibus:
                case CardNameType.Duchess:
                case CardNameType.DragonEgg:
                case CardNameType.Eversor:
                case CardNameType.FlowOfTheEssential:
                case CardNameType.Forestwing:
                case CardNameType.PixieSGrove:
                case CardNameType.OneTail:
                case CardNameType.Pott:
                case CardNameType.Umbella:
                    var CardClone = Instantiate(m_cardSample, m_cardDeck);
                    CardClone.SetActive(true);
                    CardClone.name = "card-" + CardCheck.Name.ToString();
                    CardClone.transform.localPosition = Vector3.zero;
                    CardClone.GetComponent<CardController>().Init(CardCheck);
                    CardClone.AddComponent<CardOneTail>();
                    CardClone.GetComponent<ICard>().Init(CardCheck);
                    break;
                default:
                    Debug.LogError("Not found card to init in wild");
                    break;
            }
        }
        //Suffle
        for (int i = 0; i < m_cardDeck.childCount; i++)
        {
            var IndexRandom = UnityEngine.Random.Range(0, (m_cardDeck.childCount - 1) * 10) / 10;
            m_cardDeck.GetChild(i).SetSiblingIndex(m_cardDeck.GetChild(IndexRandom).GetSiblingIndex());
        }
        //Pos
        for (int i = 0; i < m_cardDeck.childCount; i++)
            m_cardDeck.GetChild(i).localPosition = Vector3.up * i * 2f;
    }

    private void OnWildFill()
    {
        StartCoroutine(IEWildFill());
    }

    private IEnumerator IEWildFill()
    {
        GameEvent.CardFill();

        for (int i = 0; i < m_cardContent.childCount; i++)
        {
            var CardPoint = m_cardContent.GetChild(i);
            switch (CardPoint.childCount)
            {
                case 0:
                    var CardTop = m_cardDeck.GetChild(m_cardDeck.childCount - 1);
                    CardFill(CardTop, CardPoint);
                    yield return new WaitForSeconds(0.5f);
                    break;
                case 1:
                    continue;
                default:
                    Debug.LogError("Wild fill found 2 game object in card point " + i);
                    break;
            }
        }

        GameEvent.CardFillComplete();
    }

    private void CardFill(Transform Card, Transform Point)
    {
        Card.SetParent(Point, true);
        Card.DOLocalMove(Vector3.zero, 1).SetEase(Ease.OutQuad);
        Card.GetComponent<CardController>().Open(0.5f, null);
    }
}