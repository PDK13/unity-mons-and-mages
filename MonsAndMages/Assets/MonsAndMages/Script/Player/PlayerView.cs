using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private GameObject m_playerSample;

    private float m_width;

    private void OnEnable()
    {
        GameEvent.onViewPlayer += OnViewPlayer;
    }

    private void OnDisable()
    {
        GameEvent.onViewPlayer -= OnViewPlayer;
    }

    private void Start()
    {
        OnInitPlayer(2);
    }

    private void OnInitPlayer(int Count)
    {
        if (Count < 1)
        {
            Debug.LogWarning("Init player count set to 1");
            Count = 1;
        }

        m_width = m_playerSample.GetComponent<RectTransform>().sizeDelta.x;
        for (int i = 1; i < Count; i++)
        {
            var PlayerClone = Instantiate(m_playerSample, this.transform);
            PlayerClone.transform.localPosition = Vector3.right * m_width;
        }

        GameManager.instance.PlayerJoin(transform.GetComponentsInChildren<IPlayer>());
    }

    private void OnViewPlayer(IPlayer Player, bool Update)
    {
        if (!Update)
        {
            this.transform
                .DOLocalMoveX(Player.PlayerIndex * -m_width, 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => GameEvent.ViewPlayer(Player, true));
        }
    }
}