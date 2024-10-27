using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //

    [SerializeField] private CardConfig m_cardConfig;
    [SerializeField] private TweenConfig m_tweenConfig;
    [SerializeField] private ExplainConfig m_explainConfig;
    [SerializeField] private DiceConfig m_diceConfig;
    [SerializeField] private TutorialConfig m_tutorialConfig;

    private List<IPlayer> m_playerQueue = new List<IPlayer>(); //Max 4 player, min 2 player in game
    private int m_playerIndex = 0;
    private ChoiceType m_playerChoice = ChoiceType.None;

    [Space]
    [SerializeField][Min(0)] private int m_startIndex = 0;
    [SerializeField][Min(0)] private int m_baseIndex = 0;

    private TutorialConfig m_tutorialCurrent = null;
    private int m_tutorialIndex = 0;

    //

    public CardConfig CardConfig => m_cardConfig;

    public TweenConfig TweenConfig => m_tweenConfig;

    public ExplainConfig ExplainConfig => m_explainConfig;

    public DiceConfig DiceConfig => m_diceConfig;

    public IPlayer[] PlayerQueue => m_playerQueue.ToArray();

    public IPlayer PlayerCurrent => m_playerQueue[m_playerIndex];

    public int PlayerIndex => m_playerIndex;

    public ChoiceType PlayerChoice => m_playerChoice;

    public bool TutorialActive => m_tutorialCurrent != null;

    public List<CardNameType> TutorialWild => m_tutorialCurrent.Wild;

    public TutorialConfigData TutorialStepCurrent => m_tutorialCurrent.Step[m_tutorialIndex];

    //

    private void Awake()
    {
        GameManager.instance = this;
    }

    //

    public void GameStart()
    {
        GameEvent.Start();

        m_playerQueue = new List<IPlayer>();
        m_playerIndex = m_startIndex;
        m_playerChoice = ChoiceType.None;

        GameEvent.Init();

        PlayerData[] PlayerJoin = new PlayerData[2]
        {
            new PlayerData(0, m_baseIndex == 0, 40, 5),
            new PlayerData(1, m_baseIndex == 1, 40, 5),
        };
        GameEvent.InitPlayer(PlayerJoin);

        StartCoroutine(IGameStart());
    }

    public void GameTutorial()
    {
        GameEvent.Start();

        TutorialStart();

        m_playerQueue = new List<IPlayer>();
        m_playerIndex = m_startIndex;
        m_playerChoice = ChoiceType.None;

        GameEvent.Init();

        PlayerData[] PlayerJoin = new PlayerData[2]
        {
            new PlayerData(0, m_baseIndex == 0, 100, 100),
            new PlayerData(1, m_baseIndex == 1, 100, 100),
        };
        GameEvent.InitPlayer(PlayerJoin);

        StartCoroutine(IGameStart());
    }

    private IEnumerator IGameStart()
    {
        yield return new WaitForSeconds(2f);

        GameEvent.ViewArea(ViewType.Wild, () =>
        {
            GameEvent.WildCardFill(() =>
            {
                PlayerCurrentStart();
            });
        });
    }

    //

    public void GameEnd()
    {
        StopAllCoroutines();

        m_tutorialCurrent = null;
        m_tutorialIndex = 0;

        m_playerQueue = new List<IPlayer>();
        m_playerIndex = m_startIndex;
        m_playerChoice = ChoiceType.None;

        GameEvent.End();
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
                PlayerCurrent.DoStart();
            });
        });
    } //Camera move to Field and PlayerQueue before start PlayerQueue's turn


    public void PlayerDoMediateOrCollectChoice(IPlayer Player)
    {
        m_playerChoice = ChoiceType.MediateOrCollect;
        TutorialCheck();
        GameEvent.PlayerDoChoice(PlayerCurrent, () => GameEvent.UiChoiceMediateOrCollect());
    } //Do Choice

    public void PlayerDoMediateStart(IPlayer Player, int RuneStoneAdd)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Player.DoMediate(RuneStoneAdd, () => Player.DoStaffNext(true));
    }

    public void PlayerDoCollectStart(IPlayer Player, ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Player.DoCollect(Card, () => Player.DoStaffNext(true));
    }


    public void CardOriginWoodlandChoice(ICard Card)
    {
        m_playerChoice = ChoiceType.CardOriginWoodland;
        TutorialCheck();
        GameEvent.UiChoiceCardOriginWoodland();
    } //Do Choice

    public void CardOriginWoodlandDoStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Card.DoMoveBack(() => Card.DoOriginWoodlandStart());
    }


    public void CardOriginGhostChoice(ICard Card)
    {
        m_playerChoice = ChoiceType.CardOriginGhost;
        TutorialCheck();
        GameEvent.UiChoiceCardOriginGhost();
    } //Do Choice

    public void CardOriginGhostDoStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Card.DoMoveBack(() => Card.DoOriginGhostStart());
    }


    public void PlayerDoCardManaActiveChoice(IPlayer Player)
    {
        m_playerChoice = ChoiceType.CardFullMana;
        TutorialCheck();
        GameEvent.UiChoiceCardFullMana();
    } //Do Choice

    public void CardManaActiveStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        GameEvent.CardActiveMana(Card, () => Card.DoMoveBack(() => Card.DoManaActive(() => Card.Player.ProgessCheck())));
    }


    public void CardClassMagicAddictChoice(ICard Card)
    {
        m_playerChoice = ChoiceType.CardClassMagicAddict;
        TutorialCheck();
        GameEvent.UiChoiceCardClassMagicAddict();
    } //Do Choice

    public void CardClassMagicAddictStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Card.DoMoveBack(() => Card.DoClassMagicAddictStart());
    }


    public void CardClassFlyingChoice(ICard Card)
    {
        m_playerChoice = ChoiceType.CardClassFlying;
        TutorialCheck();
        GameEvent.UiChoiceCardClassFlying();
    } //Do Choices

    public void CardClassFlyingStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Card.DoMoveBack(() => Card.DoClassFlyingStart());
    }


    public void CardSpellChoice(ICard Card)
    {
        m_playerChoice = ChoiceType.CardSpell;
        TutorialCheck();
        GameEvent.UiChoiceCardSpell();
    } //Do Choices

    public void CardSpellStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Card.DoMoveBack(() => Card.DoSpellStart());
    }


    public void CardEnterChoice(ICard Card)
    {
        m_playerChoice = ChoiceType.CardEnter;
        TutorialCheck();
        GameEvent.UiChoiceCardEnter();
    } //Do Choices

    public void CardEnterStart(ICard Card)
    {
        m_playerChoice = ChoiceType.None;
        GameEvent.UiChoiceHide();
        GameEvent.UiInfoHide(true, false);
        Card.DoMoveBack(() => Card.DoEnterStart());
    }


    public void PlayerEnd(IPlayer Player)
    {
        m_playerIndex++;
        if (m_playerIndex > m_playerQueue.Count - 1)
            m_playerIndex = 0;
        PlayerCurrentStart();
    }


    public void TutorialStart()
    {
        if (m_tutorialCurrent != null)
            return;
        m_tutorialCurrent = m_tutorialConfig;
        m_tutorialIndex = 0;
    }

    public void TutorialContinue(bool CheckNext)
    {
        if (m_tutorialCurrent == null)
            return;
        m_tutorialIndex++;
        if (m_tutorialIndex > m_tutorialCurrent.Step.Count - 1)
            TutorialEnd();
        else
        if (CheckNext)
            TutorialCheck();
    }

    public void TutorialCheck()
    {
        if (!TutorialActive)
            return;

        switch (TutorialStepCurrent.Type)
        {
            case TutorialStepType.Box:
                //...
                break;
            case TutorialStepType.Button:
                //Progess in Player View script
                break;
            case TutorialStepType.Card:
                GameEvent.TutorialCard(TutorialStepCurrent.CardValue);
                break;
        }
    }

    public void TutorialEnd()
    {
        m_tutorialCurrent = null;
        m_tutorialIndex = 0;
    }
}