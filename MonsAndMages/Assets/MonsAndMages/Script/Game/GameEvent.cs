using System;
using UnityEngine;
using UnityEngine.UI;

public class GameEvent
{
    //Init

    public static Action onInit;
    public static Action<PlayerData[]> onInitPlayer;

    public static void Init()
    {
        onInit?.Invoke();
    }

    public static void InitPlayer(PlayerData[] Player)
    {
        onInitPlayer?.Invoke(Player);
    }

    //Wild

    public static Action<Action> onWildFill;

    public static void WildCardFill(Action OnComplete)
    {
        onWildFill?.Invoke(OnComplete);
    }

    //View

    public static Action<ViewType, Action> onView;
    public static Action onViewUIHide;
    public static Action<ViewType> onViewUIShow;
    public static Action<IPlayer, Action> onViewPlayer;
    public static Action<InfoType, bool> onViewInfo;

    public static void View(ViewType Type, Action OnComplete)
    {
        onView?.Invoke(Type, OnComplete);
    }

    public static void ViewUiHide()
    {
        onViewUIHide?.Invoke();
    }

    public static void ViewUiShow(ViewType Type)
    {
        onViewUIShow?.Invoke(Type);
    }

    public static void ViewPlayer(IPlayer Player, Action OnComplete)
    {
        onViewPlayer?.Invoke(Player, OnComplete);
    } //View Player field

    public static void ViewInfo(InfoType Type, bool Show)
    {
        onViewInfo?.Invoke(Type, Show);
    }

    //Button

    public static Action<bool> onButtonInteractable;
    public static Action<Button> onButtonPress;

    public static void ButtonInteractable(bool Interactable)
    {
        onButtonInteractable?.Invoke(Interactable);
    }

    public static void ButtonPressed(Button Button)
    {
        onButtonPress?.Invoke(Button);
    }

    //Player

    public static Action<IPlayer, Action> onPlayerStart; //Player start turn
    public static Action<IPlayer, int, Action> onPlayerTakeRuneStoneFromSupply; //Player take rune stone from supply
    public static Action<IPlayer, int, Action> onPlayerTakeRuneStoneFromMediation; //Player take rune stone from mediation
    public static Action<IPlayer, Action> onPlayerStunnedCheck; //Player start turn
    public static Action<IPlayer, Action> onPlayerDoChoice; //Player choice Mediate or Collect
    public static Action<IPlayer, int, Action> onPlayerDoMediate; //Player Mediate Event
    public static Action<IPlayer, ICard, Action> onPlayerDoCollect; //Player Collect Event
    public static Action<IPlayer, Action> onPlayerEnd;
    public static Action<IPlayer, int, Action> onPlayerHealthChange;
    public static Action<IPlayer, int, Action> onPlayerStunnedChange;

    public static void PlayerStart(IPlayer Player, Action OnComplete)
    {
        onPlayerStart?.Invoke(Player, OnComplete);
    }

    public static void PlayerTakeRuneStoneFromSupply(IPlayer Player, int Value, Action OnComplete)
    {
        onPlayerTakeRuneStoneFromSupply?.Invoke(Player, Value, OnComplete);
    }

    public static void PlayerTakeRuneStoneFromMediation(IPlayer Player, int Value, Action OnComplete)
    {
        onPlayerTakeRuneStoneFromMediation?.Invoke(Player, Value, OnComplete);
    }

    public static void PlayerStunnedCheck(IPlayer Player, Action OnComplete)
    {
        onPlayerStunnedCheck?.Invoke(Player, OnComplete);
    }

    public static void PlayerDoChoice(IPlayer Player, Action OnComplete)
    {
        onPlayerDoChoice?.Invoke(Player, OnComplete);
    }

    public static void PlayerDoMediate(IPlayer Player, int RuneStoneAdd, Action OnComplete)
    {
        onPlayerDoMediate?.Invoke(Player, RuneStoneAdd, OnComplete);
    } //Mediate Event

    public static void PlayerDoCollect(IPlayer Player, ICard Card, Action OnComplete)
    {
        onPlayerDoCollect?.Invoke(Player, Card, OnComplete);
    } //Collect Event

    public static void PlayerEnd(IPlayer Player, Action OnComplete)
    {
        onPlayerEnd?.Invoke(Player, OnComplete);
    }

    public static void PlayerHealthChange(IPlayer Player, int Value, Action OnComplete)
    {
        onPlayerHealthChange?.Invoke(Player, Value, OnComplete);
    }

    public static void PlayerStunnedChange(IPlayer Player, int Value, Action OnComplete)
    {
        onPlayerStunnedChange?.Invoke(Player, Value, OnComplete);
    }

    //Card

    public static Action<ICard, Action> onCardTap;
    public static Action<ICard, Action> onCardRumble;
    public static Action<ICard, Action> onCardAttack;

    public static void CardTap(ICard Card, Action OnComplete)
    {
        onCardTap?.Invoke(Card, OnComplete);
    }

    public static void CardRumble(ICard Card, Action OnComplete)
    {
        onCardRumble?.Invoke(Card, OnComplete);
    }

    public static void CardAttack(ICard Card, Action OnComplete)
    {
        onCardAttack?.Invoke(Card, OnComplete);
    }
}