using System;

public class GameEvent
{
    //Value "Update" is FALSE for UI invoke, then value is TRUE for Player and System invoke

    public static Action<IPlayer, bool> onPlayerStart;
    public static Action<IPlayer, int, bool> onPlayerTakeRuneStoneFromSupply;
    public static Action<IPlayer, bool> onPlayerTakeRuneStoneFromMediation;
    public static Action<IPlayer, bool> onPlayerCheckStunned;

    public static Action<IPlayer, bool> onPlayerDoChoice;
    public static Action<IPlayer, int, bool> onPlayerDoMediate;
    public static Action<IPlayer, ICard, bool> onPlayerDoCollect;

    public static Action<IPlayer, ICard, bool> onCardAbilityOriginActive; //Origin Event

    public static Action<IPlayer, bool, bool> onPlayerDoWandNext;
    public static Action<IPlayer> onPlayerDoWandActive;

    public static Action<IPlayer, ICard, bool> onCardAttack; //Attack Event
    public static Action<IPlayer, ICard, bool> onCardEnergyFill; //Energy Event
    public static Action<IPlayer, ICard, bool> onCardEnergyCheck;
    public static Action<IPlayer, ICard, bool> onCardEnergyActive;
    public static Action<IPlayer, ICard, bool> onCardAbilityClassActive; //Class Event
    public static Action<IPlayer, ICard, bool> onCardAbilitySpellActive; //Spell Event

    public static Action<IPlayer> onPlayerEnd;

    //

    public static void PlayerStart(IPlayer Player, bool Update)
    {
        if (Update)
            PlayerTakeRuneStoneFromSupply(Player, 1, false);
        onPlayerStart?.Invoke(Player, Update);
    }

    public static void PlayerTakeRuneStoneFromSupply(IPlayer Player, int Value, bool Update)
    {
        if (Update)
        {
            Player.TakeRuneStoneFromSupply(Value);
            PlayerTakeRuneStoneFromMediation(Player, false);
        }
        onPlayerTakeRuneStoneFromSupply?.Invoke(Player, Value, Update);
    }

    public static void PlayerTakeRuneStoneFromMediation(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.TakeRuneStoneFromMediation();
            PlayerCheckStuned(Player, false);
        }
        onPlayerTakeRuneStoneFromMediation?.Invoke(Player, Update);
    }

    public static void PlayerCheckStuned(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.CheckStunned();
            //
            if (Player.Stuned)
                PlayerDoWandNext(Player, false, false);
            else
                PlayerDoChoice(Player, false);
        }
        onPlayerCheckStunned?.Invoke(Player, Update);
    }


    public static void PlayerDoChoice(IPlayer Player, bool Update)
    {
        if (Update)
            Player.DoChoice();
        onPlayerDoChoice?.Invoke(Player, Update);
    }

    public static void PlayerDoMediate(IPlayer Player, int RuneStoneAdd, bool Update)
    {
        if (Update)
        {
            Player.DoMediate(RuneStoneAdd);
            PlayerDoWandNext(Player, true, false);
        }
        onPlayerDoMediate?.Invoke(Player, RuneStoneAdd, Update);
    } //Mediate Event

    public static void PlayerDoCollect(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCollect(Card);
            Card.DoCollectActive(Player);
            CardOriginActive(Player, Card, false);
        }
        onPlayerDoCollect?.Invoke(Player, Card, Update);
    } //Collect Event


    public static void CardOriginActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardOriginActive(Card);
            Card.OriginActive(Player);
            PlayerDoWandNext(Player, true, false);
        }
        onCardAbilityOriginActive?.Invoke(Player, Card, Update);
    } //Origin Event


    public static void PlayerDoWandNext(IPlayer Player, bool CardActive, bool Update)
    {
        if (Update)
        {
            Player.DoWandNext();
            if (CardActive)
                PlayerDoWandActive(Player, false);
            //else??
        }
        onPlayerDoWandNext?.Invoke(Player, CardActive, Update);
    }

    public static void PlayerDoWandActive(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.DoWandActive();
            Player.CardQueue[Player.WandStep].WandActive(Player);
            CardAttack(Player, Player.CardQueue[Player.WandStep], false);
        }
        onPlayerDoWandActive?.Invoke(Player);
    }


    public static void CardAttack(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardAttack();
            Card.AttackActive(Player);
            //All other Player take damage
            CardEnergyFill(Player, Card, false);
        }
        onCardAttack?.Invoke(Player, Card, Update);
    } //Attack Event

    public static void CardEnergyFill(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardEnergyFill(Card);
            Card.EnergyFill(Player, 1);
            CardEnergyCheck(Player, Card, false);
        }
        onCardEnergyFill?.Invoke(Player, Card, Update);
    } //Energy Event

    public static void CardEnergyCheck(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardEnergyCheck(Card);
            Card.EnergyCheck();
            if (Card.EnergyFull)
                CardEnergyActive(Player, Card, false);
            //else?
        }
        onCardEnergyCheck?.Invoke(Player, Card, Update);
    }

    public static void CardEnergyActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardEnergyActive(Card);
            Card.EnergyActive(Player);
            CardClassActive(Player, Card, false);
        }
        onCardEnergyActive?.Invoke(Player, Card, Update);
    }

    public static void CardClassActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardClassActive(Card);
            Card.ClassActive(Player);
            CardSpellActive(Player, Card, false);
        }
        onCardAbilityClassActive?.Invoke(Player, Card, Update);
    } //Class Event

    public static void CardSpellActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardSpellActive(Card);
            Card.SpellActive(Player);
            //Check other card??
        }
        onCardAbilitySpellActive?.Invoke(Player, Card, Update);
    } //Spell Event


    public static void PlayerEnd(IPlayer Player)
    {
        onPlayerEnd?.Invoke(Player);
    }
}