using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //

    [SerializeField] private CardConfig m_cardConfig;

    private int m_playerCount;
    private List<IPlayer> m_player = new List<IPlayer>(); //Max 4 player, min 2 player in game
    private int m_playerTurn = 0;

    private int m_healthPointStart = 5;
    private int m_runeStoneStart = 5;

    private List<WildCardData> m_wildCardData = new List<WildCardData>(); //Max 9 card in wild
    private List<CardNameType> m_baseCardData = new List<CardNameType>(); //Queue card left fill to wild

    //

    public CardConfig CardConfig => m_cardConfig;

    public IPlayer PlayerCurrent => m_player[m_playerTurn];

    //

    private void OnEnable()
    {
        GameEvent.onPlayerEnd += OnPlayerEnd;
    }

    private void OnDisable()
    {
        GameEvent.onPlayerEnd -= OnPlayerEnd;
    }

    private void Start()
    {
        GameEvent.PlayerStart(PlayerCurrent, false);
    }

    //

    private void OnPlayerEnd(IPlayer Player)
    {
        m_playerTurn++;
        if (m_playerTurn > m_player.Count - 1)
            m_playerTurn = 0;
        GameEvent.PlayerStart(PlayerCurrent, false);
    }
}