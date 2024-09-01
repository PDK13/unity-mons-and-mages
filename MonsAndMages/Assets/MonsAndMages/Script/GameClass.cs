using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int PlayerIndex;
    [Min(1)] public int HealthPoint;
    [Min(0)] public int RuneStone; //Use to get monsters from wild
    [Min(0)] public int StunPoint; //Max 3 to get 1 skip turn
    [Min(0)] public int StunCurrent;
    public bool Stuned;
    public List<ICard> CardQueue = new List<ICard>();
    [Min(0)] public int WandStep;
    public int[] Mediation = { 0, 0 };

    public int WandStepNext => WandStep + 1 > CardQueue.Count - 1 ? 0 : WandStep + 1;
}

[Serializable]
public class CardData
{
    public CardNameType Name;
    public CardOriginType Origin;
    public CardClassType Class;
    [Min(0)] public int RuneStoneCost;
    [Min(0)] public int EnergyPoint;
    [Min(0)] public int EnergyCurrent;
    [Min(0)] public int AttackPoint;
    [Min(0)] public int GrowCurrent;
    public Sprite Image;
}

[Serializable]
public class WildCardData
{
    public CardNameType Name;
    public Transform Card;
}