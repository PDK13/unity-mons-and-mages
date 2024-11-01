using System;
using System.Collections;
using UnityEngine;

public class WildView : MonoBehaviour
{
    [SerializeField] private GameObject m_cardSample;
    [SerializeField] private Transform m_cardDeck;
    [SerializeField] private Transform m_cardContent;

    private bool m_wildFillFirstTime = true;

    public RectTransform PointerLast => m_cardContent.GetChild(m_cardContent.childCount - 1).GetComponent<RectTransform>();

    private void OnEnable()
    {
        GameEvent.onInit += OnInit;
        GameEvent.onEnd += OnEnd;
        GameEvent.onWildFill += OnWildFill;
    }

    private void OnDisable()
    {
        GameEvent.onInit -= OnInit;
        GameEvent.onEnd -= OnEnd;
        GameEvent.onWildFill -= OnWildFill;
    }

    private void Start()
    {
        m_cardSample.SetActive(false);
    }

    //

    private void OnInit()
    {
        var Tutorial = GameManager.instance.TutorialActive;

        //Generate
        foreach (var CardCheck in GameManager.instance.CardConfig.Card)
        {
            if (Tutorial)
                if (!GameManager.instance.TutorialWild.Exists(t => t == CardCheck.Name))
                    continue;
            //
            var CardClone = Instantiate(m_cardSample, m_cardDeck);
            CardClone.SetActive(true);
            CardClone.name = "card-" + CardCheck.Name.ToString();
            CardClone.transform.localPosition = Vector3.zero;
            CardClone.AddComponent<CardController>(); //Card controller required for every event!
            CardClone.GetComponent<ICard>().Init(CardCheck);
        }
        //Suffle
        for (int i = 0; i < m_cardDeck.childCount; i++)
        {
            if (Tutorial)
            {
                var CardName = m_cardDeck.GetChild(i).GetComponent<ICard>().Name;
                m_cardDeck.GetChild(i).SetSiblingIndex(GameManager.instance.TutorialWild.FindIndex(t => t == CardName));
            }
            else
            {
                var IndexRandom = UnityEngine.Random.Range(0, (m_cardDeck.childCount - 1) * 10) / 10;
                m_cardDeck.GetChild(i).SetSiblingIndex(m_cardDeck.GetChild(IndexRandom).GetSiblingIndex());
            }
        }
        //Pos
        for (int i = 0; i < m_cardDeck.childCount; i++)
            m_cardDeck.GetChild(i).localPosition = Vector3.up * i * 2f;
    }

    //

    private void OnEnd()
    {
        StopAllCoroutines();

        for (int i = 0; i < m_cardDeck.childCount; i++)
            Destroy(m_cardDeck.GetChild(i).gameObject);

        for (int i = 0; i < m_cardContent.childCount; i++)
            for (int j = 0; j < m_cardContent.GetChild(i).childCount; j++)
                Destroy(m_cardContent.GetChild(i).GetChild(j).gameObject);

        m_wildFillFirstTime = true;
    }

    //

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
            if (!m_wildFillFirstTime)
            {
                if (GetCardIndexExist(i))
                    continue;
            }

            var Centre = m_cardContent.GetChild(i).GetComponent<RectTransform>();

            var Card = m_cardDeck.GetChild(m_cardDeck.childCount - 1).GetComponent<ICard>();
            Card.DoFill(PointerLast, Centre);

            if (m_cardDeck.childCount == 0)
                break;

            yield return new WaitForSeconds(MoveDuration);
        }
        if (m_wildFillFirstTime)
            yield return new WaitForSeconds(2f);

        OnComplete?.Invoke();

        m_wildFillFirstTime = false;
    }

    private bool GetCardIndexExist(int Index)
    {
        for (int i = 0; i < PointerLast.childCount; i++)
        {
            if (PointerLast.GetChild(i).GetComponent<ICard>().Index != Index)
                continue;
            return true;
        }
        return false;
    }
}