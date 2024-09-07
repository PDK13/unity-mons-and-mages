//Value "Update" is FALSE for UI invoke, then value is TRUE for Player and System invoke

using System;

public class GameEvent
{
    //Init-Event

    public static Action onInit;
    public static Action<PlayerData[]> onInitPlayer;
    public static Action onInitWild;
    public static Action onGameStart;

    public static void Init()
    {
        onInit?.Invoke();
    }

    public static void InitPlayer(PlayerData[] Player)
    {
        onInitPlayer?.Invoke(Player);
    }

    public static void InitWild()
    {
        onInitWild?.Invoke();
    }

    public static void GameStart()
    {
        onGameStart?.Invoke();
    }

    //Ui-Event

    public static Action<IPlayer, bool> onViewPlayer;
    public static Action<bool> onViewWild;
    public static Action<bool> onViewField;

    public static void ViewPlayer(IPlayer Player, bool Update = false)
    {
        onViewPlayer?.Invoke(Player, Update);
    } //View Player field

    public static void ViewWild(bool Update = false)
    {
        onViewWild?.Invoke(Update);
    } //View Wild side

    public static void ViewField(bool Update = false)
    {
        onViewField?.Invoke(Update);
    } //View Player side

    //Match-Event

    public static Action<IPlayer, bool> onPlayerTurn;
    public static Action<IPlayer, bool> onPlayerStart;
    public static Action<IPlayer, int, bool> onPlayerTakeRuneStoneFromSupply;
    public static Action<IPlayer, bool> onPlayerTakeRuneStoneFromMediation;
    public static Action<IPlayer, bool> onPlayerStunnedCheck;

    public static Action<IPlayer, bool> onPlayerDoChoice;
    public static Action<IPlayer, int, bool> onPlayerDoMediate; //Mediate Event

    public static Action<ICard, bool> onCardTap;
    public static Action<IPlayer, ICard, bool> onPlayerDoCollect; //Collect Event
    public static Action onCardFill;
    public static Action onCardFillComplete;

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

    public static void PlayerTurn(IPlayer Player, bool Update = false)
    {
        onPlayerTurn?.Invoke(Player, Update);
    }

    public static void PlayerStart(IPlayer Player, bool Update = false)
    {
        onPlayerStart?.Invoke(Player, Update);
    }

    public static void PlayerTakeRuneStoneFromSupply(IPlayer Player, int Value, bool Update = false)
    {
        onPlayerTakeRuneStoneFromSupply?.Invoke(Player, Value, Update);
    }

    public static void PlayerTakeRuneStoneFromMediation(IPlayer Player, bool Update = false)
    {
        onPlayerTakeRuneStoneFromMediation?.Invoke(Player, Update);
    }

    public static void PlayerStunnedCheck(IPlayer Player, bool Update = false)
    {
        onPlayerStunnedCheck?.Invoke(Player, Update);
    }


    public static void PlayerDoChoice(IPlayer Player, bool Update = false)
    {
        onPlayerDoChoice?.Invoke(Player, Update);
    } //Choice Event

    public static void PlayerDoMediate(IPlayer Player, int RuneStoneAdd, bool Update = false)
    {
        if (!Player.MediationEmty)
            return;
        onPlayerDoMediate?.Invoke(Player, RuneStoneAdd, Update);
    } //Mediate Event


    public static void CardTap(ICard Card, bool Update = false)
    {
        onCardTap?.Invoke(Card, Update);
    }

    public static void PlayerDoCollect(IPlayer Player, ICard Card, bool Update = false)
    {
        if (Player.RuneStone < Card.RuneStoneCost)
            return;
        onPlayerDoCollect?.Invoke(Player, Card, Update);
    } //Collect Event

    public static void CardFill()
    {
        onCardFill?.Invoke();
    }

    public static void CardFillComplete()
    {
        onCardFillComplete?.Invoke();
    }


    public static void CardOriginActive(ICard Card, bool Update = false)
    {
        onCardOriginActive?.Invoke(Card, Update);
    } //Origin Event


    public static void PlayerDoWandNext(IPlayer Player, bool CardActive, bool Update = false)
    {
        onPlayerDoWandNext?.Invoke(Player, CardActive, Update);
    }

    public static void PlayerDoWandActive(IPlayer Player, bool Update = false)
    {
        onPlayerDoWandActive?.Invoke(Player, Update);
    }


    public static void CardAttack(ICard Card, bool Update = false)
    {
        onCardAttack?.Invoke(Card, Update);
    } //Attack Event

    public static void CardEnergyFill(ICard Card, bool Update = false)
    {
        onCardEnergyFill?.Invoke(Card, Update);
    } //Energy Event

    public static void CardEnergyCheck(ICard Card, bool Update = false)
    {
        onCardEnergyCheck?.Invoke(Card, Update);
    }

    public static void CardEnergyActive(ICard Card, bool Update = false)
    {
        onCardEnergyActive?.Invoke(Card, Update);
    }

    public static void CardClassActive(ICard Card, bool Update = false)
    {
        onCardClassActive?.Invoke(Card, Update);
    } //Class Event

    public static void CardSpellActive(ICard Card, bool Update = false)
    {
        onCardSpellActive?.Invoke(Card, Update);
    } //Spell Event


    public static void PlayerContinueCheck(IPlayer Player, bool Update = false)
    {
        onPlayerContinueCheck?.Invoke(Player, Update);
    }

    public static void PlayerContinue(IPlayer Player, bool Update = false)
    {
        onPlayerContinue?.Invoke(Player, Update);
    } //Continue Event


    public static void PlayerEnd(IPlayer Player)
    {
        onPlayerEnd?.Invoke(Player);
    }
}