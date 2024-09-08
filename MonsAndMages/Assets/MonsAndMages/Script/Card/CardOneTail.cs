using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardOneTail : MonoBehaviour, ICard
{
    private CardData m_data;
    private CardController m_controller;

    //ICard

    public CardNameType Name => m_data.Name;

    public CardOriginType Origin => m_data.Origin;

    public CardClassType Class => m_data.Class;

    public int RuneStoneCost => m_data.RuneStoneCost;

    public int Energy => m_data.ManaPoint;

    public int EnergyCurrent => m_data.ManaCurrent;

    public bool EnergyFull => m_data.ManaCurrent >= m_data.ManaPoint;

    public int Attack => m_data.AttackPoint;

    public int Grow => m_data.GrowCurrent;

    public int AttackCombine => m_data.AttackCombine;

    public IPlayer Player => m_data.Player;

    public CardController Controller => this.GetComponent<CardController>();

    public void Init(CardData Data)
    {
        m_data = Data;
    }

    //

    public void DoCollectActive(IPlayer Player)
    {
        m_data.Player = Player;
    }

    public void DoOriginActive() { }

    public void DoEnterActive() { }

    public void DoPassiveActive() { }

    //

    public void DoWandActive() { }

    public void DoAttackActive() { }

    public void DoEnergyFill(int Value)
    {
        m_data.ManaCurrent += Value;
    }

    public void DoEnergyCheck() { }

    //

    public void DoEnergyActive()
    {
        m_data.ManaCurrent -= m_data.ManaPoint;
    }

    public void DoClassActive() { }

    public void DoSpellActive() { } //Update...!
}