using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "card-config", menuName = "Mons And Mages/Card Config", order = 0)]
public class CardConfig : ScriptableObject
{
    public List<CardData> CardList = new List<CardData>();
}

[Serializable]
public class CardData
{
    public CardNameType Name;
    public CardOriginType Origin;
    public CardClassType Class;
    [Min(0)] public int Cost;
    [Min(0)] public int Energy;
    [Min(0)] public int Attack;
    public Sprite Image;
}