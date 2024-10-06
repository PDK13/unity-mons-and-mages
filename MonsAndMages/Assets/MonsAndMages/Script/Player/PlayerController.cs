using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] private TextMeshProUGUI m_playerName;

    [Space]
    [SerializeField] private GameObject m_cardPoint;
    [SerializeField] private RectTransform m_cardContent;

    [Space]
    [SerializeField] private CardMediate[] m_cardMediation = new CardMediate[2];

    [Space]
    [SerializeField] private Transform m_boxGroup;

    [Space]
    [SerializeField] private RectTransform m_runeStoneBox;
    [SerializeField] private TextMeshProUGUI m_tmpRuneStone;

    [Space]
    [SerializeField] private RectTransform m_stunBox;
    [SerializeField] private TextMeshProUGUI m_tmpStun;

    [Space]
    [SerializeField] private RectTransform m_healthBox;
    [SerializeField] private TextMeshProUGUI m_tmpHealth;

    [Space]
    [SerializeField] private GameObject m_runeStoneIcon;

    [Space]
    [SerializeField] private StaffController m_staff;

    //

    private int m_index = 0;
    private bool m_base = false;
    private int m_healthPoint = 0;
    private int m_healthCurrent = 0;
    private int m_runeStone = 0;
    private int m_stunPoint = 0;
    private int m_stunCurrent = 0;
    private int[] m_mediation = new int[2] { 0, 0 };
    private List<ICard> m_cardQueue = new List<ICard>();
    private int m_staffStep = 0;

    private List<ICard> m_progessCard = new List<ICard>();
    public int m_progessMana = 0;
    public CardOriginType m_progessOrigin = CardOriginType.None;

    //

    private IEnumerator Start()
    {
        m_cardPoint.SetActive(false);
        m_runeStoneIcon.SetActive(false);

        yield return null;

        DoBoardReRange();
    }

    //

    private void InfoRuneStoneUpdate(Action OnComplete)
    {
        m_runeStoneBox.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            m_tmpRuneStone.text = RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
            m_runeStoneBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
        });
        GameEvent.PlayerRuneStoneUpdate(this, null);
    }

    private void InfoStunUpdate(Action OnComplete)
    {
        m_stunBox.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            m_tmpStun.text = StunCurrent.ToString();
            m_stunBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
        });
        GameEvent.PlayerStunnedUpdate(this, null);
    }

    private void InfoHealthUpdate(Action OnComplete)
    {
        m_healthBox.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            m_tmpHealth.text = HealthCurrent.ToString();
            m_healthBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
        });
        GameEvent.PlayerHealthUpdate(this, null);
    }

    private void InfoMediationUpdate(int Index, Action OnComplete)
    {
        m_cardMediation[Index].InfoRuneStoneUpdate(m_mediation[Index], OnComplete);
    }

    //IPlayer

    public int Index => m_index;

    public bool Base => m_base;

    public int HealthPoint => m_healthPoint;

    public int HealthCurrent => m_healthCurrent;

    public int RuneStone => m_runeStone;

    public int StunPoint => m_stunPoint;

    public int StunCurrent => m_stunCurrent;

    public bool Stuned => m_stunCurrent >= m_stunPoint;

    public int[] Mediation => m_mediation;

    public bool MediationEmty => m_mediation[0] == 0 || m_mediation[1] == 0;

    public ICard[] CardQueue => m_cardQueue.ToArray();

    public int StaffStep => m_staffStep;

    public ICard CardStaffCurrent => m_cardQueue[StaffStep];


    public RectTransform PointerLast => m_cardContent.GetChild(m_cardContent.childCount - 1).GetComponent<RectTransform>();


    public void Init(PlayerData Data)
    {
        m_index = Data.Index;
        m_base = Data.Base;
        m_healthPoint = Data.HealthPoint;
        m_healthCurrent = Data.HealthCurrent;
        m_runeStone = Data.RuneStone;
        m_stunPoint = Data.StunPoint;
        m_stunCurrent = Data.StunCurrent;
        m_mediation = Data.Mediation;
        for (int i = 0; i < m_cardContent.childCount; i++)
        {
            var CardStage = new CardData();
            CardStage.Named = "Stage";
            CardStage.Name = CardNameType.Stage;
            var Card = m_cardContent.GetChild(i).GetComponentInChildren<ICard>();
            Card.Init(CardStage);
            Card.Pointer = PointerLast;
            Card.Centre = m_cardContent.GetChild(i).GetComponent<RectTransform>();
            m_cardQueue.Add(Card);
        }

        m_playerName.text = "P" + (Index + 1).ToString();

        InfoRuneStoneUpdate(null);
        InfoStunUpdate(null);
        InfoHealthUpdate(null);
        InfoMediationUpdate(0, null);
        InfoMediationUpdate(1, null);

        GameManager.instance.PlayerJoin(this);
    }


    public void DoStart()
    {
        GameEvent.PlayerStart(this, () =>
        {
            DoTakeRuneStoneFromSupply(1, () =>
            {
                DoTakeRuneStoneFromMediation(() =>
                {
                    DoStunnedCheck();
                });
            });
        });
    }

    private void DoTakeRuneStoneFromSupply(int Value, Action OnComplete)
    {
        if (Value == 0)
        {
            OnComplete?.Invoke();
            return;
        }

        m_runeStone += Value;

        var RuneStone = Instantiate(m_runeStoneIcon, this.transform).GetComponent<RectTransform>();
        var RuneStoneFx = RuneStone.Find("fx-glow");
        var RuneStoneIcon = RuneStone.Find("icon");
        var RuneStoneIconScale = RuneStoneIcon.localScale;

        RuneStone.gameObject.SetActive(true);

        RuneStoneFx
            .DORotate(Vector3.forward * 359f, 1.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        Sequence RuneStoneIconTween = DOTween.Sequence();
        RuneStoneIconTween.Append(RuneStoneIcon.DOScale(RuneStoneIconScale + Vector3.one * 0.1f, 0.05f));
        RuneStoneIconTween.Append(RuneStoneIcon.DOScale(RuneStoneIconScale, 0.05f));
        RuneStoneIconTween.Append(RuneStone.DOMove(m_runeStoneBox.position, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            InfoRuneStoneUpdate(() =>
            {
                Destroy(RuneStone.gameObject, 0.2f);
                OnComplete?.Invoke();
            });
        }).SetDelay(0.25f));
        RuneStoneIconTween.Play();
    }

    private void DoTakeRuneStoneFromMediation(Action OnComplete)
    {
        var RuneStoneTake = false;
        for (int i = 0; i < m_mediation.Length; i++)
        {
            if (m_mediation[i] > 0)
            {
                RuneStoneTake = true;

                m_mediation[i] -= 2;
                m_runeStone += 2;

                InfoMediationUpdate(i, () =>
                {
                    var RuneStone = Instantiate(m_runeStoneIcon, this.transform).GetComponent<RectTransform>();
                    var RuneStoneFx = RuneStone.Find("fx-glow");
                    var RuneStoneIcon = RuneStone.Find("icon");
                    var RuneStoneIconScale = RuneStoneIcon.localScale;

                    RuneStone.gameObject.SetActive(true);

                    RuneStoneFx
                        .DORotate(Vector3.forward * 359f, 1.5f, RotateMode.FastBeyond360)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Restart);

                    Sequence RuneStoneIconTween = DOTween.Sequence();
                    RuneStoneIconTween.Append(RuneStoneIcon.DOScale(RuneStoneIconScale + Vector3.one * 0.1f, 0.05f));
                    RuneStoneIconTween.Append(RuneStoneIcon.DOScale(RuneStoneIconScale, 0.05f));
                    RuneStoneIconTween.Append(RuneStone.DOMove(m_runeStoneBox.position, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        InfoRuneStoneUpdate(() =>
                        {
                            Destroy(RuneStone.gameObject, 0.2f);
                            OnComplete?.Invoke();
                        });
                    }).SetDelay(0.25f));
                    RuneStoneIconTween.Play();
                });
            }
        }
        if (!RuneStoneTake)
            OnComplete?.Invoke();
    }

    private void DoStunnedCheck()
    {
        GameEvent.PlayerStunnedCheck(this, () =>
        {
            if (Stuned)
                StunChange(m_stunCurrent, () => DoStaffNext(false));
            else
                GameManager.instance.PlayerDoChoiceMediateOrCollect(this);
        });
    }


    public void DoMediate(int RuneStoneAdd, Action OnComplete)
    {
        m_runeStone -= RuneStoneAdd;
        InfoRuneStoneUpdate(() =>
        {
            if (m_mediation[0] == 0)
            {
                m_mediation[0] = RuneStoneAdd * 2;
                InfoMediationUpdate(0, OnComplete);
            }
            else
            if (m_mediation[1] == 0)
            {
                m_mediation[1] = RuneStoneAdd * 2;
                InfoMediationUpdate(1, OnComplete);
            }
            else
                OnComplete?.Invoke();
        });
    }


    public void DoCollect(ICard Card, Action OnComplete)
    {
        m_cardQueue.Add(Card);

        if (m_cardQueue[0].Name == CardNameType.Stage && m_cardQueue.Count > 5)
        {
            Destroy(m_cardQueue[0].Body.gameObject);
            m_cardQueue.RemoveAt(0);
            m_staffStep -= 1;
        }
        else
        {
            var Centre = Instantiate(m_cardPoint, m_cardContent).GetComponent<RectTransform>();
            Centre.gameObject.name = "card-point";
            Centre.gameObject.SetActive(true);
        }

        RuneStoneChange(-Card.RuneStoneCost, () =>
        {
            Card.Renderer.maskable = false;

            DoBoardReRange(Card);

            GameEvent.ViewArea(ViewType.Field, () =>
            {
                GameEvent.WildCardFill(null);
                GameEvent.ViewPlayer(this, () =>
                {
                    Card.Pointer = PointerLast;
                    Card.Centre = PointerLast;
                    Card.DoMoveBack(() => Card.DoRumble(() =>
                    {
                        Card.Renderer.maskable = true;
                        Card.InfoShow(true);
                        Card.DoCollectActive(this, OnComplete);
                    }));
                    //Move card first before active card collect event!
                });
                //Go back to field and player's area!
            });
            //After player's rune stone updated, start progess ui!
        });
    }

    public void DoBoardReRange(params ICard[] CardIgnore)
    {
        //CardCheck
        for (int i = 0; i < m_cardQueue.Count; i++)
        {
            if (CardIgnore.ToList().Exists(t => t.Equals(m_cardQueue[i])))
                continue;
            m_cardQueue[i].Pointer = PointerLast;
            m_cardQueue[i].Centre = m_cardContent.GetChild(i).GetComponent<RectTransform>();
            m_cardQueue[i].DoFixed();
        }
        //Staff
        m_staff.Pointer = PointerLast;
        m_staff.Centre = m_cardContent.GetChild(Mathf.Max(0, m_staffStep)).GetComponent<RectTransform>();
        m_staff.DoFixed();
    }


    public void DoOriginDragon(ICard Card, Action OnComplete)
    {
        var DragonCount = m_cardQueue.Count(t => t.Origin == CardOriginType.Dragon);
        DoOriginDragonProgess(Card, DragonCount, OnComplete);
    } //Origin Dragon Event

    private void DoOriginDragonProgess(ICard Card, int DragonLeft, Action OnComplete)
    {
        var DiceQueue = GameManager.instance.DiceConfig.Data;
        var DiceCount = DiceQueue.Count;
        var DiceIndex = UnityEngine.Random.Range(0, (DiceCount * 10) - 1) / 10;
        var DiceFace = DiceQueue[DiceIndex];

        DragonLeft--;

        GameEvent.OriginDragon(() =>
        {
            Card.DoRumble(() =>
            {
                var PlayerQueue = GameManager.instance.PlayerQueue;
                for (int i = 0; i < PlayerQueue.Length; i++)
                {
                    if (PlayerQueue[i].Equals(this))
                        PlayerQueue[i].HealthChange(-DiceFace.Bite, () =>
                        {
                            if (DragonLeft > 0)
                                DoOriginDragonProgess(Card, DragonLeft, OnComplete);
                            else
                                OnComplete?.Invoke();
                        });
                    else
                        PlayerQueue[i].HealthChange(-DiceFace.Dragon, null);
                }
            });
        });
    }

    public void DoOriginWoodlandReady(ICard Card)
    {
        var WoodlandCount = m_cardQueue.Count(t => t.Origin == CardOriginType.Woodland);
        var ManaGainValue = 1.0f * WoodlandCount / 2 + (WoodlandCount % 2 == 0 ? 0 : 0.5f);
        m_progessMana = Mathf.Max(1, (int)ManaGainValue);

        foreach (var CardCheck in m_cardQueue)
        {
            if (CardCheck.Origin != CardOriginType.Woodland)
                continue;
            CardCheck.DoChoiceReady();
        }

        m_progessCard.Add(Card);
        GameManager.instance.CardOriginWoodlandReady(Card);
    } //Origin Woodland Event

    public void DoOriginWoodlandStart(ICard CardChoice)
    {
        m_progessMana -= 1;

        if (m_progessMana > 0)
        {
            foreach (var Card in m_cardQueue)
                Card.DoChoiceUnReady();
            CardChoice.DoChoiceOnce(true);
        }
        else
        {
            foreach (var Card in m_cardQueue)
            {
                Card.DoChoiceUnReady();
                Card.DoChoiceOnce(false);
            }
        }

        CardChoice.DoEffectAlpha(() => CardChoice.DoManaFill(1, () =>
        {
            if (m_progessMana > 0)
            {
                foreach (var CardCheck in m_cardQueue)
                {
                    if (CardCheck.Origin != CardOriginType.Woodland)
                        continue;
                    CardCheck.DoChoiceReady();
                }
                GameManager.instance.CardOriginWoodlandReady(m_progessCard[m_progessCard.Count - 1]);
            }
            else
            {
                var Progess = m_progessCard[m_progessCard.Count - 1];
                m_progessCard.RemoveAt(m_progessCard.Count - 1);
                Progess.DoCollectProgess();
            }
        }));
    }

    public void DoOriginGhostReady(ICard Card)
    {
        var GhostCount = m_cardQueue.ToArray().Count(t => t.Origin == CardOriginType.Ghost);
        var StaffCurrent = m_staffStep;
        var StaffAvaible = GhostCount;
        while (StaffAvaible > 0)
        {
            StaffCurrent++;
            if (StaffCurrent > m_cardQueue.Count - 1)
                StaffCurrent = 0;
            m_cardQueue[StaffCurrent].DoChoiceReady();
            StaffAvaible--;
        }

        m_progessCard.Add(Card);
        GameManager.instance.CardOriginGhostReady(Card);
    } //Origin Ghost Event

    public void DoOriginGhostStart(ICard CardChoice)
    {
        foreach (var Card in m_cardQueue)
            Card.DoChoiceUnReady();
        CardChoice.DoEffectAlpha(() => DoOriginGhostProgess(CardChoice, () =>
        {
            var Progess = m_progessCard[m_progessCard.Count - 1];
            m_progessCard.RemoveAt(m_progessCard.Count - 1);
            Progess.DoCollectProgess();
        }));
    }

    private void DoOriginGhostProgess(ICard CardChoice, Action OnComplete)
    {
        DoStaffNext(() =>
        {
            if (CardStaffCurrent.Equals(CardChoice))
                //Active staff when land on card choosed
                DoStaffActive(OnComplete);
            else
                //Continue move staff if not land on chossed card
                DoOriginGhostProgess(CardChoice, OnComplete);
        });
    }

    public void DoOriginInsect(ICard Card, Action OnComplete) { } //Origin Insect Event

    public void DoOriginSiren(ICard Card, Action OnComplete) { } //Origin Siren Event

    public void DoOriginNeutral(ICard Card, Action OnComplete)
    {
        OnComplete?.Invoke();
    } //Origin Neutral Event


    public void DoStaffNext(bool Active)
    {
        DoBoardReRange();

        //Start Move staff to Point
        var StaffIndexLast = m_staffStep;
        var StaffIndexNext = StaffIndexLast + 1 > m_cardQueue.Count - 1 ? 0 : StaffIndexLast + 1;
        m_staffStep = StaffIndexNext;

        //Update staff Parent to Last
        m_staff.Pointer = PointerLast;
        m_staff.Centre = m_cardContent.GetChild(m_staffStep).GetComponent<RectTransform>();
        m_staff.DoMoveNextJump(() =>
        {
            if (Active)
                DoStaffActive(() => CardManaCheckEnd());
            else
                DoEnd(() => GameManager.instance.PlayerEnd(this));
        });
    }

    public void DoStaffNext(Action OnComplete)
    {
        DoBoardReRange();

        //Start Move staff to Point
        var StaffIndexLast = m_staffStep;
        var StaffIndexNext = StaffIndexLast + 1 > m_cardQueue.Count - 1 ? 0 : StaffIndexLast + 1;
        m_staffStep = StaffIndexNext;

        //Update staff Parent to Last
        m_staff.Pointer = PointerLast;
        m_staff.Centre = m_cardContent.GetChild(m_staffStep).GetComponent<RectTransform>();
        m_staff.DoMoveNextJump(OnComplete);
    }

    public void DoStaffActive(Action OnComplete)
    {
        CardStaffCurrent.DostaffActive(OnComplete);
    }

    public void DoStaffRumble(Action OnComplete)
    {
        m_staff.DoRumble(OnComplete);
    }


    public void DoClassFighter(ICard Card, Action OnComplete)
    {
        DoClassFighterProgess(Card, Card.AttackCombine, 0, OnComplete);
    } //Class Fighter Event

    private void DoClassFighterProgess(ICard Card, int AttackCombineLeft, int DiceDotSumRolled, Action OnComplete)
    {
        var DiceQueue = GameManager.instance.DiceConfig.Data;
        var DiceCount = DiceQueue.Count;
        var DiceIndex = UnityEngine.Random.Range(0, (DiceCount * 10) - 1) / 10;
        var DiceFace = DiceQueue[DiceIndex];

        AttackCombineLeft--;
        DiceDotSumRolled += DiceFace.Dot;

        GameEvent.ClassFighter(() =>
        {
            if (AttackCombineLeft > 0)
                DoClassFighterProgess(Card, AttackCombineLeft, DiceDotSumRolled, OnComplete);
            else
            {
                var DiceDotSumAttack = (int)(DiceDotSumRolled / 2);
                Card.DoRumble(() =>
                {
                    var OnCompleteEvent = false;
                    var PlayerQueue = GameManager.instance.PlayerQueue;
                    for (int i = 0; i < PlayerQueue.Length; i++)
                    {
                        if (PlayerQueue[i].Equals(this))
                            continue;

                        if (!OnCompleteEvent)
                        {
                            OnCompleteEvent = true;
                            PlayerQueue[i].HealthChange(-DiceDotSumAttack, OnComplete);
                        }
                        else
                            PlayerQueue[i].HealthChange(-DiceDotSumAttack, null);
                    }
                });
            }
        });

    }

    public void DoClassMagicAddictReady(ICard Card, Action OnComplete)
    {
        bool CardGotMana = false;
        for (int i = 0; i < m_cardQueue.Count; i++)
        {
            if (m_cardQueue[i].Equals(Card) || m_cardQueue[i].ManaCurrent == 0)
                continue;
            CardGotMana = true;
            m_cardQueue[i].DoChoiceReady();
        }
        if (CardGotMana)
        {
            //Found monster(s) got mana for this monster cast spell once more time
            m_progessCard.Add(Card);
            GameManager.instance.CardClassMagicAddictReady(Card);
        }
        else
            //If no monster got mana for this monster cast spell once more time, skip this
            OnComplete?.Invoke();
    } //Class Magic Addict Event

    public void DoClassMagicAddictStart(ICard CardChoice)
    {
        foreach (var Card in m_cardQueue)
            Card.DoChoiceUnReady();

        CardChoice.DoEffectAlpha(() => CardChoice.DoManaFill(-1, () =>
        {
            m_progessCard[m_progessCard.Count - 1].DoSpellActive(() =>
            {
                var Progess = m_progessCard[m_progessCard.Count - 1];
                m_progessCard.RemoveAt(m_progessCard.Count - 1);
                Progess.DoManaProgess();
            });
        }));
    }

    public void DoClassSinger(ICard Card, Action OnComplete)
    {
        var SingerCount = m_cardQueue.ToArray().Count(t => t.Class == CardClassType.Singer);
        Card.DoRumble(() =>
        {
            var OnCompleteEvent = false;
            var PlayerQueue = GameManager.instance.PlayerQueue;
            for (int i = 0; i < PlayerQueue.Length; i++)
            {
                if (PlayerQueue[i].Equals(this))
                    continue;

                if (!OnCompleteEvent)
                {
                    OnCompleteEvent = true;
                    PlayerQueue[i].HealthChange(-SingerCount, OnComplete);
                }
                else
                    PlayerQueue[i].HealthChange(-SingerCount, null);
            }
        });
    } //Class Singer Event

    public void DoClassCareTaker(ICard Card, Action OnComplete) { } //Class Care Taker Event

    public void DoClassDiffuser(ICard Card, Action OnComplete)
    {
        var CardCurrentIndex = m_cardQueue.ToList().IndexOf(Card);
        var CardL = CardCurrentIndex - 1 >= 0 ? m_cardQueue[CardCurrentIndex - 1] : null;
        var CardR = CardCurrentIndex + 1 <= m_cardQueue.Count - 1 ? m_cardQueue[CardCurrentIndex + 1] : null;

        Card.DoRumble(() =>
        {
            Card.DoEffectOutlineMana(() =>
            {
                Card.DoEffectOutlineNormal(OnComplete);
                if (CardL != null)
                    CardL.DoManaFill(1, null);
                if (CardR != null)
                    CardR.DoManaFill(1, null);
            });
        });
    } //Class Diffuser Event

    public void DoClassFlyingReady(ICard Card)
    {
        var CardIndex = m_cardQueue.ToList().IndexOf(Card);
        for (int i = 0; i < m_cardQueue.Count; i++)
        {
            if (Mathf.Abs(CardIndex - i) <= 1)
                continue;
            m_cardQueue[i].DoChoiceReady();
        }

        m_progessCard.Add(Card);
        GameManager.instance.CardClassFlyingReady(Card);
    } //Class Flying Event

    public void DoClassFlyingStart(ICard CardChoice)
    {
        foreach (var Card in m_cardQueue)
            Card.DoChoiceUnReady();
        CardChoice.DoEffectAlpha(() =>
        {
            var CardFromIndex = m_cardQueue.ToList().IndexOf(m_progessCard[m_progessCard.Count - 1]);
            var CardToIndex = m_cardQueue.ToList().IndexOf(CardChoice);
            var MoveDirection = CardFromIndex < CardToIndex ? 1 : -1;
            DoClassFlyingProgess(CardFromIndex, CardToIndex + (MoveDirection * -1), () =>
            {
                m_progessCard[m_progessCard.Count - 1].DoRumble(() => CardChoice.DoManaFill(1, () =>
                {
                    var Progess = m_progessCard[m_progessCard.Count - 1];
                    m_progessCard.RemoveAt(m_progessCard.Count - 1);
                    Progess.DoManaProgess();
                }));
            });
        });
    }

    private void DoClassFlyingProgess(int IndexFrom, int IndexTo, Action OnComplete)
    {
        var CardFrom = m_cardQueue[IndexFrom];
        var MoveDirection = IndexFrom < IndexTo ? 1 : -1;

        bool StaffMoved = false;

        switch (MoveDirection)
        {
            case -1:
                for (int i = IndexFrom - 1; i >= IndexTo; i--)
                {
                    var CentreLinear = m_cardContent.transform.GetChild(i + 1).GetComponent<RectTransform>();
                    m_cardQueue[i].DoMoveCentreLinear(CentreLinear, null);
                    m_cardQueue[i + 1] = m_cardQueue[i];
                    if (!StaffMoved && i == StaffStep)
                    {
                        StaffMoved = true;
                        m_staffStep = i + 1;
                        m_staff.Pointer = PointerLast;
                        m_staff.Centre = m_cardContent.GetChild(m_staffStep).GetComponent<RectTransform>();
                        m_staff.DoMoveCentreLinear(null);
                    }
                }
                break;
            case 1:
                for (int i = IndexFrom + 1; i <= IndexTo; i++)
                {
                    var CentreLinear = m_cardContent.transform.GetChild(i - 1).GetComponent<RectTransform>();
                    m_cardQueue[i].DoMoveCentreLinear(CentreLinear, null);
                    m_cardQueue[i - 1] = m_cardQueue[i];
                    if (!StaffMoved && i == StaffStep)
                    {
                        StaffMoved = true;
                        m_staffStep = i - 1;
                        m_staff.Pointer = PointerLast;
                        m_staff.Centre = m_cardContent.GetChild(m_staffStep).GetComponent<RectTransform>();
                        m_staff.DoMoveCentreLinear(null);
                    }
                }
                break;
        }

        var CentreJump = m_cardContent.transform.GetChild(IndexTo).GetComponent<RectTransform>();
        CardFrom.DoMoveCentreJump(CentreJump, () =>
        {
            DoBoardReRange();
            OnComplete?.Invoke();
        });
        if (!StaffMoved && IndexFrom == StaffStep)
        {
            m_staffStep = IndexTo;
            m_staff.Pointer = PointerLast;
            m_staff.Centre = m_cardContent.GetChild(m_staffStep).GetComponent<RectTransform>();
            m_staff.DoMoveCentreJump(null);
        }
        m_cardQueue[IndexTo] = CardFrom;
    }


    public void DoCardManaFillReady(ICard Card, int Mana)
    {
        m_progessMana = Mana;
        m_progessOrigin = CardOriginType.None;

        foreach (var CardCheck in m_cardQueue)
        {
            if (CardCheck.Type != CardType.Mons)
                continue;
            CardCheck.DoChoiceReady();
        }

        m_progessCard.Add(Card);
        GameManager.instance.CardManaFillReady(Card);
    } //Mana Fill Event

    public void DoCardManaFillReady(ICard Card, CardOriginType Origin, int Mana)
    {
        m_progessMana = Mana;
        m_progessOrigin = Origin;

        foreach (var CardCheck in m_cardQueue)
        {
            if (CardCheck.Type != CardType.Mons || CardCheck.Origin != Origin)
                continue;
            CardCheck.DoChoiceReady();
        }

        m_progessCard.Add(Card);
        GameManager.instance.CardManaFillReady(Card);
    }

    public void DoCardManaFillStart(ICard CardChoice)
    {
        CardChoice.DoManaFill(1, () =>
        {
            if (m_progessMana > 0)
            {
                foreach (var CardCheck in m_cardQueue)
                {
                    if (CardCheck.Type != CardType.Mons || (m_progessOrigin != CardOriginType.None && CardCheck.Origin != m_progessOrigin))
                        continue;
                    CardCheck.DoChoiceReady();
                }
                GameManager.instance.CardManaFillReady(m_progessCard[m_progessCard.Count - 1]);
            }
            else
            {
                var Progess = m_progessCard[m_progessCard.Count - 1];
                m_progessCard.RemoveAt(m_progessCard.Count - 1);
                Progess.DoManaProgess();
            }
        });
    }


    public void CardManaCheckEnd()
    {
        if (m_progessCard.Count > 0)
        {
            var Progess = m_progessCard[m_progessCard.Count - 1];
            m_progessCard.RemoveAt(m_progessCard.Count - 1);
            Progess.DoCollectProgess();
            Progess.DoManaProgess();
        }
        else
        if (m_cardQueue.Exists(t => t.ManaFull))
            GameManager.instance.PlayerDoCardManaActiveReady(this);
        else
            DoEnd(() => GameManager.instance.PlayerEnd(this));
    }


    public void DoEnd(Action OnComplete)
    {
        m_stunCurrent = 0;
        GameEvent.PlayerEnd(this, OnComplete);
    }


    public void RuneStoneChange(int Value, Action OnComplete)
    {
        if (Value == 0)
        {
            OnComplete?.Invoke();
            return;
        }
        m_runeStone += Value;
        InfoRuneStoneUpdate(OnComplete);
    }

    public void StunChange(int Value, Action OnComplete)
    {
        if (Value == 0)
        {
            OnComplete?.Invoke();
            return;
        }
        m_stunCurrent += Value;
        m_stunCurrent = Mathf.Clamp(m_stunCurrent, 0, m_stunPoint);
        InfoStunUpdate(OnComplete);
    }

    public void HealthChange(int Value, Action OnComplete)
    {
        if (Value == 0)
        {
            OnComplete?.Invoke();
            return;
        }
        m_healthCurrent += Value;
        InfoHealthUpdate(OnComplete);
    }
}