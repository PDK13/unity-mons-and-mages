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


    void DoTakeRuneStoneFromSupply(int Value); //Take 1 rune stone from supply

    void DoTakeRuneStoneFromMediation(); //Take rune stone from mediation

    void DoStunnedCheck(); //Check stun stage


    void DoChoice();

    void DoMediate(int RuneStoneAdd);

    Transform DoCollectReady();

    void DoCollect(ICard Card);


    void DoWandNext(); //Move Wand Next

    void DoWandActive(); //Active Card at Wand after moved if not stunned


    void DoContinueCheck(IPlayer Player);

    void DoContinue(IPlayer Player);

    void DoEnd(IPlayer Player);


    void StunChange(int Value);

    void HealthChange(int Value);
}