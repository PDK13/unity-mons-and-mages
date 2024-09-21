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

    public string GetExplainOrigin(CardOriginType Type)
    {
        if (Type == CardOriginType.None)
            return "";
        return "Origin: " + Type.ToString() + "\n" + CardOrigin.Find(t => t.Type == Type).Text;
    }

    public string GetExplainClass(CardClassType Type)
    {
        if (Type == CardClassType.None)
            return "";
        return "Class: " + Type.ToString() + "\n" + CardClass.Find(t => t.Type == Type).Text;
    }
}

[Serializable]
public class ExplainConfigOriginData
{
    public CardOriginType Type;
    [Multiline(4)] public string Text;
}

[Serializable]
public class ExplainConfigClassData
{
    public CardClassType Type;
    [Multiline(4)] public string Text;
}