using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldView : MonoBehaviour
{
    [SerializeField] private GameObject m_playerSample;
    [SerializeField] private RectTransform m_playerContent;

    private void OnEnable()
    {
        GameEvent.onInitPlayer += OnInitPlayer;
    }

    private void OnDisable()
    {
        GameEvent.onInitPlayer -= OnInitPlayer;
    }

    private void OnInitPlayer(PlayerData[] Player)
    {
        foreach (var PlayerCheck in Player)
        {
            var PlayerClone = Instantiate(m_playerSample, m_playerContent);
            PlayerClone.transform.localPosition = Vector3.right * m_playerContent.sizeDelta.x;
            PlayerClone.GetComponent<PlayerController>().Init(PlayerCheck);
        }
    }
}