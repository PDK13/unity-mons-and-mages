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

    ICard CardActiveCurrent { get; }


    RectTransform PointerLast { get; }


    void Init(PlayerData Data);


    void DoStart(Action OnComplete); //Start turn


    void DoTakeRuneStoneFromSupply(int Value, Action OnComplete); //Take 1 rune stone from supply

    void DoTakeRuneStoneFromMediation(Action OnComplete); //Take rune stone from mediation

    void DoStunnedCheck(Action<bool> OnComplete); //Check stun stage


    void DoMediate(int RuneStoneAdd, Action OnComplete);


    void DoCollect(ICard Card, Action OnComplete);

    void DoBoardReRange(params ICard[] CardIgnore);


    void DoOriginDragon(ICard Card, Action OnComplete); //Origin Dragon Event

    void DoOriginWoodlandReady(ICard Card); //Origin Woodland Event

    void DoOriginWoodlandStart(ICard CardChoice);

    void DoOriginGhostReady(ICard Card); //Origin Ghost Event

    void DoOriginGhostStart(ICard CardChoice, Action OnComplete);

    void DoOriginInsect(ICard Card, Action OnComplete); //Origin Insect Event

    void DoOriginSiren(ICard Card, Action OnComplete); //Origin Siren Event

    void DoOriginNeutral(ICard Card, Action OnComplete); //Origin Neutral Event


    void DoStaffNext(Action OnComplete); //Move staff Next

    void DoStaffActive(Action OnComplete); //Active Card at staff after moved if not stunned

    void DoStaffRumble(Action OnComplete);


    void DoClassFighter(ICard Card, Action OnComplete);

    void DoClassMagicAddictReady(ICard Card, Action OnComplete);

    void DoClassMagicAddictStart(ICard CardChoice, Action OnComplete);

    void DoClassSinger(ICard Card, Action OnComplete);

    void DoClassCareTaker(ICard Card, Action OnComplete);

    void DoClassDiffuser(ICard Card, Action OnComplete);

    void DoClassFlyingReady(ICard Card);

    void DoClassFlyingStart(ICard CardChoice, Action OnComplete);


    void DoCardChoiceManaFillReady();

    void DoCardChoiceManaFillReady(CardOriginType Origin);

    void DoCardChoiceManaFillStart(ICard Card, int Value, Action OnComplete); //Fill Mana for another progess


    void CardManaCheckEnd();


    void DoEnd(Action OnComplete);


    void RuneStoneChange(int Value, Action OnComplete);

    void StunChange(int Value, Action OnComplete);

    void HealthChange(int Value, Action OnComplete);
}