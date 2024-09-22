using DG.Tweening;
using System;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private RectTransform m_playerContent;
    [SerializeField] private RectTransform m_gameContent;

    private ViewType m_viewType = ViewType.None;
    private bool m_viewChange = false;

    private void OnEnable()
    {
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onViewArea += OnView;
        GameEvent.onViewPlayer += OnViewPlayer;

        GameEvent.onCardRumble += OnCardRumble;
    }

    private void OnDisable()
    {
        GameEvent.onInitPlayer -= OnInitPlayer;

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
                GameEvent.ShowUiArea(ViewType.Field, false);
                m_gameContent
                    .DOLocalMoveY(0f, MoveYDuration)
                    .SetEase(MoveYEase)
                    .OnComplete(() =>
                    {
                        GameEvent.ShowUiArea(ViewType.Field, true);
                        m_viewChange = false;
                        OnComplete?.Invoke();
                    });
                break;
            case ViewType.Wild:
                GameEvent.ShowUiArea(ViewType.Wild, false);
                m_gameContent
                    .DOLocalMoveY(-m_gameContent.sizeDelta.y, MoveYDuration)
                    .SetEase(MoveYEase)
                    .OnComplete(() =>
                    {
                        GameEvent.ShowUiArea(ViewType.Wild, true);
                        m_viewChange = false;
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