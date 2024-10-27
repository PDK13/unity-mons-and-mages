using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "explain-config", menuName = "Mons And Mages/Explain Config", order = 0)]
public class ExplainConfig : ScriptableObject
{
    public List<ExplainConfigOriginData> CardOrigin = new List<ExplainConfigOriginData>();

    [Space]
    public List<ExplainConfigClassData> CardClass = new List<ExplainConfigClassData>();

    public Sprite GetExplainOrigin(CardOriginType Type)
    {
        if (Type == CardOriginType.None)
            return null;
        return CardOrigin.Find(t => t.Type == Type).Text;
    }

    public Sprite GetExplainClass(CardClassType Type)
    {
        if (Type == CardClassType.None)
            return null;
        return CardClass.Find(t => t.Type == Type).Text;
    }
}

[Serializable]
public class ExplainConfigOriginData
{
    public CardOriginType Type;
    public Sprite Text;
}

[Serializable]
public class ExplainConfigClassData
{
    public CardClassType Type;
    public Sprite Text;
}