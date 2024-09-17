using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private RectTransform m_playerContent;
    [SerializeField] private RectTransform m_gameContent;

    private ViewType m_viewType = ViewType.None;

    private void OnEnable()
    {
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onView += OnView;
        GameEvent.onViewPlayer += OnViewPlayer;

        GameEvent.onCardRumble += OnCardRumble;
    }

    private void OnDisable()
    {
        GameEvent.onInitPlayer -= OnInitPlayer;

        GameEvent.onView += OnView;
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
            OnComplete?.Invoke();
            return;
        }

        m_viewType = Type;

        GameEvent.ViewUiHide();
        switch (Type)
        {
            case ViewType.Field:
                m_gameContent
                    .DOLocalMoveY(0f, 1f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        GameEvent.ViewUiShow(ViewType.Field);
                        OnComplete?.Invoke();
                    });
                break;
            case ViewType.Wild:
                m_gameContent
                    .DOLocalMoveY(-m_gameContent.sizeDelta.y, 1f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        GameEvent.ViewUiShow(ViewType.Wild);
                        OnComplete?.Invoke();
                    });
                break;
        }
    }

    private void OnViewPlayer(IPlayer Player, Action OnComplete)
    {
        m_playerContent
            .DOLocalMoveX(Player.Index * -m_playerContent.sizeDelta.x, 1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => OnComplete?.Invoke());
    }


    private void OnCardRumble(ICard Card, Action OnComplete)
    {
        var PosCurrent = this.GetComponent<RectTransform>().localPosition;
        this.transform.DOShakePosition(0.5f, 10, 50).OnComplete(() =>
        {
            this.transform.localPosition = PosCurrent;
            OnComplete?.Invoke();
        });
    }
}