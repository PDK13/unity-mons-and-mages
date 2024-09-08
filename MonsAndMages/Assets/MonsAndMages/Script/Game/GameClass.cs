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
    [Min(0)] public int WandStep;
    public int[] Mediation = { 0, 0 };

    public bool Stuned => StunCurrent >= StunPoint;

    public int WandStepNext => WandStep + 1 > CardQueue.Count - 1 ? 0 : WandStep + 1;

    public bool MediationEmty => Mediation[0] > 0 && Mediation[1] > 0;

    public PlayerData(int Index, bool Base)
    {
        this.Index = Index;
        this.Base = Base;
    }
}

[Serializable]
public class CardData
{
    public string Named = "";
    public CardNameType Name;
    public CardOriginType Origin;
    public CardClassType Class;
    [Min(0)] public int RuneStoneCost;
    [Min(0)] public int ManaPoint;
    [Min(0)] public int ManaCurrent;
    [Min(0)] public int AttackPoint;
    [Min(0)] public int GrowCurrent;
    [Min(0)] public int RuneStoneTake; //When Wand move to card, get Rune Stone
    public Sprite Image;

    public IPlayer Player;
    public ICard Card;

    public int AttackCombine => AttackPoint + GrowCurrent;
}

[Serializable]
public class WildCardData
{
    public CardNameType Name;
    public Transform Card;
}