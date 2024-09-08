using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStage : MonoBehaviour, ICard, ICardStage
{
    //ICard

    public CardNameType Name => CardNameType.Stage;

    public CardOriginType Origin => CardOriginType.None;

    public CardClassType Class => CardClassType.None;

    public int RuneStoneCost => 0;

    public int Energy => 0;

    public int EnergyCurrent => 0;

    public bool EnergyFull => false;

    public int Attack => 0;

    public int Grow => 0;

    public int AttackCombine => 0;

    public IPlayer Player => null;

    public CardController Controller => this.GetComponent<CardController>();

    public void Init(CardData Data) { }

    public void DoCollectActive(IPlayer Player) { }

    public void DoOriginActive() { }

    public void DoEnterActive() { }

    public void DoPassiveActive() { }


    public void DoWandActive() { }

    public void DoAttackActive() { }

    public void DoEnergyFill(int Value) { }

    public void DoEnergyCheck() { }


    public void DoEnergyActive() { }

    public void DoClassActive() { }

    public void DoSpellActive() { }
}