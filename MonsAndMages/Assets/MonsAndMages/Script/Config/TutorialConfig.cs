using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "tutorial-config", menuName = "Mons And Mages/Tutorial Config", order = 0)]
public class TutorialConfig : ScriptableObject
{
    public List<CardNameType> Wild = new List<CardNameType>();
    public List<TutorialConfigData> Step = new List<TutorialConfigData>();
}

[Serializable]
public class TutorialConfigData
{
    public bool Auto = false;

    [BoxGroup("Data")]
    public TutorialStepType Type = TutorialStepType.None;

    [ShowIfGroup("Data/Box", Value = TutorialStepType.Box)]
    public TutorialBoxType BoxValue = TutorialBoxType.None;

    [ShowIfGroup("Data/Button", Value = TutorialStepType.Button)]
    public TutorialButtonType ButtonValue = TutorialButtonType.None;

    [ShowIfGroup("Data/Card", Value = TutorialStepType.Card)]
    public CardNameType CardValue = CardNameType.None;

    //public TutorialHintType Hint = TutorialHintType.None;

    public bool Box => Type == TutorialStepType.Box;

    public bool Button => Type == TutorialStepType.Button;
    public bool ButtonMeidate => Type == TutorialStepType.Button && ButtonValue == TutorialButtonType.Mediate;
    public bool ButtonMeidateOption => Type == TutorialStepType.Button && ButtonValue == TutorialButtonType.MediateOption;
    public bool ButtonCollect => Type == TutorialStepType.Button && ButtonValue == TutorialButtonType.Collect;
    public bool ButtonAccept => Type == TutorialStepType.Button && ButtonValue == TutorialButtonType.Accept;
    public bool ButtonCancel => Type == TutorialStepType.Button && ButtonValue == TutorialButtonType.Cancel;
    public bool ButtonBack => Type == TutorialStepType.Button && ButtonValue == TutorialButtonType.Back;
    public bool ButtonPlayer => Type == TutorialStepType.Button && ButtonValue == TutorialButtonType.Player;

    public bool Card => Type == TutorialStepType.Card;
}