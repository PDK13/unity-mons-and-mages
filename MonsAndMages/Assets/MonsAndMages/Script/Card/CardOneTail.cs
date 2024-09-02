using UnityEngine;

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

    public void Init(CardData Data)
    {
        m_data = Data;
    }

    //

    public void DoCollectActive(IPlayer Player) { }

    public void OriginActive(IPlayer Player) { }

    public void EnterActive(IPlayer Player) { }

    public void PassiveActive(IPlayer Player) { }

    //

    public void WandActive(IPlayer Player) { }

    public void AttackActive(IPlayer Player) { }

    public void EnergyFill(IPlayer Player, int Value)
    {
        m_data.EnergyCurrent += Value;
    }

    public void EnergyCheck() { }

    //

    public void EnergyActive(IPlayer Player)
    {
        m_data.EnergyCurrent -= m_data.EnergyPoint;
    }

    public void ClassActive(IPlayer Player) { }

    public void SpellActive(IPlayer Player) { } //Update...!
}