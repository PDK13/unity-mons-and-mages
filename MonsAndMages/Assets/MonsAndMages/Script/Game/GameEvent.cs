//Value "Update" is FALSE for UI invoke, then value is TRUE for Player and System invoke

using System;
using UnityEngine;
using UnityEngine.UI;

public class GameEvent
{
    //Init-Event

    public static Action onInit;
    public static Action<PlayerData[]> onInitPlayer;
    public static Action onWildFill;
    public static Action<Action> onGameStart;

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
        onWildFill?.Invoke();
    }

    public static void GameStart(Action OnComplete)
    {
        onGameStart?.Invoke(OnComplete);
    }

    //Ui-Event

    public static Action<IPlayer, Action> onViewPlayer;
    public static Action<Action> onViewWild;
    public static Action<Action> onViewField;

    public static Action<bool> onButtonInteractable;
    public static Action<Button> onButtonPress;

    public static void ViewPlayer(IPlayer Player, Action OnComplete)
    {
        onViewPlayer?.Invoke(Player, OnComplete);
    } //View Player field

    public static void ViewWild(Action OnComplete)
    {
        onViewWild?.Invoke(OnComplete);
    } //View Wild side

    public static void ViewField(Action OnComplete)
    {
        onViewField?.Invoke(OnComplete);
    } //View Player side

    public static void ButtonInteractable(bool Interactable)
    {
        onButtonInteractable?.Invoke(Interactable);
    }

    public static void ButtonPressed(Button Button)
    {
        onButtonPress?.Invoke(Button);
    }

    //Match-Event

    public static Action<IPlayer, Action> onPlayerTurn;
    public static Action<IPlayer, Action> onPlayerStart;
    public static Action<IPlayer, int, Action> onPlayerTakeRuneStoneFromSupply;
    public static Action<IPlayer, Action> onPlayerTakeRuneStoneFromMediation;
    public static Action<IPlayer, Action> onPlayerStunnedCheck;

    public static Action<IPlayer, Action> onPlayerDoChoice;
    public static Action<IPlayer, int, Action> onPlayerDoMediate; //Mediate Event

    public static Action<ICard, Action> onCardTap;
    public static Action<IPlayer, ICard, Action> onPlayerDoCollect; //Collect Event

    public static Action onCardFill;
    public static Action onCardFillComplete;

    public static Action<ICard, Action> onCardOriginActive; //Origin Event

    public static Action<IPlayer, bool, Action> onPlayerDoWandNext;
    public static Action<IPlayer, Action> onPlayerDoWandActive;

    public static Action<ICard, Action> onCardAttack; //Attack Event
    public static Action<ICard, Action> onCardEnergyFill; //Energy Event
    public static Action<ICard, Action> onCardEnergyCheck;
    public static Action<ICard, Action> onCardEnergyActive;
    public static Action<ICard, Action> onCardClassActive; //Class Event
    public static Action<ICard, Action> onCardSpellActive; //Spell Event

    public static Action<IPlayer, Action> onPlayerContinueCheck;
    public static Action<IPlayer, Action> onPlayerContinue;

    public static Action<IPlayer, Action> onPlayerEnd;

    public static void PlayerTurn(IPlayer Player, Action OnComplete)
    {
        onPlayerTurn?.Invoke(Player, OnComplete);
    }

    public static void PlayerStart(IPlayer Player, Action OnComplete)
    {
        onPlayerStart?.Invoke(Player, OnComplete);
    }

    public static void PlayerTakeRuneStoneFromSupply(IPlayer Player, int Value, Action OnComplete)
    {
        onPlayerTakeRuneStoneFromSupply?.Invoke(Player, Value, OnComplete);
    }

    public static void PlayerTakeRuneStoneFromMediation(IPlayer Player, Action OnComplete)
    {
        onPlayerTakeRuneStoneFromMediation?.Invoke(Player, OnComplete);
    }

    public static void PlayerStunnedCheck(IPlayer Player, Action OnComplete)
    {
        onPlayerStunnedCheck?.Invoke(Player, OnComplete);
    }


    public static void PlayerDoChoice(IPlayer Player, Action OnComplete)
    {
        onPlayerDoChoice?.Invoke(Player, OnComplete);
    } //Choice Event

    public static void PlayerDoMediate(IPlayer Player, int RuneStoneAdd, Action OnComplete)
    {
        onPlayerDoMediate?.Invoke(Player, RuneStoneAdd, OnComplete);
    } //Mediate Event


    public static void CardTap(ICard Card, Action OnComplete)
    {
        onCardTap?.Invoke(Card, OnComplete);
    }

    public static void PlayerDoCollect(IPlayer Player, ICard Card, Action OnComplete)
    {
        onPlayerDoCollect?.Invoke(Player, Card, OnComplete);
    } //Collect Event

    public static void CardFill()
    {
        onCardFill?.Invoke();
    }

    public static void CardFillComplete()
    {
        onCardFillComplete?.Invoke();
    }


    public static void CardOriginActive(ICard Card, Action OnComplete)
    {
        onCardOriginActive?.Invoke(Card, OnComplete);
    } //Origin Event


    public static void PlayerDoWandNext(IPlayer Player, bool CardActive, Action OnComplete)
    {
        onPlayerDoWandNext?.Invoke(Player, CardActive, OnComplete);
    }

    public static void PlayerDoWandActive(IPlayer Player, Action OnComplete)
    {
        onPlayerDoWandActive?.Invoke(Player, OnComplete);
    }


    public static void CardAttack(ICard Card, Action OnComplete)
    {
        onCardAttack?.Invoke(Card, OnComplete);
    } //Attack Event

    public static void CardEnergyFill(ICard Card, Action OnComplete)
    {
        onCardEnergyFill?.Invoke(Card, OnComplete);
    } //Energy Event

    public static void CardEnergyCheck(ICard Card, Action OnComplete)
    {
        onCardEnergyCheck?.Invoke(Card, OnComplete);
    }

    public static void CardEnergyActive(ICard Card, Action OnComplete)
    {
        onCardEnergyActive?.Invoke(Card, OnComplete);
    }

    public static void CardClassActive(ICard Card, Action OnComplete)
    {
        onCardClassActive?.Invoke(Card, OnComplete);
    } //Class Event

    public static void CardSpellActive(ICard Card, Action OnComplete)
    {
        onCardSpellActive?.Invoke(Card, OnComplete);
    } //Spell Event


    public static void PlayerContinueCheck(IPlayer Player, Action OnComplete)
    {
        onPlayerContinueCheck?.Invoke(Player, OnComplete);
    }

    public static void PlayerContinue(IPlayer Player, Action OnComplete)
    {
        onPlayerContinue?.Invoke(Player, OnComplete);
    } //Continue Event


    public static void PlayerEnd(IPlayer Player, Action OnComplete)
    {
        onPlayerEnd?.Invoke(Player, OnComplete);
    }
}