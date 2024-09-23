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

    public static Action<Action> onWildFill { get; set; }

    public static void WildCardFill(Action OnComplete)
    {
        onWildFill?.Invoke(OnComplete);
    }

    //View

    public static Action<ViewType, Action> onViewArea { get; set; }
    public static Action<ICard> onViewCard { get; set; }
    public static Action<IPlayer, Action> onViewPlayer { get; set; }

    public static void ViewArea(ViewType Type, Action OnComplete)
    {
        onViewArea?.Invoke(Type, OnComplete);
    }

    public static void ViewCard(ICard Card)
    {
        onViewCard?.Invoke(Card);
    }

    public static void ViewPlayer(IPlayer Player, Action OnComplete)
    {
        onViewPlayer?.Invoke(Player, OnComplete);
    }

    //Show

    public static Action<ViewType, bool> onShowUiArea { get; set; }
    public static Action<InfoType, bool> onShowUiInfo { get; set; }

    public static void ShowUiArea(ViewType Type, bool Show)
    {
        onShowUiArea?.Invoke(Type, Show);
    }

    public static void ShowUiInfo(InfoType Type, bool Show)
    {
        onShowUiInfo?.Invoke(Type, Show);
    }

    //Button

    public static Action<bool> onButtonInteractable { get; set; }
    public static Action<Button> onButtonPress { get; set; }

    public static void ButtonInteractable(bool Interactable)
    {
        onButtonInteractable?.Invoke(Interactable);
    }

    public static void ButtonPressed(Button Button)
    {
        onButtonPress?.Invoke(Button);
    }

    //Player

    public static Action<IPlayer, Action> onPlayerStart { get; set; } //PlayerQueue start turn
    public static Action<IPlayer, int, Action> onPlayerTakeRuneStoneFromSupply { get; set; } //PlayerQueue take rune stone from supply
    public static Action<IPlayer, int, Action> onPlayerTakeRuneStoneFromMediation { get; set; } //PlayerQueue take rune stone from mediation
    public static Action<IPlayer, Action> onPlayerStunnedCheck { get; set; } //PlayerQueue start turn
    public static Action<IPlayer, Action> onPlayerDoChoice { get; set; } //PlayerQueue choice PlayerDoMediate or PlayerDoCollect
    public static Action<IPlayer, int, Action> onPlayerDoMediate { get; set; } //PlayerQueue PlayerDoMediate Event
    public static Action<IPlayer, ICard, Action> onPlayerDoCollect { get; set; } //PlayerQueue PlayerDoCollect Event
    public static Action<IPlayer, Action> onPlayerCardManaActiveDoChoice { get; set; }
    public static Action<ICard, Action> onCardManaActive { get; set; }
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
    } //PlayerDoMediate Event

    public static void PlayerDoCollect(IPlayer Player, ICard Card, Action OnComplete)
    {
        onPlayerDoCollect?.Invoke(Player, Card, OnComplete);
    } //PlayerDoCollect Event

    public static void PlayerCardManaActiveDoChoice(IPlayer Player, Action OnComplete)
    {
        onPlayerCardManaActiveDoChoice?.Invoke(Player, OnComplete);
    }

    public static void CardActiveMana(ICard Card, Action OnComplete)
    {
        onCardManaActive?.Invoke(Card, OnComplete);
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


    public static Action<ICard, Action> onCardRumble { get; set; }
    public static Action<ICard, Action> onCardAttack { get; set; }

    public static void CardRumble(ICard Card, Action OnComplete)
    {
        onCardRumble?.Invoke(Card, OnComplete);
    }

    public static void CardAttack(ICard Card, Action OnComplete)
    {
        onCardAttack?.Invoke(Card, OnComplete);
    }

    //Origin

    public static Action<ICard, int, Action> onOriginDragon { get; set; } //Roll a Dice for Dragon
    public static Action<ICard> onOriginGhost { get; set; }

    public static void OriginDragon(ICard Card, int Dice, Action OnComplete)
    {
        onOriginDragon?.Invoke(Card, Dice, OnComplete);
    }

    public static void OriginGhost(ICard Card)
    {
        onOriginGhost?.Invoke(Card);
    }

    //Class

    public static Action<ICard, int, Action> onClassFighter { get; set; } //Roll a Dice for Fighter

    public static void ClassFighter(ICard Card, int Dice, Action OnComplete)
    {
        onClassFighter?.Invoke(Card, Dice, OnComplete);
    }
}