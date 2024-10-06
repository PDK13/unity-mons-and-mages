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


    void Init(PlayerData Data);


    void DoStart(); //Start turn


    void DoMediate(int RuneStoneAdd, Action OnComplete);


    void DoCollect(ICard Card, Action OnComplete);

    void DoBoardReRange(params ICard[] CardIgnore);


    void DoOriginDragon(ICard Card, Action OnComplete); //Origin Dragon Event

    void DoOriginWoodlandReady(ICard Card); //Origin Woodland Event

    void DoOriginWoodlandStart(ICard CardChoice);

    void DoOriginGhostReady(ICard Card); //Origin Ghost Event

    void DoOriginGhostStart(ICard CardChoice);

    void DoOriginInsect(ICard Card, Action OnComplete); //Origin Insect Event

    void DoOriginSiren(ICard Card, Action OnComplete); //Origin Siren Event

    void DoOriginNeutral(ICard Card, Action OnComplete); //Origin Neutral Event


    void DoStaffNext(bool Active); //Move staff Next

    void DoStaffNext(Action OnComplete); //Move staff Next

    void DoStaffActive(Action OnComplete); //Active Card at staff after moved if not stunned

    void DoStaffRumble(Action OnComplete);


    void DoClassFighter(ICard Card, Action OnComplete); //Class Fighter Event

    void DoClassMagicAddictReady(ICard Card, Action OnComplete); //Class Magic Addict Event

    void DoClassMagicAddictStart(ICard CardChoice);

    void DoClassSinger(ICard Card, Action OnComplete); //Class Singer Event

    void DoClassCareTaker(ICard Card, Action OnComplete); //Class Care Taker Event

    void DoClassDiffuser(ICard Card, Action OnComplete); //Class Diffuser Event

    void DoClassFlyingReady(ICard Card); //Class Flying Event

    void DoClassFlyingStart(ICard CardChoice);


    void DoCardManaFillReady(ICard Card, int Mana);

    void DoCardManaFillReady(ICard Card, CardOriginType Origin, int Mana);

    void DoCardManaFillStart(ICard CardChoice); //Fill Mana for another progess


    void CardManaCheckEnd();


    void DoEnd(Action OnComplete);


    void RuneStoneChange(int Value, Action OnComplete);

    void StunChange(int Value, Action OnComplete);

    void HealthChange(int Value, Action OnComplete);
}