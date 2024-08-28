using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "card-config", menuName = "Mons And Mages/Card Config", order = 0)]
public class CardConfig : ScriptableObject
{
    public List<CardData> CardList = new List<CardData>();
}