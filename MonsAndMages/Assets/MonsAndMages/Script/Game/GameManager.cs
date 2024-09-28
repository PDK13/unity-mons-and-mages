using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //

    private bool m_battle;

    [SerializeField] private CardConfig m_cardConfig;
    [SerializeField] private TweenConfig m_tweenConfig;
    [SerializeField] private ExplainConfig m_explainConfig;
    [SerializeField] private DiceConfig m_diceConfig;

    [Space]
    [SerializeField] private Transform m_playerContent;

    private List<IPlayer> m_playerQueue = new List<IPlayer>(); //Max 4 player, min 2 player in game

    private int m_playerIndex = 0;
    private ChoiceType m_playerChoice = ChoiceType.None;

    public bool Battle => m_battle;

    public CardConfig CardConfig => m_cardConfig;

    public TweenConfig TweenConfig => m_tweenConfig;

    public ExplainConfig ExplainConfig => m_explainConfig;

    public DiceConfig DiceConfig => m_diceConfig;

    public IPlayer[] PlayerQueue => m_playerQueue.ToArray();

    public IPlayer PlayerCurrent => m_playerQueue[m_playerIndex];

    public int PlayerIndex => m_playerIndex;

    public ChoiceType PlayerChoice => m_playerChoice;

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
            new PlayerData(0, m_baseIndex == 0, 10, 100),
            new PlayerData(1, m_baseIndex == 1, 10, 100),
        };
        GameEvent.InitPlayer(PlayerJoin);

        yield return new WaitForSeconds(2f);

        GameEvent.ViewArea(ViewType.Wild, () =>
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
        if (m_playerQueue.Exists(t => t == Player))
            return;
        m_playerQueue.Add(Player);
    }

    public IPlayer GetPlayer(int PlayerIndex)
    {
        return m_playerQueue[PlayerIndex];
    }

    public int GetPlayerIndex(IPlayer Player)
    {
        return m_playerQueue.FindIndex(t => t.Equals(Player));
    }

    //

    private void PlayerCurrentStart()
    {
        GameEvent.ViewArea(ViewType.Field, () =>
        {
            GameEvent.ViewPlayer(PlayerCurrent, () =>
            {
                PlayerStart(PlayerCurrent);
            });
        });
    } //Camera move to Field and PlayerQueue before start PlayerQueue's turn

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
                PlayerDoStaffNext(Player, false);
            else
                PlayerDoChoiceMediateOrCollect(Player);
        });
    }


    private void PlayerDoChoiceMediateOrCollect(IPlayer Player)
    {
        m_playerChoice = ChoiceType.MediateOrCollect;
        GameEvent.UiChoiceMediateOrCollect();
    } //Do Choice

    public void PlayerDoMediateStart(IPlayer Player, int RuneStoneAdd)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);

        Player.DoMediate(RuneStoneAdd, () => PlayerDoStaffNext(Player, true));
    }

    public void PlayerDoCollectStart(IPlayer Player, ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);

        Player.DoCollect(Card, () =>
        {
            Card.Renderer.maskable = false;
            var Point = Player.DoCollectReady();

            GameEvent.ViewArea(ViewType.Field, () =>
            {
                GameEvent.WildCardFill(null);
                GameEvent.ViewPlayer(Player, () =>
                {
                    Card.Pointer(Point.Pointer, Point.Centre, false, false);
                    Card.MoveBack(() => Card.Rumble(() =>
                    {
                        Card.Renderer.maskable = true;
                        Card.InfoShow(true);
                        Card.DoCollectActive(Player, () => PlayerDoStaffNext(Card.Player, true));
                    }));
                    //Move card first before active card collect event!
                });
                //Go back to field and player's area!
            });
            //After player's rune stone updated, start progess ui!
        });
    }


    public void CardOriginGhostDoChoice(ICard Card)
    {
        m_playerChoice = ChoiceType.CardOriginGhost;
        GameEvent.UiChoiceCardOriginGhost();
    } //Do Choice

    public void CardOriginGhostDoStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Card.MoveBack(() => Card.DoOriginGhostStart());
    }


    public void PlayerDoStaffNext(IPlayer Player, bool CardActive)
    {
        Player.DoStaffNext(() =>
        {
            if (CardActive)
                Player.DoStaffActive(() => CardManaCheck(Player));
            else
                PlayerEnd(Player);
        });
    }

    public void CardManaCheck(IPlayer Player)
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
                Card.EffectOutlineMana(() => PlayerDoCardManaActiveDoChoice(Player));
                CardManaActive = true;
            }
            else
                Card.EffectOutlineMana(null);
        }
        if (!CardManaActive)
            PlayerEnd(Player);
    }


    private void PlayerDoCardManaActiveDoChoice(IPlayer Player)
    {
        m_playerChoice = ChoiceType.CardFullMana;
        GameEvent.UiChoiceCardFullMana();
    } //Do Choice

    public void CardManaActiveStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        GameEvent.CardActiveMana(Card, () => Card.MoveBack(() => Card.DoManaActive(() => CardManaCheck(Card.Player))));
    }


    public void CardClassMagicAddictDoChoice(ICard Card)
    {
        m_playerChoice = ChoiceType.CardClassMagicAddict;
        GameEvent.UiChoiceCardClassMagicAddict();
    } //Do Choice

    public void CardClassMagicAddictStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Card.MoveBack(() => Card.DoClassMagicAddictStart());
    }


    public void CardClassFlyingDoChoice(ICard Card)
    {
        m_playerChoice = ChoiceType.CardClassFlying;
        GameEvent.UiChoiceCardClassFlying();
    } //Do Choice

    public void CardClassFlyingStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Card.MoveBack(() => Card.DoClassFlyingStart());
    }


    private void PlayerEnd(IPlayer Player)
    {
        Player.DoEnd(() =>
        {
            m_playerIndex++;
            if (m_playerIndex > m_playerQueue.Count - 1)
                m_playerIndex = 0;
            PlayerCurrentStart();
        });
    }
}