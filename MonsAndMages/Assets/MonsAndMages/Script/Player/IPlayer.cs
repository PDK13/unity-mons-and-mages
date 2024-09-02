using System.Collections.Generic;

public interface IPlayer
{
    int PlayerIndex { get; }

    int HealthPoint { get; }

    int RuneStone { get; }

    int StunPoint { get; }

    int StunCurrent { get; }

    bool Stuned { get; }

    List<ICard> CardQueue { get; }

    int WandStep { get; }

    int[] Mediation { get; }

    //

    void TakeRuneStoneFromSupply(int Value); //Take 1 rune stone from supply

    void TakeRuneStoneFromMediation(); //Take rune stone from mediation

    void CheckStunned(); //Check stun stage

    //

    void DoChoice();

    void DoMediate(int RuneStoneAdd);

    void DoCollect(ICard Card);

    void DoCardOriginActive(ICard Card);

    //

    void DoWandNext(); //Move Wand Next

    void DoWandActive(); //Active Card at Wand after moved if not stunned

    void DoCardAttack(); //Attack other Players with Card got Wand on lasted moved

    void DoCardEnergyFill(ICard Card); //Fill energy for Card got Wand on lasted attack

    void DoCardEnergyCheck(ICard Card); //Check energy from Card got Wand on lasted filled energy

    void DoCardEnergyActive(ICard Card); //Active energy from Card got Wand on lasted filled energy

    void DoCardClassActive(ICard Card); //Active class ability from Card got Wand on lasted filled energy

    void DoCardSpellActive(ICard Card); //Active spell ability from Card got Wand on lasted filled energy

    //

    void TakeStun(int Value = 1);

    void TakeDamage(int Value);

    void DoHeal(int Value);
}