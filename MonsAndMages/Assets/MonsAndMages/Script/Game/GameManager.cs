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

    private int m_healthPointStart = 40;
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
            new PlayerData(0, m_playerIndex == 0, m_healthPointStart, m_runeStoneStart),
            new PlayerData(1, m_playerIndex == 1, m_healthPointStart, m_runeStoneStart),
        };
        GameEvent.InitPlayer(PlayerJoin);

        yield return new WaitForSeconds(2f);

        GameEvent.ViewWild(() =>
        {
            GameEvent.WildCardFill(() =>
            {
                m_gameStart = true;
                PlayerCurrentStart();
            });
        });
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

    private void PlayerCurrentStart()
    {
        GameEvent.ViewField(() =>
        {
            GameEvent.ViewPlayer(PlayerCurrent, () =>
            {
                PlayerStart(PlayerCurrent);
            });
        });
    }

    private void PlayerStart(IPlayer Player)
    {
        GameEvent.PlayerStart(PlayerCurrent, () =>
        {
            Debug.Log("PlayerStart");
            PlayerTakeRuneStoneFromSupply(Player, 1);
        });
    }


    private void PlayerTakeRuneStoneFromSupply(IPlayer Player, int Value)
    {
        Player.DoTakeRuneStoneFromSupply(Value);
        GameEvent.PlayerTakeRuneStoneFromSupply(Player, 1, () =>
        {
            Debug.Log("PlayerTakeRuneStoneFromSupply");
            PlayerTakeRuneStoneFromMediation(Player);
        });
    }

    private void PlayerTakeRuneStoneFromMediation(IPlayer Player)
    {
        Player.DoTakeRuneStoneFromMediation();
        GameEvent.PlayerTakeRuneStoneFromMediation(Player, () =>
        {
            Debug.Log("PlayerTakeRuneStoneFromMediation");
            PlayerStunnedCheck(Player);
        });
    }

    private void PlayerStunnedCheck(IPlayer Player)
    {
        Player.DoStunnedCheck();
        GameEvent.PlayerStunnedCheck(Player, () =>
        {
            Debug.Log("PlayerStunnedCheck"); //Not have ui yet!!
            if (Player.Stuned)
                PlayerDoWandNext(Player, false);
            else
                PlayerDoChoice(Player);
        });
    }


    private void PlayerDoChoice(IPlayer Player)
    {
        Player.DoChoice();
        GameEvent.PlayerDoChoice(Player, () =>
        {
            Debug.Log("PlayerDoChoice");
            //...
        });
    } //Choice Event

    public void PlayerDoMediate(IPlayer Player, int RuneStoneAdd)
    {
        GameEvent.PlayerDoMediate(Player, RuneStoneAdd, () =>
        {
            PlayerDoWandNext(Player, true);
        });
    } //Mediate Event

    public void PlayerDoCollect(IPlayer Player, ICard Card)
    {
        Player.DoCollect(Card);
        Card.DoCollectActive(Player);
        GameEvent.PlayerDoCollect(Player, Card, () =>
        {
            CardOriginActive(Card);
        });
    } //Collect Event


    private void CardOriginActive(ICard Card)
    {
        Card.DoOriginActive();
        GameEvent.CardOriginActive(Card, () =>
        {
            PlayerDoWandNext(Card.Player, true);
        });
    } //Origin Event


    private void PlayerDoWandNext(IPlayer Player, bool CardActive)
    {
        Player.DoWandNext();
        GameEvent.PlayerDoWandNext(Player, CardActive, () =>
        {
            if (CardActive)
                PlayerDoWandActive(Player);
            else
                GameEvent.PlayerEnd(Player, () => PlayerEnd(Player));
        });
    }

    private void PlayerDoWandActive(IPlayer Player)
    {
        Player.DoWandActive();
        GameEvent.PlayerDoWandActive(Player, () =>
        {
            CardAttack(Player.CardQueue[Player.WandStep]);
        });
    }


    private void CardAttack(ICard Card)
    {
        Card.DoAttackActive();
        GameEvent.CardAttack(Card, () =>
        {
            for (int i = 0; i < m_player.Count; i++)
            {
                if (m_player[i] == Card.Player)
                    continue;
                m_player[i].HealthChange(-Card.AttackCombine);
            }
            CardEnergyFill(Card);
        });
    } //Attack Event

    private void CardEnergyFill(ICard Card)
    {
        Card.DoEnergyFill(1);
        GameEvent.CardEnergyFill(Card, () =>
        {
            CardEnergyCheck(Card);
        });
    } //Energy Event

    private void CardEnergyCheck(ICard Card)
    {
        Card.DoEnergyCheck();
        GameEvent.CardEnergyCheck(Card, () =>
        {
            if (Card.EnergyFull)
                CardEnergyActive(Card);
            //else?
        });
    }

    private void CardEnergyActive(ICard Card)
    {
        Card.DoEnergyActive();
        GameEvent.CardEnergyActive(Card, () =>
        {
            CardClassActive(Card);
        });
    }

    private void CardClassActive(ICard Card)
    {
        Card.DoClassActive();
        GameEvent.CardClassActive(Card, () =>
        {
            CardSpellActive(Card);
        });
    } //Class Event

    private void CardSpellActive(ICard Card)
    {
        Card.DoSpellActive();
        GameEvent.CardSpellActive(Card, () =>
        {
            PlayerContinueCheck(Card.Player);
        });
    } //Spell Event


    private void PlayerContinueCheck(IPlayer Player)
    {
        Player.DoContinueCheck(Player);
        GameEvent.PlayerContinueCheck(Player, () =>
        {
            if (Player.CardQueue.Exists(t => t.EnergyFull))
                PlayerContinue(Player);
            else
                PlayerEnd(Player);
        });
    }

    private void PlayerContinue(IPlayer Player)
    {
        Player.DoContinue(Player);
        GameEvent.PlayerContinue(Player, () =>
        {
            //...
        });
    } //Continue Event

    private void PlayerEnd(IPlayer Player)
    {
        Player.DoEnd(Player);
        m_playerTurn++;
        if (m_playerTurn > m_player.Count - 1)
            m_playerTurn = 0;
        GameEvent.PlayerEnd(Player, () =>
        {
            PlayerCurrentStart();
        });
    }
}