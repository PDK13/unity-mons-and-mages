using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private RectTransform m_playerContent;
    [SerializeField] private RectTransform m_gameContent;

    private void OnEnable()
    {
        GameEvent.onInitPlayer += OnInitPlayer;

        GameEvent.onViewPlayer += OnViewPlayer;
        GameEvent.onViewWild += OnViewCollect;
        GameEvent.onViewField += OnViewBack;
    }

    private void OnDisable()
    {
        GameEvent.onInitPlayer -= OnInitPlayer;

        GameEvent.onViewPlayer -= OnViewPlayer;
        GameEvent.onViewWild -= OnViewCollect;
        GameEvent.onViewField -= OnViewBack;
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


    private void OnViewPlayer(IPlayer Player, Action OnComplete)
    {
        m_playerContent.DOLocalMoveX(Player.Index * -m_playerContent.sizeDelta.x, 1f).SetEase(Ease.OutQuad).OnComplete(() => OnComplete?.Invoke());
    }

    private void OnViewCollect(Action OnComplete)
    {
        m_gameContent.DOLocalMoveY(-m_gameContent.sizeDelta.y, 1f).SetEase(Ease.OutQuad).OnComplete(() => OnComplete?.Invoke());
    }

    private void OnViewBack(Action OnComplete)
    {
        m_gameContent.DOLocalMoveY(0f, 1f).SetEase(Ease.OutQuad).OnComplete(() => OnComplete?.Invoke());
    }
}