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
        GameEvent.onEnd += OnEnd;
    }

    private void OnDisable()
    {
        GameEvent.onInitPlayer -= OnInitPlayer;
        GameEvent.onEnd -= OnEnd;
    }

    private void Start()
    {
        m_playerSample.SetActive(false);
    }

    //

    private void OnInitPlayer(PlayerData[] Player)
    {
        for (int i = 0; i < Player.Length; i++)
        {
            var PlayerClone = Instantiate(m_playerSample, m_playerContent);
            PlayerClone.SetActive(true);
            PlayerClone.transform.localPosition = Vector3.right * m_playerContent.sizeDelta.x * i;
            PlayerClone.GetComponent<PlayerController>().Init(Player[i]);
        }
    }

    private void OnEnd()
    {
        for (int i = 0; i < m_playerContent.childCount; i++)
            Destroy(m_playerContent.GetChild(i).gameObject);
    }
}