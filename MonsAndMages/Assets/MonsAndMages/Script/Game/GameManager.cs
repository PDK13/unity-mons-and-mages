using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //

    [SerializeField] private CardConfig m_cardConfig;
    [SerializeField] private TweenConfig m_tweenConfig;
    [SerializeField] private Transform m_playerContent;

    private List<IPlayer> m_player = new List<IPlayer>(); //Max 4 player, min 2 player in game

    private int m_playerIndex = 0;
    private bool m_playerChoice = false;

    public CardConfig CardConfig => m_cardConfig;

    public TweenConfig TweenConfig => m_tweenConfig;

    public IPlayer PlayerCurrent => m_player[m_playerIndex];

    public int PlayerIndex => m_playerIndex;

    public bool PlayerChoice => m_playerChoice;

    //

    [Space]
    [SerializeField][Min(0)] private int m_startIndex = 0;
    [SerializeField][Min(0)] private int m_baseIndex = 0;
    [SerializeField] private bool m_sameDevice = true;

    public bool SameDevice => m_sameDevice;

    //

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
            new PlayerData(0, m_baseIndex == 0, 40, 3),
            new PlayerData(1, m_baseIndex == 1, 40, 3),
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
            GameEvent.ViewPlayer(PlayerCurrent, () =>
            {
                PlayerStart(PlayerCurrent);
            });
        });
    } //Camera move to Field and Player before start Player's turn

    private void PlayerStart(IPlayer Player)
    {
        Player.DoStart(() => PlayerTakeRuneStoneFromSupply(Player, 1));
    }


    private void PlayerTakeRuneStoneFromSupply(IPlayer Player, int Value)
    {
        Player.DoTakeRuneStoneFromSupply(Value, () => PlayerTakeRuneStoneFromMediation(Player));
    }

    private void PlayerTakeRuneStoneFromMediation(IPlayer Player)
    {
        Player.DoTakeRuneStoneFromMediation(() => PlayerStunnedCheck(Player));
    }

    private void PlayerStunnedCheck(IPlayer Player)
    {
        Player.DoStunnedCheck((Stunned) =>
        {
            if (Stunned)
                PlayerDoWandNext(Player, false);
            else
                PlayerDoChoice(Player);
        });
    }


    private void PlayerDoChoice(IPlayer Player)
    {
        Player.DoChoice(() =>
        {
            m_playerChoice = Player.Base || SameDevice;
            GameEvent.ViewUiShow(ViewType.Field);
        });
    } //Choice Event


    public void PlayerDoMediate(IPlayer Player, int RuneStoneAdd)
    {
        m_playerChoice = false;
        Player.DoMediate(RuneStoneAdd, () => PlayerDoWandNext(Player, true));
    } //Mediate Event

    public void PlayerDoCollect(IPlayer Player, ICard Card)
    {
        m_playerChoice = false;
        Player.DoCollect(Card, () => CardOriginActive(Card));
    } //Collect Event


    private void CardOriginActive(ICard Card)
    {
        Card.DoOriginActive(() => PlayerDoWandNext(Card.Player, true));
    } //Origin Event


    private void PlayerDoWandNext(IPlayer Player, bool CardActive)
    {
        Player.DoWandNext(() =>
        {
            if (CardActive)
                PlayerDoWandActive(Player);
            else
                PlayerEnd(Player);
        });
    }

    private void PlayerDoWandActive(IPlayer Player)
    {
        Player.DoWandActive(() => CardAttack(Player.CardQueue[Player.WandStep]));
    }


    private void CardAttack(ICard Card)
    {
        if (Card != null)
        {
            if (Card.Name == CardNameType.Stage)
                Card.DoAttackActive(() => PlayerEnd(PlayerCurrent));
            else
            {
                Card.DoAttackActive(() =>
                {
                    for (int i = 0; i < m_player.Count; i++)
                    {
                        if (m_player[i] == Card.Player)
                            continue;
                        m_player[i].HealthChange(-Card.AttackCombine, null);
                    }
                    CardEnergyFill(Card);
                });
            }
        }
        else
            PlayerEnd(PlayerCurrent);
    } //Attack Event

    private void CardEnergyFill(ICard Card)
    {
        Card.DoEnergyFill(1, () => CardEnergyCheck(Card.Player));
    } //Energy Event

    private void CardEnergyCheck(IPlayer Player)
    {
        bool CardEnergyActive = false;
        foreach (var Card in Player.CardQueue)
        {
            if (Card == null)
                continue;

            if (!Card.EnergyFull)
                continue;

            if (!CardEnergyActive)
            {
                Card.EffectOutlineEnergy(0.1f, () => PlayerDoCardEnergyActiveChoice(Player));
                CardEnergyActive = true;
            }
            else
                Card.EffectOutlineEnergy(0.1f, null);
        }
        if (!CardEnergyActive)
            PlayerEnd(Player);
    }

    private void PlayerDoCardEnergyActiveChoice(IPlayer Player)
    {
        Player.CardEnergyActiveDoChoice(() =>
        {
            m_playerChoice = Player.Base || SameDevice;
        });
    }


    public void CardEnergyActive(ICard Card)
    {
        Card.DoEnergyActive(() => CardClassActive(Card));
    }

    private void CardClassActive(ICard Card)
    {
        Card.DoClassActive(() => CardSpellActive(Card));
    } //Class Event

    private void CardSpellActive(ICard Card)
    {
        Card.DoSpellActive(() => CardEnergyCheck(Card.Player));
    } //Spell Event


    private void PlayerEnd(IPlayer Player)
    {
        Player.DoEnd(() =>
        {
            m_playerIndex++;
            if (m_playerIndex > m_player.Count - 1)
                m_playerIndex = 0;
            PlayerCurrentStart();
        });
    }
}