using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "tween-config", menuName = "Mons And Mages/Tween Config", order = 0)]
public class TweenConfig : ScriptableObject
{
    public TweenConfigCardFillData WildFill;
    public TweenConfigGameViewData GameView;
    public TweenConfigCardActionData CardAction;
}

[Serializable]
public class TweenConfigCardFillData
{
    [Min(0)] public float MoveDuration = 1f;
    public Ease MoveEase = Ease.OutQuad;
}

[Serializable]
public class TweenConfigGameViewData
{
    [Min(0)] public float MoveYDuration = 1f;
    public Ease MoveYEase = Ease.OutQuad;

    [Min(0)] public float MoveXDuration = 1f;
    public Ease MoveXEase = Ease.OutQuad;
}

[Serializable]
public class TweenConfigCardActionData
{
    [Min(0)] public float FlipDuration = 1f;
    [Min(0)] public float MoveDuration = 1f;
    [Min(0)] public float RumbleDuration = 0.5f;
    [Min(0)] public float AlphaDuration = 1f;
    [Min(0)] public float OutlineDuration = 1f;
}