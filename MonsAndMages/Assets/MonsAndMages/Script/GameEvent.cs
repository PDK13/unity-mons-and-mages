using System;

public class GameEvent
{
    //Value "Update" is FALSE for UI invoke, then value is TRUE for Player and System invoke

    public static Action<IPlayer, bool> onPlayerStart;
    public static Action<IPlayer, int, bool> onPlayerTakeRuneStoneFromSupply;
    public static Action<IPlayer, bool> onPlayerTakeRuneStoneFromMediation;
    public static Action<IPlayer, bool> onPlayerCheckStunned;

    public static Action<IPlayer, bool> onPlayerDoChoice;
    public static Action<IPlayer, int, bool> onPlayerDoMediate; //Mediate Event
    public static Action<IPlayer, ICard, bool> onPlayerDoCollect; //Collect Event

    public static Action<IPlayer, ICard, bool> onCardOriginActive; //Origin Event

    public static Action<IPlayer, bool, bool> onPlayerDoWandNext;
    public static Action<IPlayer, bool> onPlayerDoWandActive;

    public static Action<IPlayer, ICard, bool> onCardAttack; //Attack Event
    public static Action<IPlayer, ICard, bool> onCardEnergyFill; //Energy Event
    public static Action<IPlayer, ICard, bool> onCardEnergyCheck;
    public static Action<IPlayer, ICard, bool> onCardEnergyActive;
    public static Action<IPlayer, ICard, bool> onCardClassActive; //Class Event
    public static Action<IPlayer, ICard, bool> onCardSpellActive; //Spell Event

    public static Action<IPlayer> onPlayerEnd;

    //

    public static void PlayerStart(IPlayer Player, bool Update)
    {
        onPlayerStart?.Invoke(Player, Update);
    }

    public static void PlayerTakeRuneStoneFromSupply(IPlayer Player, int Value, bool Update)
    {
        onPlayerTakeRuneStoneFromSupply?.Invoke(Player, Value, Update);
    }

    public static void PlayerTakeRuneStoneFromMediation(IPlayer Player, bool Update)
    {
        onPlayerTakeRuneStoneFromMediation?.Invoke(Player, Update);
    }

    public static void PlayerCheckStuned(IPlayer Player, bool Update)
    {
        onPlayerCheckStunned?.Invoke(Player, Update);
    }


    public static void PlayerDoChoice(IPlayer Player, bool Update)
    {
        onPlayerDoChoice?.Invoke(Player, Update);
    }

    public static void PlayerDoMediate(IPlayer Player, int RuneStoneAdd, bool Update)
    {
        onPlayerDoMediate?.Invoke(Player, RuneStoneAdd, Update);
    } //Mediate Event

    public static void PlayerDoCollect(IPlayer Player, ICard Card, bool Update)
    {
        onPlayerDoCollect?.Invoke(Player, Card, Update);
    } //Collect Event


    public static void CardOriginActive(IPlayer Player, ICard Card, bool Update)
    {
        onCardOriginActive?.Invoke(Player, Card, Update);
    } //Origin Event


    public static void PlayerDoWandNext(IPlayer Player, bool CardActive, bool Update)
    {
        onPlayerDoWandNext?.Invoke(Player, CardActive, Update);
    }

    public static void PlayerDoWandActive(IPlayer Player, bool Update)
    {
        onPlayerDoWandActive?.Invoke(Player, Update);
    }


    public static void CardAttack(IPlayer Player, ICard Card, bool Update)
    {
        onCardAttack?.Invoke(Player, Card, Update);
    } //Attack Event

    public static void CardEnergyFill(IPlayer Player, ICard Card, bool Update)
    {
        onCardEnergyFill?.Invoke(Player, Card, Update);
    } //Energy Event

    public static void CardEnergyCheck(IPlayer Player, ICard Card, bool Update)
    {
        onCardEnergyCheck?.Invoke(Player, Card, Update);
    }

    public static void CardEnergyActive(IPlayer Player, ICard Card, bool Update)
    {
        onCardEnergyActive?.Invoke(Player, Card, Update);
    }

    public static void CardClassActive(IPlayer Player, ICard Card, bool Update)
    {
        onCardClassActive?.Invoke(Player, Card, Update);
    } //Class Event

    public static void CardSpellActive(IPlayer Player, ICard Card, bool Update)
    {
        onCardSpellActive?.Invoke(Player, Card, Update);
    } //Spell Event


    public static void PlayerEnd(IPlayer Player)
    {
        onPlayerEnd?.Invoke(Player);
    }
}