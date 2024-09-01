using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    private PlayerData m_data;

    //IPlayer

    public int PlayerIndex => m_data.PlayerIndex;

    public int HealthPoint => m_data.HealthPoint;

    public int RuneStone => m_data.RuneStone;

    public int StunPoint => m_data.PlayerIndex;

    public int StunCurrent => m_data.StunCurrent;

    public bool Stuned => m_data.Stuned;

    public List<ICard> CardQueue => m_data.CardQueue;

    public int WandStep => m_data.PlayerIndex;

    public int[] Mediation => m_data.Mediation;

    //

    public void TakeRuneStoneFromSupply(int Value)
    {
        if (Value <= 0)
        {
            Debug.LogErrorFormat("{0} Rune Stone get from supply set to 1", Value);
            Value = 1;
        }
        m_data.RuneStone += 1;
    }

    public void TakeRuneStoneFromMediation()
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

    public void CheckStunned() { }

    //

    public void DoMediate(int RuneStoneAdd)
    {
        if (m_data.Mediation[0] > 0 && m_data.Mediation[1] > 0)
            return;

        if (m_data.Mediation[0] == 0)
            m_data.Mediation[0] = RuneStoneAdd * 2;
        if (m_data.Mediation[1] == 0)
            m_data.Mediation[1] = RuneStoneAdd * 2;

        GameEvent.PlayerDoMediate(this);
    }

    public void DoCollect(ICard CardData)
    {
        if (m_data.RuneStone < CardData.RuneStoneCost)
            return;

        m_data.RuneStone -= CardData.RuneStoneCost;

        if (m_data.CardQueue.Count < 5 || CardQueue[0] != null)
            m_data.CardQueue.Add(CardData);
        else
        {
            m_data.CardQueue.RemoveAt(0);
            m_data.CardQueue.Add(CardData);
        }

        GameEvent.PlayerDoCollect(this);
    }

    public void DoCardAbilityOriginActive() { }

    //

    public void DoWandNext()
    {
        m_data.WandStep = m_data.WandStepNext;
    }

    public void DoWandActive() { }

    public void DoCardAttack() { }

    public void DoCardEnergyFill() { }

    public void DoCardEnergyCheck() { }

    public void DoCardEnergyActive() { }

    public void DoCardAbilityClassActive() { }

    public void DoCardAbilitySpellActive() { }

    //

    public void TakeStun(int Value = 1)
    {
        if (Value <= 0)
        {
            Debug.LogErrorFormat("{0} Stun set to 1", Value);
            Value = 1;
        }
        m_data.StunPoint += Value;
        if (StunPoint >= 3)
        {
            m_data.StunPoint = 0;
            m_data.Stuned = true;
        }
    }

    public void TakeDamage(int Value)
    {
        if (Value <= 0)
        {
            Debug.LogErrorFormat("{0} Damage set to 1", Value);
            Value = 1;
        }
        m_data.HealthPoint -= Value;
    }

    public void DoHeal(int Value)
    {
        if (Value <= 0)
        {
            Debug.LogErrorFormat("{0} Heal set to 1", Value);
            Value = 1;
        }
        m_data.HealthPoint += Value;
    }
}