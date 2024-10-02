using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    int Index { get; }

    bool Base { get; }

    int HealthPoint { get; }

    int HealthCurrent { get; }

    int RuneStone { get; }

    int StunPoint { get; }

    int StunCurrent { get; }

    bool Stuned { get; }

    int[] Mediation { get; }

    bool MediationEmty { get; }

    ICard[] CardQueue { get; }

    int StaffStep { get; }

    ICard CardStaffCurrent { get; }

    ICard CardManaActiveCurrent { get; }


    void Init(PlayerData Data);


    void DoStart(Action OnComplete); //Start turn


    void DoTakeRuneStoneFromSupply(int Value, Action OnComplete); //Take 1 rune stone from supply

    void DoTakeRuneStoneFromMediation(Action OnComplete); //Take rune stone from mediation

    void DoStunnedCheck(Action<bool> OnComplete); //Check stun stage


    void DoMediate(int RuneStoneAdd, Action OnComplete);


    public (RectTransform Pointer, RectTransform Centre) DoCollectReady();

    void DoCollect(ICard Card, Action OnComplete);

    void DoBoardReRange();


    void DoStaffNext(Action OnComplete); //Move staff Next

    void DoStaffActive(Action OnComplete); //Active Card at staff after moved if not stunned

    void DoStaffRumble(Action OnComplete);


    void DoEnd(Action OnComplete);


    void RuneStoneChange(int Value, Action OnComplete);

    void StunChange(int Value, Action OnComplete);

    void HealthChange(int Value, Action OnComplete);


    void DoCardSpecialActiveCurrent(ICard Card);


    void DoCardSwap(int IndexStart, int IndexTo, Action OnComplete);
}