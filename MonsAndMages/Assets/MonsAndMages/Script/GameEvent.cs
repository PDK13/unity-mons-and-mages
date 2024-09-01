using System;

public class GameEvent
{
    //Value "Update" is FALSE for UI invoke, then value is TRUE for Player and System invoke

    public static Action<IPlayer, bool> onPlayerStart;
    public static Action<IPlayer, int, bool> onPlayerTakeRuneStoneFromSupply;
    public static Action<IPlayer, bool> onPlayerTakeRuneStoneFromMediation;
    public static Action<IPlayer, bool> onPlayerCheckStunned;

    public static Action<IPlayer> onPlayerDoMediate;
    public static Action<IPlayer> onPlayerDoCollect;

    public static Action<IPlayer, ICard, bool> onCardAbilityOriginActive; //Origin Ability Event

    public static Action<IPlayer, bool, bool> onPlayerDoWandNext;
    public static Action<IPlayer> onPlayerDoWandActive;

    public static Action<IPlayer, ICard, bool> onCardAttack; //Attack Event
    public static Action<IPlayer, ICard, bool> onCardEnergyFill;
    public static Action<IPlayer, ICard, bool> onCardEnergyCheck;
    public static Action<IPlayer, ICard, bool> onCardEnergyActive;
    public static Action<IPlayer, ICard, bool> onCardAbilityClassActive; //Class Ability Event
    public static Action<IPlayer, ICard, bool> onCardAbilitySpellActive; //Spell Ability Event

    public static Action<IPlayer> onPlayerEnd;

    //

    public static void PlayerStart(IPlayer Player, bool Update)
    {
        if (Update)
            Player.TakeRuneStoneFromSupply(1);
        onPlayerStart?.Invoke(Player, Update);
    }

    public static void PlayerTakeRuneStoneFromSupply(IPlayer Player, int Value, bool Update)
    {
        if (Update)
            Player.TakeRuneStoneFromSupply(1);
        onPlayerTakeRuneStoneFromSupply?.Invoke(Player, Value, Update);
    }

    public static void PlayerTakeRuneStoneFromMediation(IPlayer Player, bool Update)
    {
        if (Update)
            Player.TakeRuneStoneFromMediation();
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
                PlayerEnd(Player);
        }
        onPlayerCheckStunned?.Invoke(Player, Update);
    }


    public static void PlayerDoMediate(IPlayer Player)
    {
        onPlayerDoMediate?.Invoke(Player);
    }

    public static void PlayerDoCollect(IPlayer Player)
    {
        onPlayerDoCollect?.Invoke(Player);
    }


    public static void CardAbilityOriginActive(IPlayer Player, ICard Type, bool Update)
    {
        if (Update)
        {
            Player.DoCardAbilityOriginActive();
            Player.CardQueue[Player.WandStep].AbilityOriginActive(Player);
        }
        onCardAbilityOriginActive?.Invoke(Player, Type, Update);
    } //Origin Ability Event


    public static void PlayerDoWandNext(IPlayer Player, bool CardActive, bool Update)
    {
        if (Update)
        {
            Player.DoWandNext();
            if (CardActive)
                PlayerDoWandActive(Player, false);
        }
        onPlayerDoWandNext?.Invoke(Player, CardActive, Update);
    }

    public static void PlayerDoWandActive(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.DoWandActive();
            Player.CardQueue[Player.WandStep].DoWandActive(Player);
        }
        onPlayerDoWandActive?.Invoke(Player);
    }


    public static void CardAttack(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardAttack();
            //All other Player take damage
        }
        onCardAttack?.Invoke(Player, Card, Update);
    } //Attack Event

    public static void CardEnergyFill(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardEnergyFill();
            Player.CardQueue[Player.WandStep].DoEnergyFill(Player, 1);
        }
        onCardEnergyFill?.Invoke(Player, Card, Update);
    }

    public static void CardEnergyCheck(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardEnergyCheck();
            if (Player.CardQueue[Player.WandStep].EnergyFull)
                CardEnergyActive(Player, Card, false);
        }
        onCardEnergyCheck?.Invoke(Player, Card, Update);
    }

    public static void CardEnergyActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardEnergyActive();
            Player.CardQueue[Player.WandStep].DoEnergyActive(Player);
        }
        onCardEnergyActive?.Invoke(Player, Card, Update);
    }

    public static void CardAbilityClassActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardAbilityClassActive();
            Player.CardQueue[Player.WandStep].AbilityClassActive(Player);
        }
        onCardAbilityClassActive?.Invoke(Player, Card, Update);
    } //Class Ability Event

    public static void CardSpellClassActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardAbilitySpellActive();
            Player.CardQueue[Player.WandStep].AbiltySpellActive(Player);
        }
        onCardAbilitySpellActive?.Invoke(Player, Card, Update);
    } //Spell Ability Event


    public static void PlayerEnd(IPlayer Player)
    {
        onPlayerEnd?.Invoke(Player);
    }
}