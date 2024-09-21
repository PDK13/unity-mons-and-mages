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

    private bool m_wildFillFirstTime = true;

    private void OnEnable()
    {
        GameEvent.onInit += OnInit;

        GameEvent.onPlayerStart += OnPlayerStart;

        GameEvent.onWildFill += OnWildFill;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;

        GameEvent.onPlayerStart -= OnPlayerStart;

        GameEvent.onWildFill -= OnWildFill;
    }

    private void Start()
    {
        m_cardSample.SetActive(false);
    }

    //

    private void OnInit()
    {
        //Generate
        foreach (var CardCheck in GameManager.instance.CardConfig.Card)
        {
            var CardClone = Instantiate(m_cardSample, m_cardDeck);
            CardClone.SetActive(true);
            CardClone.name = "card-" + CardCheck.Name.ToString();
            CardClone.transform.localPosition = Vector3.zero;

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
                    CardClone.AddComponent<CardController>();
                    break;
                default:
                    Destroy(CardClone);
                    Debug.LogError("Not found card to init in wild");
                    continue;
            }

            CardClone.GetComponent<ICard>().Init(CardCheck);
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


    private void OnPlayerStart(IPlayer Player, Action OnComplete)
    {
        for (int i = 0; i < m_cardContent.childCount; i++)
        {
            var Card = m_cardContent.GetChild(i).GetComponentInChildren<ICard>();
            if (Card != null)
                Card.Ready();
        }
    }


    private void OnWildFill(Action OnComplete)
    {
        if (m_cardDeck.childCount == 0)
        {
            OnComplete?.Invoke();
            return;
        }
        StartCoroutine(IEWildFill(OnComplete));
    }

    private IEnumerator IEWildFill(Action OnComplete)
    {
        if (m_wildFillFirstTime)
            yield return new WaitForSeconds(2f);

        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration + 0.02f;

        for (int i = 0; i < m_cardContent.childCount; i++)
        {
            var CardPoint = m_cardContent.GetChild(i);
            if (CardPoint.childCount != 0)
                continue;

            m_cardDeck.GetChild(m_cardDeck.childCount - 1).GetComponent<ICard>().Fill(CardPoint);

            if (m_cardDeck.childCount == 0)
                break;

            yield return new WaitForSeconds(MoveDuration);
        }
        if (m_wildFillFirstTime)
            yield return new WaitForSeconds(2f);

        OnComplete?.Invoke();

        m_wildFillFirstTime = false;
    }
}