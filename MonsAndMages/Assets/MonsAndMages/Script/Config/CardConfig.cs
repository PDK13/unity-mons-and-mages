using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "card-config", menuName = "Mons And Mages/Card Config", order = 0)]
public class CardConfig : ScriptableObject
{
    public List<CardData> Card = new List<CardData>();

    [Space]
    public List<CardConfigOriginData> Origin = new List<CardConfigOriginData>();

    [Space]
    public List<CardConfigClassData> Class = new List<CardConfigClassData>();

    public Sprite GetIconOrigin(CardOriginType Type)
    {
        if (Type == CardOriginType.None)
            return null;
        return Origin.Find(t => t.Type == Type).Icon;
    }

    public Sprite GetIconClass(CardClassType Type)
    {
        if (Type == CardClassType.None)
            return null;
        return Class.Find(t => t.Type == Type).Icon;
    }
}

[Serializable]
public class CardConfigOriginData
{
    public CardOriginType Type;
    public Sprite Icon;
}

[Serializable]
public class CardConfigClassData
{
    public CardClassType Type;
    public Sprite Icon;
}