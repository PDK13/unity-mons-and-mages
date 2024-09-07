using DG.Tweening;
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

    //

    private void OnEnable()
    {
        GameEvent.onGameStart += OnGameStart;

        GameEvent.onPlayerTurn += OnPlayerTurn;
        GameEvent.onPlayerStart += OnPlayerStart;
        GameEvent.onPlayerTakeRuneStoneFromSupply += OnPlayerTakeRuneStoneFromSupply;
        GameEvent.onPlayerTakeRuneStoneFromMediation += OnPlayerTakeRuneStoneFromMediation;
        GameEvent.onPlayerStunnedCheck += OnPlayerCheckStunned;

        GameEvent.onPlayerDoChoice += OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate += OnPlayerDoMediate; //Mediate Event
        GameEvent.onPlayerDoCollect += OnPlayerDoCollect; //Collect Event

        GameEvent.onCardOriginActive += OnCardAbilityOriginActive; //Origin Event

        GameEvent.onPlayerDoWandNext += OnPlayerDoWandNext;
        GameEvent.onPlayerDoWandActive += OnPlayerDoWandActive;

        GameEvent.onCardAttack += OnCardAttack; //Attack Event
        GameEvent.onCardEnergyFill += OnCardEnergyFill; //Energy Event
        GameEvent.onCardEnergyCheck += OnCardEnergyCheck;
        GameEvent.onCardEnergyActive += OnCardEnergyActive;
        GameEvent.onCardClassActive += OnCardClassActive; //Class Event
        GameEvent.onCardSpellActive += OnCardSpellActive; //Spell Event

        GameEvent.onPlayerContinueCheck += OnPlayerContinueCheck;
        GameEvent.onPlayerContinue += OnPlayerContinue;
        GameEvent.onPlayerEnd += OnPlayerEnd;
    }

    private void OnDisable()
    {
        GameEvent.onGameStart -= OnGameStart;

        GameEvent.onPlayerTurn -= OnPlayerTurn;
        GameEvent.onPlayerStart -= OnPlayerStart;
        GameEvent.onPlayerTakeRuneStoneFromSupply -= OnPlayerTakeRuneStoneFromSupply;
        GameEvent.onPlayerTakeRuneStoneFromMediation -= OnPlayerTakeRuneStoneFromMediation;
        GameEvent.onPlayerStunnedCheck -= OnPlayerCheckStunned;

        GameEvent.onPlayerDoChoice -= OnPlayerDoChoice;
        GameEvent.onPlayerDoMediate -= OnPlayerDoMediate;
        GameEvent.onPlayerDoCollect -= OnPlayerDoCollect;

        GameEvent.onCardOriginActive -= OnCardAbilityOriginActive;

        GameEvent.onPlayerDoWandNext -= OnPlayerDoWandNext;
        GameEvent.onPlayerDoWandActive -= OnPlayerDoWandActive;

        GameEvent.onCardAttack -= OnCardAttack;
        GameEvent.onCardEnergyFill -= OnCardEnergyFill;
        GameEvent.onCardEnergyCheck -= OnCardEnergyCheck;
        GameEvent.onCardEnergyActive -= OnCardEnergyActive;
        GameEvent.onCardClassActive -= OnCardClassActive;
        GameEvent.onCardSpellActive -= OnCardSpellActive;

        GameEvent.onPlayerContinueCheck -= OnPlayerContinueCheck;
        GameEvent.onPlayerContinue -= OnPlayerContinue;
        GameEvent.onPlayerEnd -= OnPlayerEnd;
    }

    private void Awake()
    {
        GameManager.instance = this;
    }

    private IEnumerator Start()
    {
        Debug.Log("Init Game");

        m_playerTurn = m_PlayerStart;

        yield return null;

        GameEvent.Init();

        yield return null;

        PlayerData[] PlayerJoin = new PlayerData[2]
        {
            new PlayerData(0, m_playerIndex == 0),
            new PlayerData(1, m_playerIndex == 1),
        };
        GameEvent.InitPlayer(PlayerJoin);

        yield return new WaitForSeconds(3f);

        GameEvent.ViewWild();

        yield return new WaitForSeconds(2f);

        GameEvent.InitWild();

        yield return new WaitForSeconds(7f);

        GameEvent.ViewField();

        yield return new WaitForSeconds(2f);

        m_gameStart = true;

        GameEvent.GameStart();

        Debug.Log("Start Game");
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
        GameEvent.PlayerTurn(PlayerCurrent, true);
    }


    private void OnPlayerTurn(IPlayer Player, bool Update)
    {
        if (Update)
            StartCoroutine(IEPlayerTurn());
    }

    private IEnumerator IEPlayerTurn()
    {
        GameEvent.ViewPlayer(PlayerCurrent);

        yield return new WaitForSeconds(2f);

        GameEvent.PlayerStart(PlayerCurrent);
    }

    private void OnPlayerStart(IPlayer Player, bool Update)
    {
        if (Update)
            GameEvent.PlayerTakeRuneStoneFromSupply(Player, 1);
    }

    private void OnPlayerTakeRuneStoneFromSupply(IPlayer Player, int Value, bool Update)
    {
        if (Update)
        {
            Player.DoTakeRuneStoneFromSupply(Value);
            GameEvent.PlayerTakeRuneStoneFromMediation(Player);
        }
    }

    private void OnPlayerTakeRuneStoneFromMediation(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.DoTakeRuneStoneFromMediation();
            GameEvent.PlayerStunnedCheck(Player);
        }
    }

    private void OnPlayerCheckStunned(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.DoStunnedCheck();
            //
            if (Player.Stuned)
                GameEvent.PlayerDoWandNext(Player, false);
            else
                GameEvent.PlayerDoChoice(Player);
        }
    }


    private void OnPlayerDoChoice(IPlayer Player, bool Update)
    {
        if (Update)
            Player.DoChoice();
    } //Choice Event

    private void OnPlayerDoMediate(IPlayer Player, int RuneStoneAdd, bool Update)
    {
        if (Update)
        {
            Player.DoMediate(RuneStoneAdd);
            GameEvent.PlayerDoWandNext(Player, true);
        }
    } //Mediate Event

    private void OnPlayerDoCollect(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCollect(Card);
            Card.DoCollectActive(Player);
            GameEvent.CardOriginActive(Card);
        }
    } //Collect Event


    private void OnCardAbilityOriginActive(ICard Card, bool Update)
    {
        if (Update)
        {
            Card.DoOriginActive();
            GameEvent.PlayerDoWandNext(Card.Player, true);
        }
    } //Origin Event


    private void OnPlayerDoWandNext(IPlayer Player, bool CardActive, bool Update)
    {
        if (Update)
        {
            Player.DoWandNext();
            if (CardActive)
                GameEvent.PlayerDoWandActive(Player);
            else
                GameEvent.PlayerEnd(Player);
        }
    }

    private void OnPlayerDoWandActive(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.DoWandActive();
            GameEvent.CardAttack(Player.CardQueue[Player.WandStep]);
        }
    }


    private void OnCardAttack(ICard Card, bool Update)
    {
        if (Update)
        {
            Card.DoAttackActive();
            for (int i = 0; i < m_player.Count; i++)
            {
                if (m_player[i] == Card.Player)
                    continue;
                m_player[i].HealthChange(-Card.AttackCombine);
            }
            GameEvent.CardEnergyFill(Card);
        }
    } //Attack Event

    private void OnCardEnergyFill(ICard Card, bool Update)
    {
        if (Update)
        {
            Card.DoEnergyFill(1);
            GameEvent.CardEnergyCheck(Card);
        }
    } //Energy Event

    private void OnCardEnergyCheck(ICard Card, bool Update)
    {
        if (Update)
        {
            Card.DoEnergyCheck();
            if (Card.EnergyFull)
                GameEvent.CardEnergyActive(Card);
            //else?
        }
    }

    private void OnCardEnergyActive(ICard Card, bool Update)
    {
        if (Update)
        {
            Card.DoEnergyActive();
            GameEvent.CardClassActive(Card);
        }
    }

    private void OnCardClassActive(ICard Card, bool Update)
    {
        if (Update)
        {
            Card.DoClassActive();
            GameEvent.CardSpellActive(Card);
        }
    } //Class Event

    private void OnCardSpellActive(ICard Card, bool Update)
    {
        if (Update)
        {
            Card.DoSpellActive();
            GameEvent.PlayerContinueCheck(Card.Player);
        }
    } //Spell Event


    private void OnPlayerContinueCheck(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.DoContinueCheck(Player);
            if (Player.CardQueue.Exists(t => t.EnergyFull))
                GameEvent.PlayerContinue(Player);
            else
                GameEvent.PlayerEnd(Player);
        }
    }

    private void OnPlayerContinue(IPlayer Player, bool Update)
    {
        if (Update)
            Player.DoContinue(Player);
    } //Continue Event

    private void OnPlayerEnd(IPlayer Player)
    {
        Player.DoEnd(Player);
        m_playerTurn++;
        if (m_playerTurn > m_player.Count - 1)
            m_playerTurn = 0;
        GameEvent.PlayerStart(PlayerCurrent);
    }
}