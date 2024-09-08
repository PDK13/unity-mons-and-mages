using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //

    [SerializeField][Min(0)] private int m_PlayerStart = 0;
    [SerializeField][Min(0)] private int m_playerIndex = 0;
    [SerializeField] private bool m_sameDevice = true;

    [Space]
    [SerializeField] private CardConfig m_cardConfig;
    [SerializeField] private Transform m_playerContent;

    private bool m_gameStart = false;

    private int m_playerCount;
    private List<IPlayer> m_player = new List<IPlayer>(); //Max 4 player, min 2 player in game
    private int m_playerTurn = 0;
    private bool m_playerView = false;

    private int m_healthPointStart = 5;
    private int m_runeStoneStart = 5;

    private List<WildCardData> m_wildCardData = new List<WildCardData>(); //Max 9 card in wild
    private List<CardNameType> m_baseCardData = new List<CardNameType>(); //Queue card left fill to wild

    //

    public CardConfig CardConfig => m_cardConfig;

    public IPlayer PlayerCurrent => m_player[m_playerTurn];

    public bool PlayerView => m_gameStart ? m_sameDevice || m_player[m_playerTurn].Base : false;

    private void Awake()
    {
        GameManager.instance = this;
    }

    private IEnumerator Start()
    {
        m_playerTurn = m_PlayerStart;

        GameEvent.Init();

        PlayerData[] PlayerJoin = new PlayerData[2]
        {
            new PlayerData(0, m_playerIndex == 0),
            new PlayerData(1, m_playerIndex == 1),
        };
        GameEvent.InitPlayer(PlayerJoin);

        yield return new WaitForSeconds(3f);

        GameEvent.ViewWild(null);

        yield return new WaitForSeconds(2f);

        GameEvent.InitWild();

        yield return new WaitForSeconds(7f);

        GameEvent.ViewField(null);

        yield return new WaitForSeconds(2f);

        m_gameStart = true;

        GameEvent.GameStart(() => OnGameStart());
    }

    //

    public void PlayerJoin(IPlayer Player)
    {
        if (m_player.Exists(t => t == Player))
            return;
        m_player.Add(Player);
    }

    public IPlayer GetPlayer(int PlayerIndex)
    {
        return m_player[PlayerIndex];
    }

    //

    private void OnGameStart()
    {
        GameEvent.PlayerTurn(PlayerCurrent, () => OnPlayerTurn(PlayerCurrent));
    }


    private void OnPlayerTurn(IPlayer Player)
    {
        GameEvent.ViewField(() => GameEvent.ViewPlayer(PlayerCurrent, () => OnPlayerStart(PlayerCurrent)));
    }

    private void OnPlayerStart(IPlayer Player)
    {
        GameEvent.PlayerTakeRuneStoneFromSupply(Player, 1, () => OnPlayerTakeRuneStoneFromSupply(Player, 1));
    }

    private void OnPlayerTakeRuneStoneFromSupply(IPlayer Player, int Value)
    {
        Player.DoTakeRuneStoneFromSupply(Value);
        GameEvent.PlayerTakeRuneStoneFromMediation(Player, () => OnPlayerTakeRuneStoneFromMediation(Player));
    }

    private void OnPlayerTakeRuneStoneFromMediation(IPlayer Player)
    {
        Player.DoTakeRuneStoneFromMediation();
        GameEvent.PlayerStunnedCheck(Player, () => OnPlayerStunnedCheck(Player));
    }

    private void OnPlayerStunnedCheck(IPlayer Player)
    {
        Player.DoStunnedCheck();
        //
        if (Player.Stuned)
            GameEvent.PlayerDoWandNext(Player, false, () => OnPlayerDoWandNext(Player, false));
        else
            GameEvent.PlayerDoChoice(Player, () => OnPlayerDoChoice(Player));
    }


    private void OnPlayerDoChoice(IPlayer Player)
    {
        Player.DoChoice();
    } //Choice Event

    private void OnPlayerDoMediate(IPlayer Player, int RuneStoneAdd)
    {
        Player.DoMediate(RuneStoneAdd);
        GameEvent.PlayerDoWandNext(Player, true, () => OnPlayerDoWandNext(Player, true));
    } //Mediate Event

    private void OnPlayerDoCollect(IPlayer Player, ICard Card)
    {
        Player.DoCollect(Card);
        Card.DoCollectActive(Player);
        GameEvent.CardOriginActive(Card, () => OnCardAbilityOriginActive(Card));
    } //Collect Event


    private void OnCardAbilityOriginActive(ICard Card)
    {
        Card.DoOriginActive();
        GameEvent.PlayerDoWandNext(Card.Player, true, () => OnPlayerDoWandNext(Card.Player, true));
    } //Origin Event


    private void OnPlayerDoWandNext(IPlayer Player, bool CardActive)
    {
        Player.DoWandNext();
        if (CardActive)
            GameEvent.PlayerDoWandActive(Player, () => OnPlayerDoWandActive(Player));
        else
            GameEvent.PlayerEnd(Player);
    }

    private void OnPlayerDoWandActive(IPlayer Player)
    {
        Player.DoWandActive();
        GameEvent.CardAttack(Player.CardQueue[Player.WandStep], () => OnCardAttack(Player.CardQueue[Player.WandStep]));
    }


    private void OnCardAttack(ICard Card)
    {
        Card.DoAttackActive();
        for (int i = 0; i < m_player.Count; i++)
        {
            if (m_player[i] == Card.Player)
                continue;
            m_player[i].HealthChange(-Card.AttackCombine);
        }
        GameEvent.CardEnergyFill(Card, () => OnCardEnergyFill(Card));
    } //Attack Event

    private void OnCardEnergyFill(ICard Card)
    {
        Card.DoEnergyFill(1);
        GameEvent.CardEnergyCheck(Card, () => OnCardEnergyCheck(Card));
    } //Energy Event

    private void OnCardEnergyCheck(ICard Card)
    {
        Card.DoEnergyCheck();
        if (Card.EnergyFull)
            GameEvent.CardEnergyActive(Card, () => OnCardEnergyActive(Card));
        //else?
    }

    private void OnCardEnergyActive(ICard Card)
    {
        Card.DoEnergyActive();
        GameEvent.CardClassActive(Card, () => OnCardClassActive(Card));
    }

    private void OnCardClassActive(ICard Card)
    {
        Card.DoClassActive();
        GameEvent.CardSpellActive(Card, () => OnCardSpellActive(Card));
    } //Class Event

    private void OnCardSpellActive(ICard Card)
    {
        Card.DoSpellActive();
        GameEvent.PlayerContinueCheck(Card.Player, () => OnPlayerContinueCheck(Card.Player));
    } //Spell Event


    private void OnPlayerContinueCheck(IPlayer Player)
    {
        Player.DoContinueCheck(Player);
        if (Player.CardQueue.Exists(t => t.EnergyFull))
            GameEvent.PlayerContinue(Player, () => OnPlayerContinue(Player));
        else
            GameEvent.PlayerEnd(Player);
    }

    private void OnPlayerContinue(IPlayer Player)
    {
        Player.DoContinue(Player);
    } //Continue Event

    private void OnPlayerEnd(IPlayer Player)
    {
        Player.DoEnd(Player);
        m_playerTurn++;
        if (m_playerTurn > m_player.Count - 1)
            m_playerTurn = 0;
        GameEvent.PlayerTurn(PlayerCurrent, () => OnPlayerTurn(PlayerCurrent));
    }
}