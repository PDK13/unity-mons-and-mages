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

    [SerializeField][Min(0)] private int m_startIndex = 0;
    [SerializeField][Min(0)] private int m_baseIndex = 0;
    [SerializeField] private bool m_sameDevice = true;

    [Space]
    [SerializeField] private CardConfig m_cardConfig;
    [SerializeField] private Transform m_playerContent;

    private List<IPlayer> m_player = new List<IPlayer>(); //Max 4 player, min 2 player in game

    private int m_playerIndex = 0;
    private bool m_playerChoice = false;

    //

    public CardConfig CardConfig => m_cardConfig;

    public IPlayer PlayerCurrent => m_player[m_playerIndex];

    public bool PlayerChoice => m_playerChoice;

    private void Awake()
    {
        GameManager.instance = this;
    }

    private IEnumerator Start()
    {
        m_playerIndex = m_startIndex;

        GameEvent.Init();

        PlayerData[] PlayerJoin = new PlayerData[2]
        {
            new PlayerData(0, m_baseIndex == 0, 40, 5),
            new PlayerData(1, m_baseIndex == 1, 40, 5),
        };
        GameEvent.InitPlayer(PlayerJoin);

        yield return new WaitForSeconds(2f);

        GameEvent.View(ViewType.Wild, () =>
        {
            GameEvent.WildCardFill(() =>
            {
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
        GameEvent.View(ViewType.Field, () =>
        {
            GameEvent.ViewUi(true);
            GameEvent.ViewPlayer(PlayerCurrent, () =>
            {
                PlayerStart(PlayerCurrent);
            });
        });
    } //Camera move to Field and Player before start Player's turn

    private void PlayerStart(IPlayer Player)
    {
        GameEvent.PlayerStart(PlayerCurrent, () =>
        {
            PlayerTakeRuneStoneFromSupply(Player, 1);
        });
    }


    private void PlayerTakeRuneStoneFromSupply(IPlayer Player, int Value)
    {
        Player.DoTakeRuneStoneFromSupply(Value);
        GameEvent.PlayerTakeRuneStoneFromSupply(Player, 1, () =>
        {
            PlayerTakeRuneStoneFromMediation(Player);
        });
    }

    private void PlayerTakeRuneStoneFromMediation(IPlayer Player)
    {
        Player.DoTakeRuneStoneFromMediation();
        GameEvent.PlayerTakeRuneStoneFromMediation(Player, () =>
        {
            PlayerStunnedCheck(Player);
        });
    }

    private void PlayerStunnedCheck(IPlayer Player)
    {
        Player.DoStunnedCheck();
        GameEvent.PlayerStunnedCheck(Player, () =>
        {
            if (Player.Stuned)
                PlayerDoWandNext(Player, false);
            else
                PlayerDoChoice(Player);
        });
    }


    private void PlayerDoChoice(IPlayer Player)
    {
        GameEvent.ViewUi(true);
        Player.DoChoice();
        GameEvent.PlayerDoChoice(Player, () =>
        {
            if (Player.Base)
                m_playerChoice = true;
        });
    } //Choice Event


    public void PlayerDoMediate(IPlayer Player, int RuneStoneAdd)
    {
        m_playerChoice = false;
        GameEvent.PlayerDoMediate(Player, RuneStoneAdd, () =>
        {
            PlayerDoWandNext(Player, true);
        });
    } //Mediate Event

    public void PlayerDoCollect(IPlayer Player, ICard Card)
    {
        m_playerChoice = false;
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
        m_playerIndex++;
        if (m_playerIndex > m_player.Count - 1)
            m_playerIndex = 0;
        GameEvent.PlayerEnd(Player, () =>
        {
            PlayerCurrentStart();
        });
    }
}