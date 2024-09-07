using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "card-config", menuName = "Mons And Mages/Card Config", order = 0)]
public class CardConfig : ScriptableObject
{
    public Image TrickImage;
    public List<CardData> Card = new List<CardData>();
}