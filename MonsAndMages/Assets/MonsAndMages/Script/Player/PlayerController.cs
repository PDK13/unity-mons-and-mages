using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] private GameObject m_cardPointSample;
    [SerializeField] private Transform m_cardContent;
    [SerializeField] private Transform m_wand;

    private PlayerData m_data;

    private void Start()
    {
        m_cardPointSample.SetActive(false);
    }

    //IPlayer

    public int Index => m_data.Index;

    public bool Base => m_data.Base;

    public int HealthPoint => m_data.HealthPoint;

    public int RuneStone => m_data.RuneStone;

    public int StunPoint => m_data.StunPoint;

    public int StunCurrent => m_data.StunCurrent;

    public bool Stuned => m_data.Stuned;

    public List<ICard> CardQueue => m_data.CardQueue;

    public int WandStep => m_data.Index;

    public int[] Mediation => m_data.Mediation;

    public bool MediationEmty => m_data.MediationEmty;

    public PlayerController Controller => this;

    //

    public void Init(PlayerData Data)
    {
        m_data = Data;
        m_data.Player = this;
        //
        for (int i = 0; i < m_cardContent.childCount; i++)
            m_data.CardQueue.Add(m_cardContent.GetChild(i).GetComponentInChildren<ICard>());
        //
        GameManager.instance.PlayerJoin(this);
    }


    public void DoTakeRuneStoneFromSupply(int Value)
    {
        if (Value <= 0)
        {
            Debug.LogErrorFormat("{0} Rune Stone get from supply set to 1", Value);
            Value = 1;
        }
        m_data.RuneStone += 1;
    }

    public void DoTakeRuneStoneFromMediation()
    {
        for (int i = 0; i < m_data.Mediation.Length; i++)
        {
            if (m_data.Mediation[i] > 0)
            {
                m_data.RuneStone += 2;
                m_data.Mediation[i] -= 2;
            }
        }
    }

    public void DoStunnedCheck() { }


    public void DoChoice() { }

    public void DoMediate(int RuneStoneAdd)
    {
        if (m_data.Mediation[0] == 0)
            m_data.Mediation[0] = RuneStoneAdd * 2;
        if (m_data.Mediation[1] == 0)
            m_data.Mediation[1] = RuneStoneAdd * 2;
    }

    public Transform DoCollectReady()
    {
        if (m_data.CardQueue.Count >= 5 && CardQueue[0].Name == CardNameType.Stage)
        {
            Destroy(m_cardContent.GetChild(0).gameObject);
            m_data.CardQueue.RemoveAt(0);
        }

        var CardPoint = Instantiate(m_cardPointSample, m_cardContent);
        CardPoint.SetActive(true);
        CardPoint.name = "card-point";

        return CardPoint.transform;
    }

    public void DoCollect(ICard Card)
    {
        m_data.RuneStone -= Card.RuneStoneCost;
        m_data.CardQueue.Add(Card);
    }


    public void DoWandNext(Action OnComplete)
    {
        var WandIndexLast = m_data.WandStep;
        var WandIndexNext = m_data.WandStepNext;

        m_data.WandStep = WandIndexNext;

        var PointLast = m_cardContent.GetChild(WandIndexLast);
        var PointNext = m_cardContent.GetChild(WandIndexNext);

        m_wand.SetParent(PointNext, this.transform);
        m_wand
            .DOLocalJump(Vector3.zero, 15f, 1, 1f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => OnComplete?.Invoke());
    }

    public void DoWandActive(Action OnComplete)
    {
        var Card = m_cardContent.GetChild(WandStep).GetComponentInChildren<ICard>();
        Card.EffectAlpha(1f, () =>
        {
            Card.DoWandActive(() => OnComplete?.Invoke());
        });
    }


    public void DoContinueCheck(IPlayer Player) { }

    public void DoContinue(IPlayer Player) { }


    public void DoEnd(IPlayer Player) { }


    public void StunChange(int Value)
    {
        m_data.StunCurrent += Value;
        m_data.StunCurrent = Mathf.Clamp(m_data.StunCurrent, 0, m_data.StunPoint);
    }

    public void HealthChange(int Value)
    {
        m_data.HealthCurrent += Value;
        m_data.HealthCurrent = Mathf.Clamp(m_data.HealthCurrent, 0, m_data.HealthPoint);
    }
}