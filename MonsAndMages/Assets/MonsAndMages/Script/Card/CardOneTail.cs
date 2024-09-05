using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardOneTail : MonoBehaviour, ICard
{
    private CardData m_data;

    //ICard

    public CardNameType Name => m_data.Name;

    public CardOriginType Origin => m_data.Origin;

    public CardClassType Class => m_data.Class;

    public int RuneStoneCost => m_data.RuneStoneCost;

    public int Energy => m_data.EnergyPoint;

    public int EnergyCurrent => m_data.EnergyCurrent;

    public bool EnergyFull => m_data.EnergyCurrent >= m_data.EnergyPoint;

    public int Attack => m_data.AttackPoint;

    public int Grow => m_data.GrowCurrent;

    public int AttackCombine => m_data.AttackPoint + m_data.GrowCurrent;

    public IPlayer Player => m_data.Player;

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
        m_data.EnergyCurrent += Value;
    }

    public void DoEnergyCheck() { }

    //

    public void DoEnergyActive()
    {
        m_data.EnergyCurrent -= m_data.EnergyPoint;
    }

    public void DoClassActive() { }

    public void DoSpellActive() { } //Update...!
}