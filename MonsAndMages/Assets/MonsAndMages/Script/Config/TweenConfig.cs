using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "tween-config", menuName = "Mons And Mages/Tween Config", order = 0)]
public class TweenConfig : ScriptableObject
{
    public TweenConfigCardFillData CardFill;
    public TweenConfigGameViewData GameView;
}

[Serializable]
public class TweenConfigCardFillData
{
    [Min(0)] public float MoveDuration = 1f;
    public Ease MoveEase = Ease.OutQuad;

    [Min(0)] public float OpenDuration = 0.5f;
}

[Serializable]
public class TweenConfigGameViewData
{
    [Min(0)] public float MoveYDuration = 1f;
    public Ease MoveYEase = Ease.OutQuad;

    [Min(0)] public float MoveXDuration = 1f;
    public Ease MoveXEase = Ease.OutQuad;
}