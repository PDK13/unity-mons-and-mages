using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameView : MonoBehaviour
{
    [SerializeField] private RectTransform m_playerContent;
    [SerializeField] private RectTransform m_gameContent;

    private void OnEnable()
    {
        GameEvent.onViewPlayer += OnViewPlayer;
        GameEvent.onViewCollect += OnViewCollect;
        GameEvent.onViewBack += OnViewBack;
    }

    private void OnDisable()
    {
        GameEvent.onViewPlayer -= OnViewPlayer;
        GameEvent.onViewCollect -= OnViewCollect;
        GameEvent.onViewBack -= OnViewBack;
    }

    private void OnViewPlayer(IPlayer Player, bool Update)
    {
        if (!Update)
        {
            m_playerContent
                .DOLocalMoveX(Player.PlayerIndex * -m_playerContent.sizeDelta.x, 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => GameEvent.ViewPlayer(Player, true));
        }
    }

    private void OnViewCollect(bool Update)
    {
        if (!Update)
        {
            m_gameContent
                .DOLocalMoveY(-m_gameContent.sizeDelta.y, 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => GameEvent.ViewCollect(true));
        }
    }

    private void OnViewBack(bool Update)
    {
        if (!Update)
        {
            m_gameContent
                .DOLocalMoveY(0f, 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => GameEvent.ViewBack(true));
        }
    }
}