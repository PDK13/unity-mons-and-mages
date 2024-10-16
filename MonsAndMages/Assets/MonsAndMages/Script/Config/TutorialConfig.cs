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
    public TutorialStepType Step = TutorialStepType.None;
    public TutorialBoxType StepBox = TutorialBoxType.None;
    public TutorialButtonType StepButton = TutorialButtonType.None;
    public CardNameType StepCard = CardNameType.None;
    public TutorialHintType Hint = TutorialHintType.None;

    public bool Box => Step == TutorialStepType.Box;

    public bool Button => Step == TutorialStepType.Button;
    public bool ButtonMeidate => Step == TutorialStepType.Button && StepButton == TutorialButtonType.Mediate;
    public bool ButtonMeidateOption => Step == TutorialStepType.Button && StepButton == TutorialButtonType.MediateOption;
    public bool ButtonCollect => Step == TutorialStepType.Button && StepButton == TutorialButtonType.Collect;
    public bool ButtonAccept => Step == TutorialStepType.Button && StepButton == TutorialButtonType.Accept;
    public bool ButtonCancel => Step == TutorialStepType.Button && StepButton == TutorialButtonType.Cancel;
    public bool ButtonBack => Step == TutorialStepType.Button && StepButton == TutorialButtonType.Back;
    public bool ButtonPlayer => Step == TutorialStepType.Button && StepButton == TutorialButtonType.Player;

    public bool Card => Step == TutorialStepType.Card;
}