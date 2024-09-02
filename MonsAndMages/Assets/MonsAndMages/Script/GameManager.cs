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
        GameEvent.onPlayerStart += OnPlayerStart;
        GameEvent.onPlayerTakeRuneStoneFromSupply += OnPlayerTakeRuneStoneFromSupply;
        GameEvent.onPlayerTakeRuneStoneFromMediation += OnPlayerTakeRuneStoneFromMediation;
        GameEvent.onPlayerCheckStunned += OnPlayerCheckStunned;

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

        GameEvent.onPlayerEndCheck += OnPlayerEndCheck;
        GameEvent.onPlayerEnd += OnPlayerEnd;
    }

    private void OnDisable()
    {
        GameEvent.onPlayerStart -= OnPlayerStart;
        GameEvent.onPlayerTakeRuneStoneFromSupply -= OnPlayerTakeRuneStoneFromSupply;
        GameEvent.onPlayerTakeRuneStoneFromMediation -= OnPlayerTakeRuneStoneFromMediation;
        GameEvent.onPlayerCheckStunned -= OnPlayerCheckStunned;

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

        GameEvent.onPlayerEndCheck -= OnPlayerEndCheck;
        GameEvent.onPlayerEnd -= OnPlayerEnd;
    }

    private void Start()
    {
        GameEvent.PlayerStart(PlayerCurrent, false);
    }

    //

    private void OnPlayerStart(IPlayer Player, bool Update)
    {
        if (Update)
            GameEvent.PlayerTakeRuneStoneFromSupply(Player, 1, false);
    }

    private void OnPlayerTakeRuneStoneFromSupply(IPlayer Player, int Value, bool Update)
    {
        if (Update)
        {
            Player.TakeRuneStoneFromSupply(Value);
            GameEvent.PlayerTakeRuneStoneFromMediation(Player, false);
        }
    }

    private void OnPlayerTakeRuneStoneFromMediation(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.TakeRuneStoneFromMediation();
            GameEvent.PlayerCheckStuned(Player, false);
        }
    }

    private void OnPlayerCheckStunned(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.CheckStunned();
            //
            if (Player.Stuned)
                GameEvent.PlayerDoWandNext(Player, false, false);
            else
                GameEvent.PlayerDoChoice(Player, false);
        }
    }


    private void OnPlayerDoChoice(IPlayer Player, bool Update)
    {
        if (Update)
            Player.DoChoice();
    }

    private void OnPlayerDoMediate(IPlayer Player, int RuneStoneAdd, bool Update)
    {
        if (Update)
        {
            Player.DoMediate(RuneStoneAdd);
            GameEvent.PlayerDoWandNext(Player, true, false);
        }
    } //Mediate Event

    private void OnPlayerDoCollect(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCollect(Card);
            Card.DoCollectActive(Player);
            GameEvent.CardOriginActive(Player, Card, false);
        }
    } //Collect Event


    private void OnCardAbilityOriginActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardOriginActive(Card);
            Card.OriginActive(Player);
            GameEvent.PlayerDoWandNext(Player, true, false);
        }
    } //Origin Event


    private void OnPlayerDoWandNext(IPlayer Player, bool CardActive, bool Update)
    {
        if (Update)
        {
            Player.DoWandNext();
            if (CardActive)
                GameEvent.PlayerDoWandActive(Player, false);
            //else??
        }
    }

    private void OnPlayerDoWandActive(IPlayer Player, bool Update)
    {
        if (Update)
        {
            Player.DoWandActive();
            Player.CardQueue[Player.WandStep].WandActive(Player);
            GameEvent.CardAttack(Player, Player.CardQueue[Player.WandStep], false);
        }
    }


    private void OnCardAttack(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardAttack();
            Card.AttackActive(Player);
            //All other Player take damage
            GameEvent.CardEnergyFill(Player, Card, false);
        }
    } //Attack Event

    private void OnCardEnergyFill(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardEnergyFill(Card);
            Card.EnergyFill(Player, 1);
            GameEvent.CardEnergyCheck(Player, Card, false);
        }
    } //Energy Event

    private void OnCardEnergyCheck(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardEnergyCheck(Card);
            Card.EnergyCheck();
            if (Card.EnergyFull)
                GameEvent.CardEnergyActive(Player, Card, false);
            //else?
        }
    }

    private void OnCardEnergyActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardEnergyActive(Card);
            Card.EnergyActive(Player);
            GameEvent.CardClassActive(Player, Card, false);
        }
    }

    private void OnCardClassActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardClassActive(Card);
            Card.ClassActive(Player);
            GameEvent.CardSpellActive(Player, Card, false);
        }
    } //Class Event

    private void OnCardSpellActive(IPlayer Player, ICard Card, bool Update)
    {
        if (Update)
        {
            Player.DoCardSpellActive(Card);
            Card.SpellActive(Player);
            //Check other card??
        }
    } //Spell Event


    private void OnPlayerEndCheck(IPlayer Player)
    {
        //Check any card can active energy?!
    }

    private void OnPlayerEnd(IPlayer Player)
    {
        m_playerTurn++;
        if (m_playerTurn > m_player.Count - 1)
            m_playerTurn = 0;
        GameEvent.PlayerStart(PlayerCurrent, false);
    }
}