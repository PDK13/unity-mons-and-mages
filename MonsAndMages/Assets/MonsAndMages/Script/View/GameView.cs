using DG.Tweening;
using System;
using UnityEngine;

public class GameView : MonoBehaviour
{
    public static GameView instance;

    [SerializeField] private RectTransform m_playerContent;
    [SerializeField] private RectTransform m_gameContent;

    private ViewType m_viewType = ViewType.None;
    private bool m_viewChange = false;

    public ViewType ViewType => m_viewType;

    //

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onEnd += OnEnd;

        GameEvent.onViewArea += OnView;
        GameEvent.onViewPlayer += OnViewPlayer;

        GameEvent.onCardRumble += OnCardRumble;
    }

    private void OnDisable()
    {
        GameEvent.onInitPlayer -= OnInitPlayer;

        GameEvent.onEnd -= OnEnd;

        GameEvent.onViewArea += OnView;
        GameEvent.onViewPlayer -= OnViewPlayer;

        GameEvent.onCardRumble -= OnCardRumble;
    }

    //

    private void OnInitPlayer(PlayerData[] Player)
    {
        for (int i = 0; i < Player.Length; i++)
        {
            if (!Player[i].Base)
                continue;
            m_playerContent.localPosition = Vector3.right * (Player[i].Index * -m_playerContent.sizeDelta.x);
        }
    }


    private void OnEnd()
    {
        m_viewType = ViewType.None;
        m_viewChange = false;
        this.transform.DOKill();
        m_playerContent.DOKill();
        m_gameContent.DOKill();
        m_gameContent.localPosition = Vector3.zero;
    }


    private void OnView(ViewType Type, Action OnComplete)
    {
        if (m_viewType == Type)
        {
            if (!m_viewChange)
                OnComplete?.Invoke();
            return;
        }
        m_viewType = Type;
        m_viewChange = true;

        var MoveYDuration = GameManager.instance.TweenConfig.GameView.MoveYDuration;
        var MoveYEase = GameManager.instance.TweenConfig.GameView.MoveYEase;

        switch (Type)
        {
            case ViewType.Field:
                GameEvent.UiChoiceHide();
                m_gameContent
                    .DOLocalMoveY(0f, MoveYDuration)
                    .SetEase(MoveYEase)
                    .OnComplete(() =>
                    {
                        m_viewChange = false;
                        GameEvent.UiChoiceCurrent();
                        OnComplete?.Invoke();
                    });
                break;
            case ViewType.Wild:
                GameEvent.UiChoiceHide();
                m_gameContent
                    .DOLocalMoveY(-m_gameContent.sizeDelta.y, MoveYDuration)
                    .SetEase(MoveYEase)
                    .OnComplete(() =>
                    {
                        m_viewChange = false;
                        GameEvent.UiChoiceCurrent();
                        OnComplete?.Invoke();
                    });
                break;
        }
    }

    private void OnViewPlayer(IPlayer Player, Action OnComplete)
    {
        var MoveXDuration = GameManager.instance.TweenConfig.GameView.MoveXDuration;
        var MoveXEase = GameManager.instance.TweenConfig.GameView.MoveXEase;

        m_playerContent
            .DOLocalMoveX(Player.Index * -m_playerContent.sizeDelta.x, MoveXDuration)
            .SetEase(MoveXEase)
            .OnComplete(() => OnComplete?.Invoke());
    }


    private void OnCardRumble(ICard Card, Action OnComplete)
    {
        var PosCurrent = this.GetComponent<RectTransform>().localPosition;
        this.transform.DOShakePosition(0.5f, 10f, 50).OnComplete(() =>
        {
            this.transform.localPosition = PosCurrent;
            OnComplete?.Invoke();
        });
    }
}