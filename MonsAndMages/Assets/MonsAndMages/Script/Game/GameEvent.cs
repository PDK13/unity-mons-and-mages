using System;
using UnityEngine;
using UnityEngine.UI;

public class GameEvent
{
    //Init

    public static Action onInit { get; set; }
    public static Action<PlayerData[]> onInitPlayer { get; set; }

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

    public static Action<ViewType, Action> onView { get; set; }
    public static Action onViewUIHide { get; set; }
    public static Action<ViewType> onViewUIShow { get; set; }
    public static Action<IPlayer, Action> onViewPlayer { get; set; }
    public static Action<InfoType, bool> onViewInfo { get; set; }

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

    public static Action<IPlayer, Action> onPlayerStart { get; set; } //Player start turn
    public static Action<IPlayer, int, Action> onPlayerTakeRuneStoneFromSupply { get; set; } //Player take rune stone from supply
    public static Action<IPlayer, int, Action> onPlayerTakeRuneStoneFromMediation { get; set; } //Player take rune stone from mediation
    public static Action<IPlayer, Action> onPlayerStunnedCheck { get; set; } //Player start turn
    public static Action<IPlayer, Action> onPlayerDoChoice { get; set; } //Player choice Mediate or Collect
    public static Action<IPlayer, int, Action> onPlayerDoMediate { get; set; } //Player Mediate Event
    public static Action<IPlayer, ICard, Action> onPlayerDoCollect { get; set; } //Player Collect Event
    public static Action<IPlayer, Action> onPlayerCardEnergyActiveDoChoice { get; set; }
    public static Action<IPlayer, Action> onPlayerEnd { get; set; }
    public static Action<IPlayer, int, Action> onPlayerRuneStoneChange { get; set; }
    public static Action<IPlayer, int, Action> onPlayerHealthChange { get; set; }
    public static Action<IPlayer, int, Action> onPlayerStunnedChange { get; set; }

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

    public static void PlayerCardEnergyActiveDoChoice(IPlayer Player, Action OnComplete)
    {
        onPlayerCardEnergyActiveDoChoice?.Invoke(Player, OnComplete);
    }

    public static void PlayerEnd(IPlayer Player, Action OnComplete)
    {
        onPlayerEnd?.Invoke(Player, OnComplete);
    }

    public static void PlayerRuneStoneChange(IPlayer Player, int Value, Action OnComplete)
    {
        onPlayerRuneStoneChange?.Invoke(Player, Value, OnComplete);
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

    public static Action<ICard, Action> onCardTap { get; set; }
    public static Action<ICard, Action> onCardRumble { get; set; }
    public static Action<ICard, Action> onCardAttack { get; set; }

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