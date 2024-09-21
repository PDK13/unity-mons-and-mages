using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int Index; //Index player on this device
    public bool Base; //Base player on this device
    [Min(1)] public int HealthPoint;
    [Min(0)] public int HealthCurrent;
    [Min(0)] public int RuneStone; //Use to get monsters from wild
    [Min(0)] public int StunPoint; //Max 3 to get 1 skip turn
    [Min(0)] public int StunCurrent;
    public List<ICard> CardQueue = new List<ICard>();
    [Min(0)] public int staffStep;
    public int[] Mediation = { 0, 0 };

    public IPlayer Player;

    public bool Stuned => StunCurrent >= StunPoint;

    public int staffStepNext => staffStep + 1 > CardQueue.Count - 1 ? 0 : staffStep + 1;

    public bool MediationEmty => Mediation[0] == 0 || Mediation[1] == 0;

    public PlayerData(int Index, bool Base, int HealthPoint, int RuneStone)
    {
        this.Index = Index;
        this.Base = Base;
        this.HealthPoint = HealthPoint;
        this.HealthCurrent = HealthPoint;
        this.RuneStone = RuneStone;
        this.StunPoint = 3;
        this.StunCurrent = 0;
    }
}

[Serializable]
public class CardData
{
    public string Named = "";
    public CardNameType Name;
    public CardOriginType Origin;
    public CardClassType Class;
    public Sprite Image;

    [Space]
    [Min(0)] public int RuneStoneCost;
    [Min(0)] public int RuneStoneTake; //When staff move to card, get Rune Stone

    [Space]
    [Min(0)] public int ManaPoint;
    [Min(0)] public int ManaStart;

    [Space]
    [Min(0)] public int AttackPoint;
    [Min(0)] public int GrowthStart;

    public IPlayer Player;
    public ICard Card;
}

[Serializable]
public class WildCardData
{
    public CardNameType Name;
    public Transform Card;
}