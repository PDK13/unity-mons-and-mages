using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "tutorial-config", menuName = "Mons And Mages/Tutorial Config", order = 0)]
public class TutorialConfig : ScriptableObject
{
    public List<TutorialConfigData> Data = new List<TutorialConfigData>();
}

[Serializable]
public class TutorialConfigData
{
    public TutorialStepType Step = TutorialStepType.None;
    public TutorialBoxType StepBox = TutorialBoxType.None;
    public TutorialButtonType StepButton = TutorialButtonType.None;
    public CardNameType StepCard = CardNameType.None;
    public TutorialHintType Hint = TutorialHintType.None;
}