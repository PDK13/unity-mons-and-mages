using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "dice-config", menuName = "Mons And Mages/Dice Config", order = 0)]
public class DiceConfig : ScriptableObject
{
    public List<DiceConfigData> Data = new List<DiceConfigData>();
}

[Serializable]
public class DiceConfigData
{
    public Sprite Face;
    [Min(0)] public int Dragon = 0;
    [Min(0)] public int Bite = 0;
}