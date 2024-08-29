using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int PlayerIndex;
    public int HealthPoint;
    public int RuneStone; //Use to get monsters from wild
    public int StunPoint; //Max 3 to get 1 skip turn
    public bool Stuned;
    public List<CardData> CardQueue = new List<CardData>();
    public int WandStep;
    public int[] Mediation = { 0, 0 };

    public PlayerData(int PlayerIndex, int HealthPoint, int RuneStone)
    {
        this.PlayerIndex = PlayerIndex;
        this.HealthPoint = HealthPoint;
        this.RuneStone = RuneStone;
        this.StunPoint = 0;
        this.CardQueue = new List<CardData>()
        {
            null,
            null,
            null,
        };
        this.WandStep = 0;
    }

    //Start

    public void TurnStart()
    {
        //1.Take 1 rune stone from supply
        RuneStone += 1;
        //2.Take rune stone from mediation
        for (int i = 0; i < Mediation.Length; i++)
        {
            if (Mediation[i] > 0)
            {
                RuneStone += 2;
                Mediation[i] -= 2;
            }
        }
        //3.Check stun stage
        if (Stuned)
            DoWandNext(false);
        else
            GameEvent.PlayerReady(PlayerIndex); //4.Player start their choice
    }

    //Choice

    public bool DoMediate(int RuneStoneAdd)
    {
        if (Mediation[0] > 0 && Mediation[1] > 0)
            return false;

        if (Mediation[0] == 0)
            Mediation[0] = RuneStoneAdd * 2;
        if (Mediation[1] == 0)
            Mediation[1] = RuneStoneAdd * 2;

        GameEvent.PlayerMediate(PlayerIndex);

        return true;
    }

    public bool DoCollect(CardData CardData)
    {
        if (RuneStone < CardData.RuneStoneCost)
            return false;

        RuneStone -= CardData.RuneStoneCost;

        if (CardQueue.Count < 5 || CardQueue[0] != null)
            CardQueue.Add(CardData);
        else
        {
            CardQueue.RemoveAt(0);
            CardQueue.Add(CardData);
        }

        GameEvent.PlayerCollect(PlayerIndex, CardData);
        GameEvent.CardCollectActive(PlayerIndex, CardData);

        return true;
    }

    public void DoWandNext(bool CardActive = true)
    {
        WandStep += 1;
        if (WandStep > CardQueue.Count - 1)
            WandStep = 0;
        if (CardActive)
            CardQueue[WandStep].DoWandActive(PlayerIndex);
        GameEvent.PlayerWandNext(PlayerIndex);
    }

    //Opponent

    public void TakeStun(int Value = 1)
    {
        StunPoint += Value;
        if (StunPoint >= 3)
        {
            StunPoint = 0;
            Stuned = true;
        }
    }

    public void TakeDamage(int Value)
    {
        HealthPoint -= Value;
    }

    public void TakeHeal(int Value)
    {
        HealthPoint += Value;
    }
}

[Serializable]
public class CardData
{
    public CardNameType Name;
    public CardOriginType Origin;
    public CardClassType Class;
    [Min(0)] public int RuneStoneCost;
    [Min(0)] public int Energy;
    [Min(0)] public int Attack;
    public Sprite Image;

    public bool DoWandActive(int PlayerIndex)
    {
        DoCardActive();
        GameEvent.CardWandActive(PlayerIndex, this);
        return true;
    }

    protected virtual void DoCardActive() { } //Every cards have their own unique active
}

[Serializable]
public class WildCardData
{
    public CardNameType Name;
    public Transform Card;
}