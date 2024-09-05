using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private GameObject m_playerSample;
    [SerializeField] private RectTransform m_playerContent;

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

        for (int i = 1; i < Count; i++)
        {
            var PlayerClone = Instantiate(m_playerSample, m_playerContent);
            PlayerClone.transform.localPosition = Vector3.right * m_playerContent.sizeDelta.x;
        }

        GameManager.instance.PlayerJoin(transform.GetComponentsInChildren<IPlayer>());
    }
}