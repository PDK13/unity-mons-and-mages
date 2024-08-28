using System;
using UnityEngine;

public class GameEvent
{
    //CardEvent

    public static Action<int, CardNameType, Transform> onCardChoice;
    public static Action<int, CardData> onCardWandActive;

    public static void CardChoice(int PlayerIndex, CardNameType CardName, Transform Card)
    {
        onCardChoice?.Invoke(PlayerIndex, CardName, Card);
    }

    public static void CardWandActive(int PlayerIndex, CardData Card)
    {
        onCardWandActive?.Invoke(PlayerIndex, Card);
    }

    //PlayerEvent

    public static Action<int> onPlayerTurn;
    public static Action<int> onPlayerReady;
    public static Action<int> onPlayerWandNext;
    public static Action<int, int> onPlayerAttack;
    public static Action<int, int> onPlayerHeal;

    public static void PlayerTurn(int PlayerIndex)
    {
        onPlayerTurn?.Invoke(PlayerIndex);
    }

    public static void PlayerReady(int PlayerIndex)
    {
        onPlayerReady?.Invoke(PlayerIndex);
    }

    public static void PlayerWandNext(int PlayerIndex)
    {
        onPlayerWandNext?.Invoke(PlayerIndex);
    }

    public static void PlayerAttack(int PlayerIndex, int Value)
    {
        onPlayerAttack?.Invoke(PlayerIndex, Value);
    }

    public static void PlayerHeal(int PlayerIndex, int Value)
    {
        onPlayerHeal?.Invoke(PlayerIndex, Value);
    }
}