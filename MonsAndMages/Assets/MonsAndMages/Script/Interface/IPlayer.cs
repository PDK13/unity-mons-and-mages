using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    int Index { get; }

    bool Base { get; }

    int HealthPoint { get; }

    int RuneStone { get; }

    int StunPoint { get; }

    int StunCurrent { get; }

    bool Stuned { get; }

    List<ICard> CardQueue { get; }

    int WandStep { get; }

    int[] Mediation { get; }

    bool MediationEmty { get; }

    PlayerController Controller { get; }


    void Init(PlayerData Data);


    void DoStart(Action OnComplete); //Start turn


    void DoTakeRuneStoneFromSupply(int Value, Action OnComplete); //Take 1 rune stone from supply

    void DoTakeRuneStoneFromMediation(Action OnComplete); //Take rune stone from mediation

    void DoStunnedCheck(Action<bool> OnComplete); //Check stun stage


    void DoChoice(Action OnComplete);

    void DoMediate(int RuneStoneAdd, Action OnComplete);

    Transform DoCollectReady();

    void DoCollect(ICard Card, Action OnComplete);


    void DoWandNext(Action OnComplete); //Move Wand Next

    void DoWandActive(Action OnComplete); //Active Card at Wand after moved if not stunned


    bool DoContinueCheck();

    void DoContinue(Action OnComplete);

    void DoEnd(Action OnComplete);


    void StunChange(int Value);

    void HealthChange(int Value);
}