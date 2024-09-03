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

    public bool MediationEmty => m_data.MediationEmty;

    //

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

    //

    public void DoChoice() { }

    public void DoMediate(int RuneStoneAdd)
    {
        if (m_data.Mediation[0] == 0)
            m_data.Mediation[0] = RuneStoneAdd * 2;
        if (m_data.Mediation[1] == 0)
            m_data.Mediation[1] = RuneStoneAdd * 2;
    }

    public void DoCollect(ICard CardData)
    {
        m_data.RuneStone -= CardData.RuneStoneCost;

        if (m_data.CardQueue.Count < 5 || CardQueue[0] != null)
            m_data.CardQueue.Add(CardData);
        else
        {
            m_data.CardQueue.RemoveAt(0);
            m_data.CardQueue.Add(CardData);
        }
    }

    //

    public void DoWandNext()
    {
        m_data.WandStep = m_data.WandStepNext;
    }

    public void DoWandActive()
    {
        CardQueue[WandStep].DoWandActive();
    }

    //

    public void DoContinueCheck(IPlayer Player) { }

    public void DoContinue(IPlayer Player) { }

    //

    public void DoEnd(IPlayer Player) { }

    //

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