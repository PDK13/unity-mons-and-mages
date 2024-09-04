using System;

public class GameEvent
{
    //Value "Update" is FALSE for UI invoke, then value is TRUE for Player and System invoke

    public static Action<IPlayer, bool> onPlayerStart;
    public static Action<IPlayer, int, bool> onPlayerTakeRuneStoneFromSupply;
    public static Action<IPlayer, bool> onPlayerTakeRuneStoneFromMediation;
    public static Action<IPlayer, bool> onPlayerStunnedCheck;

    public static Action<IPlayer, bool> onPlayerDoChoice;
    public static Action<IPlayer, int, bool> onPlayerDoMediate; //Mediate Event
    public static Action<IPlayer, ICard, bool> onPlayerDoCollect; //Collect Event

    public static Action<ICard, bool> onCardOriginActive; //Origin Event

    public static Action<IPlayer, bool, bool> onPlayerDoWandNext;
    public static Action<IPlayer, bool> onPlayerDoWandActive;

    public static Action<ICard, bool> onCardAttack; //Attack Event
    public static Action<ICard, bool> onCardEnergyFill; //Energy Event
    public static Action<ICard, bool> onCardEnergyCheck;
    public static Action<ICard, bool> onCardEnergyActive;
    public static Action<ICard, bool> onCardClassActive; //Class Event
    public static Action<ICard, bool> onCardSpellActive; //Spell Event

    public static Action<IPlayer, bool> onPlayerContinueCheck;
    public static Action<IPlayer, bool> onPlayerContinue;

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

    public static void PlayerStunnedCheck(IPlayer Player, bool Update)
    {
        onPlayerStunnedCheck?.Invoke(Player, Update);
    }


    public static void PlayerDoChoice(IPlayer Player, bool Update)
    {
        onPlayerDoChoice?.Invoke(Player, Update);
    } //Choice Event

    public static void PlayerDoMediate(IPlayer Player, int RuneStoneAdd, bool Update)
    {
        if (!Player.MediationEmty)
            return;
        onPlayerDoMediate?.Invoke(Player, RuneStoneAdd, Update);
    } //Mediate Event

    public static void PlayerDoCollect(IPlayer Player, ICard Card, bool Update)
    {
        if (Player.RuneStone < Card.RuneStoneCost)
            return;
        onPlayerDoCollect?.Invoke(Player, Card, Update);
    } //Collect Event


    public static void CardOriginActive(ICard Card, bool Update)
    {
        onCardOriginActive?.Invoke(Card, Update);
    } //Origin Event


    public static void PlayerDoWandNext(IPlayer Player, bool CardActive, bool Update)
    {
        onPlayerDoWandNext?.Invoke(Player, CardActive, Update);
    }

    public static void PlayerDoWandActive(IPlayer Player, bool Update)
    {
        onPlayerDoWandActive?.Invoke(Player, Update);
    }


    public static void CardAttack(ICard Card, bool Update)
    {
        onCardAttack?.Invoke(Card, Update);
    } //Attack Event

    public static void CardEnergyFill(ICard Card, bool Update)
    {
        onCardEnergyFill?.Invoke(Card, Update);
    } //Energy Event

    public static void CardEnergyCheck(ICard Card, bool Update)
    {
        onCardEnergyCheck?.Invoke(Card, Update);
    }

    public static void CardEnergyActive(ICard Card, bool Update)
    {
        onCardEnergyActive?.Invoke(Card, Update);
    }

    public static void CardClassActive(ICard Card, bool Update)
    {
        onCardClassActive?.Invoke(Card, Update);
    } //Class Event

    public static void CardSpellActive(ICard Card, bool Update)
    {
        onCardSpellActive?.Invoke(Card, Update);
    } //Spell Event


    public static void PlayerContinueCheck(IPlayer Player, bool Update)
    {
        onPlayerContinueCheck?.Invoke(Player, Update);
    }

    public static void PlayerContinue(IPlayer Player, bool Update)
    {
        onPlayerContinue?.Invoke(Player, Update);
    } //Continue Event


    public static void PlayerEnd(IPlayer Player)
    {
        onPlayerEnd?.Invoke(Player);
    }
}