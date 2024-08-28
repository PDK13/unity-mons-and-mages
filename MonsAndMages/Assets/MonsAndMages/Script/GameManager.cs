using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //

    [SerializeField] private CardConfig m_cardConfig;

    private int m_playerCount;
    private List<PlayerData> m_playerData = new List<PlayerData>(); //Max 4 player, min 2 player in game
    private int m_playerTurn;

    private int m_healthPointStart = 5;
    private int m_runeStoneStart = 5;

    private List<WildCardData> m_wildCardData = new List<WildCardData>(); //Max 9 card in wild
    private List<CardNameType> m_baseCardData = new List<CardNameType>(); //Queue card left fill to wild

    //

    public CardConfig CardConfig => m_cardConfig;

    //

    private void Awake()
    {
        GameEvent.onPlayerTurn += OnPlayerTurn;
    }

    private void Start()
    {
        for (int i = 0; i < m_playerCount; i++)
            m_playerData.Add(new PlayerData(i, m_healthPointStart, m_runeStoneStart));

        GameEvent.PlayerTurn(m_playerTurn);
    }

    private void OnDestroy()
    {
        GameEvent.onPlayerTurn -= OnPlayerTurn;
    }

    //

    private void GameStart()
    {

    }

    //

    private void OnPlayerTurn(int PlayerIndex)
    {
        //Player start choice card from wild
    }

}