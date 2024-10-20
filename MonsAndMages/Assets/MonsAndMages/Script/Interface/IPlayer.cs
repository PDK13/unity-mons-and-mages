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


    RectTransform PointerLast { get; }

    int ProgessMana { get; set; }

    ICard ProgessCardChoice { get; set; }


    void Init(PlayerData Data);


    void DoStart(); //Start turn


    void DoMediate(int RuneStoneAdd, Action OnComplete);


    void DoCollect(ICard Card, Action OnComplete);

    void DoBoardReRange(params ICard[] CardIgnore);


    void DoStaffNext(bool Active); //Move staff Next

    void DoStaffNext(Action OnComplete); //Move staff Next

    void DoStaffActive(Action OnComplete); //Active Card at staff after moved if not stunned

    void DoStaffTakeRuneStone(int Value, Action OnComplete);

    void DoStaffRumble(Action OnComplete);


    void DoClassFlyingProgess(int IndexFrom, int IndexTo, Action OnComplete);


    void ProgessCard(ICard Card);

    void ProgessCardDone(ICard Card);

    void ProgessCheck();


    void DoEnd(Action OnComplete);


    void RuneStoneChange(int Value, Action OnComplete);

    void StunChange(int Value, Action OnComplete);

    void HealthChange(int Value, Action OnComplete);
}