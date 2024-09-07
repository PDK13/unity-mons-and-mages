using DG.Tweening;
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
        GameEvent.onInitWild += OnInitWild;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;
        GameEvent.onInitWild -= OnInitWild;
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
            if (CardCheck.Name == CardNameType.Stage)
                continue;
            //
            var CardClone = Instantiate(m_cardSample, m_cardDeck);
            CardClone.SetActive(true);
            CardClone.name = "card-" + CardCheck.Name.ToString();
            CardClone.transform.localPosition = Vector3.zero;
            CardClone.GetComponent<CardController>().Init(CardCheck);
            switch (CardCheck.Name)
            {
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
                    CardClone.AddComponent<CardOneTail>();
                    break;
                default:
                    Debug.LogError("Wild init error card");
                    break;
            }
            CardClone.GetComponent<ICard>().Init(CardCheck);
        }
        //Suffle
        for (int i = 0; i < m_cardDeck.childCount; i++)
        {
            var IndexRandom = Random.Range(0, (m_cardDeck.childCount - 1) * 10) / 10;
            m_cardDeck.GetChild(i).SetSiblingIndex(m_cardDeck.GetChild(IndexRandom).GetSiblingIndex());
        }
        //Pos
        for (int i = 0; i < m_cardDeck.childCount; i++)
            m_cardDeck.GetChild(i).localPosition = Vector3.up * i * 2f;
    }

    private void OnInitWild()
    {
        StartCoroutine(WildFill());
    }

    //

    private IEnumerator WildFill()
    {
        GameEvent.CardFill();

        for (int i = 0; i < m_cardContent.childCount; i++)
        {
            var CardPoint = m_cardContent.GetChild(i);
            if (CardPoint.childCount == 0)
            {
                var CardTop = m_cardDeck.GetChild(m_cardDeck.childCount - 1);
                CardFill(CardTop, CardPoint);
                CardTop.GetComponent<CardController>().Open(0.5f, null);
                yield return new WaitForSeconds(0.5f);
            }
        }

        GameEvent.CardFillComplete();
    }

    private void CardFill(Transform Card, Transform Point)
    {
        Card.SetParent(Point, true);
        Card.DOLocalMove(Vector3.zero, 1).SetEase(Ease.OutQuad);
    }
}