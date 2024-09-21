using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //

    private bool m_battle;

    [SerializeField] private CardConfig m_cardConfig;
    [SerializeField] private TweenConfig m_tweenConfig;
    [SerializeField] private ExplainConfig m_explainConfig;

    [Space]
    [SerializeField] private Transform m_playerContent;

    private List<IPlayer> m_player = new List<IPlayer>(); //Max 4 player, min 2 player in game

    private int m_playerIndex = 0;
    private bool m_playerChoice = false;

    public bool Battle => m_battle;

    public CardConfig CardConfig => m_cardConfig;

    public TweenConfig TweenConfig => m_tweenConfig;

    public IPlayer PlayerCurrent => m_player[m_playerIndex];

    public int PlayerIndex => m_playerIndex;

    public bool PlayerChoice => m_playerChoice;

    //

    [Space]
    [SerializeField][Min(0)] private int m_startIndex = 0;
    [SerializeField][Min(0)] private int m_baseIndex = 0;

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
                m_battle = true;
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

    public int GetPlayerIndex(IPlayer Player)
    {
        return m_player.FindIndex(t => t.Equals(Player));
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
                PlayerDostaffNext(Player, false);
            else
                PlayerDoChoice(Player);
        });
    }


    private void PlayerDoChoice(IPlayer Player)
    {
        Player.DoChoice(() =>
        {
            m_playerChoice = true;
            GameEvent.ViewUiShow(ViewType.Field, true);
        });
    } //Choice Event


    public void PlayerDoMediate(IPlayer Player, int RuneStoneAdd)
    {
        m_playerChoice = false;
        Player.DoMediate(RuneStoneAdd, () => PlayerDostaffNext(Player, true));
    } //Mediate Event

    public void PlayerDoCollect(IPlayer Player, ICard Card)
    {
        m_playerChoice = false;
        Player.DoCollect(Card, () => CardOriginActive(Card));
    } //Collect Event


    private void CardOriginActive(ICard Card)
    {
        Card.DoOriginActive(() => PlayerDostaffNext(Card.Player, true));
    } //Origin Event


    private void PlayerDostaffNext(IPlayer Player, bool CardActive)
    {
        Player.DostaffNext(() =>
        {
            if (CardActive)
                PlayerDostaffActive(Player);
            else
                PlayerEnd(Player);
        });
    }

    private void PlayerDostaffActive(IPlayer Player)
    {
        Player.DostaffActive(() => CardAttack(Player.CardQueue[Player.staffStep]));
    }


    private void CardAttack(ICard Card)
    {
        if (Card != null)
        {
            if (Card.Name == CardNameType.Stage)
                PlayerEnd(PlayerCurrent);
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
                    CardManaFill(Card);
                });
            }
        }
        else
            PlayerEnd(PlayerCurrent);
    } //Attack Event

    private void CardManaFill(ICard Card)
    {
        Card.DoManaFill(1, () => CardManaCheck(Card.Player));
    } //ManaPoint Event

    private void CardManaCheck(IPlayer Player)
    {
        bool CardManaActive = false;
        foreach (var Card in Player.CardQueue)
        {
            if (Card == null)
                continue;

            if (!Card.ManaFull)
                continue;

            if (!CardManaActive)
            {
                Card.EffectOutlineMana(() => PlayerDoCardManaActiveChoice(Player));
                CardManaActive = true;
            }
            else
                Card.EffectOutlineMana(null);
        }
        if (!CardManaActive)
            PlayerEnd(Player);
    }

    private void PlayerDoCardManaActiveChoice(IPlayer Player)
    {
        Player.CardManaActiveDoChoice(() =>
        {
            m_playerChoice = true;
        });
    }


    public void CardManaActive(ICard Card)
    {
        Card.DoManaActive(() => CardClassActive(Card));
    }

    private void CardClassActive(ICard Card)
    {
        Card.DoClassActive(() => CardSpellActive(Card));
    } //Class Event

    private void CardSpellActive(ICard Card)
    {
        Card.DoSpellActive(() => CardManaCheck(Card.Player));
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