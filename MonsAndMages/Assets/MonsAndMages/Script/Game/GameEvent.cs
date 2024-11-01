using System;
using UnityEngine.UI;

public class GameEvent
{
    //Start

    public static Action onStart { get; set; }

    public static void Start()
    {
        onStart?.Invoke();
    }

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

    //End

    public static Action onEndView { get; set; }
    public static Action onEnd { get; set; }

    public static void EndView()
    {
        onEndView?.Invoke();
    }

    public static void End()
    {
        onEnd?.Invoke();
    }

    //Wild

    public static Action<Action> onWildFill { get; set; }

    public static void WildCardFill(Action OnComplete)
    {
        onWildFill?.Invoke(OnComplete);
    }

    //View

    public static Action<ViewType, Action> onViewArea { get; set; }
    public static Action<IPlayer, Action> onViewPlayer { get; set; }

    public static void ViewArea(ViewType Type, Action OnComplete)
    {
        onViewArea?.Invoke(Type, OnComplete);
    }

    public static void ViewPlayer(IPlayer Player, Action OnComplete)
    {
        onViewPlayer?.Invoke(Player, OnComplete);
    }

    //Ui-Choice

    public static Action onUiChoiceHide { get; set; }
    public static Action onUiChoiceCurrent { get; set; }
    public static Action onUiChoiceMediateOrCollect { get; set; }
    public static Action onUiChoiceCardFullMana { get; set; }
    public static Action onUiChoiceCardOriginWoodland { get; set; }
    public static Action onUiChoiceCardOriginGhost { get; set; }
    public static Action onUiChoiceCardClassMagicAddict { get; set; }
    public static Action onUiChoiceCardClassFlying { get; set; }
    public static Action onUiChoiceCardSpell { get; set; }
    public static Action onUiChoiceCardEnter { get; set; }

    public static void UiChoiceHide()
    {
        onUiChoiceHide?.Invoke();
    }

    public static void UiChoiceCurrent()
    {
        onUiChoiceCurrent?.Invoke();
    }

    public static void UiChoiceMediateOrCollect()
    {
        onUiChoiceMediateOrCollect?.Invoke();
    }

    public static void UiChoiceCardFullMana()
    {
        onUiChoiceCardFullMana?.Invoke();
    }

    public static void UiChoiceCardOriginWoodland()
    {
        onUiChoiceCardOriginWoodland?.Invoke();
    }

    public static void UiChoiceCardOriginGhost()
    {
        onUiChoiceCardOriginGhost?.Invoke();
    }

    public static void UiChoiceCardClassMagicAddict()
    {
        onUiChoiceCardClassMagicAddict?.Invoke();
    }

    public static void UiChoiceCardClassFlying()
    {
        onUiChoiceCardClassFlying?.Invoke();
    }

    public static void UiChoiceCardSpell()
    {
        onUiChoiceCardSpell?.Invoke();
    }

    public static void UiChoiceCardEnter()
    {
        onUiChoiceCardEnter?.Invoke();
    }

    //Ui-Info

    public static Action<bool, bool> onUiInfoHide { get; set; }
    public static Action<ICard> onUiInfoCollect { get; set; }
    public static Action<ICard> onUiInfoZoom { get; set; }
    public static Action onUiInfoMediate { get; set; }
    public static Action<ICard> onUiInfoFullMana { get; set; }
    public static Action<ICard> onUiInfoOriginWoodland { get; set; }
    public static Action<ICard> onUiInfoOriginGhost { get; set; }
    public static Action<ICard> onUiInfoClassMagicAddict { get; set; }
    public static Action<ICard> onUiInfoClassFlying { get; set; }
    public static Action<ICard> onUiInfoCardSpell { get; set; }
    public static Action<ICard> onUiInfoCardEnter { get; set; }

    public static void UiInfoHide(bool MaskTween, bool CardBack)
    {
        onUiInfoHide?.Invoke(MaskTween, CardBack);
    }

    public static void UiInfoCollect(ICard Card)
    {
        onUiInfoCollect?.Invoke(Card);
    }

    public static void UiInfoZoom(ICard Card)
    {
        onUiInfoZoom?.Invoke(Card);
    }

    public static void UiInfoMediate()
    {
        onUiInfoMediate?.Invoke();
    }

    public static void UiInfoFullMana(ICard Card)
    {
        onUiInfoFullMana?.Invoke(Card);
    }

    public static void UiInfoOriginWoodland(ICard Card)
    {
        onUiInfoOriginGhost?.Invoke(Card);
    }

    public static void UiInfoOriginGhost(ICard Card)
    {
        onUiInfoOriginGhost?.Invoke(Card);
    }

    public static void UiInfoClassMagicAddict(ICard Card)
    {
        onUiInfoClassMagicAddict?.Invoke(Card);
    }

    public static void UiInfoClassFlying(ICard Card)
    {
        onUiInfoClassFlying?.Invoke(Card);
    }

    public static void UiInfoCardSpell(ICard Card)
    {
        onUiInfoCardSpell?.Invoke(Card);
    }

    public static void UiInfoCardEnter(ICard Card)
    {
        onUiInfoCardEnter?.Invoke(Card);
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
    public static Action<IPlayer, Action> onPlayerDoChoice { get; set; } //PlayerQueue choice PlayerDoMediateStart or PlayerDoCollectStart
    public static Action<IPlayer, int, Action> onPlayerDoMediate { get; set; } //PlayerQueue PlayerDoMediateStart Event
    public static Action<IPlayer, ICard, Action> onPlayerDoCollect { get; set; } //PlayerQueue PlayerDoCollectStart Event
    public static Action<IPlayer, Action> onPlayerCardManaActiveDoChoice { get; set; }
    public static Action<ICard, Action> onCardManaActive { get; set; }
    public static Action<IPlayer, Action> onPlayerEnd { get; set; }
    public static Action<IPlayer, Action> onPlayerRuneStoneUpdate { get; set; }
    public static Action<IPlayer, Action> onPlayerHealthUpdate { get; set; }
    public static Action<IPlayer, Action> onPlayerStunnedUpdate { get; set; }

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
    } //PlayerDoMediateStart Event

    public static void PlayerDoCollect(IPlayer Player, ICard Card, Action OnComplete)
    {
        onPlayerDoCollect?.Invoke(Player, Card, OnComplete);
    } //PlayerDoCollectStart Event

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

    public static void PlayerRuneStoneUpdate(IPlayer Player, Action OnComplete)
    {
        onPlayerRuneStoneUpdate?.Invoke(Player, OnComplete);
    }

    public static void PlayerHealthUpdate(IPlayer Player, Action OnComplete)
    {
        onPlayerHealthUpdate?.Invoke(Player, OnComplete);
    }

    public static void PlayerStunnedUpdate(IPlayer Player, Action OnComplete)
    {
        onPlayerStunnedUpdate?.Invoke(Player, OnComplete);
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

    public static Action<DiceConfigData[], Action> onOriginDragon { get; set; } //Roll a Dice for Dragon

    public static void OriginDragon(DiceConfigData[] DiceResult, Action OnComplete)
    {
        onOriginDragon?.Invoke(DiceResult, OnComplete);
    }

    //Class

    public static Action<DiceConfigData[], Action> onClassFighter { get; set; } //Roll a Dice for Fighter

    public static void ClassFighter(DiceConfigData[] DiceResult, Action OnComplete)
    {
        onClassFighter?.Invoke(DiceResult, OnComplete);
    }

    //Tutorial

    public static Action<CardNameType> onTutorialCard { get; set; }

    public static void TutorialCard(CardNameType CardName)
    {
        onTutorialCard?.Invoke(CardName);
    }
}