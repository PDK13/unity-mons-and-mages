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
    private ICard m_cardActiveCurrent = null;

    //

    private IEnumerator Start()
    {
        m_cardPoint.SetActive(false);
        m_runeStoneIcon.SetActive(false);

        yield return null;
        yield return null;
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
        GameEvent.PlayerRuneStoneChange(this, 0, null);
    }

    private void InfoStunUpdate(Action OnComplete)
    {
        m_stunBox.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            m_tmpStun.text = StunCurrent.ToString();
            m_stunBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
        });
        GameEvent.PlayerStunnedChange(this, 0, null);
    }

    private void InfoHealthUpdate(Action OnComplete)
    {
        m_healthBox.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            m_tmpHealth.text = HealthCurrent.ToString();
            m_healthBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
        });
        GameEvent.PlayerHealthChange(this, 0, null);
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

    public ICard CardActiveCurrent => m_cardActiveCurrent;


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


    public void DoStart(Action OnComplete)
    {
        GameEvent.PlayerStart(this, OnComplete);
    }


    public void DoTakeRuneStoneFromSupply(int Value, Action OnComplete)
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

    public void DoTakeRuneStoneFromMediation(Action OnComplete)
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

    public void DoStunnedCheck(Action<bool> OnComplete)
    {
        InfoStunUpdate(() => OnComplete?.Invoke(Stuned));
        GameEvent.PlayerStunnedCheck(this, null);
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


    public (RectTransform Pointer, RectTransform Centre) DoCollectReady()
    {
        if (m_cardQueue.Count >= 5 && m_cardQueue[0] == null)
        {
            //staff still stay at emty position after remove stage card
            //Destroy(m_cardContent.GetChild(0).gameObject);
            //m_cardQueue.RemoveAt(0);
        }

        var CardPoint = Instantiate(m_cardPoint, m_cardContent);
        CardPoint.SetActive(true);
        CardPoint.name = "card-point";

        return (CardPoint.GetComponent<RectTransform>(), CardPoint.GetComponent<RectTransform>());
    }

    public void DoCollect(ICard Card, Action OnComplete)
    {
        RuneStoneChange(-Card.RuneStoneCost, () => OnComplete?.Invoke());
        m_cardQueue.Add(Card);
    }

    public void DoBoardReRange()
    {
        //Card
        for (int i = 0; i < m_cardQueue.Count; i++)
        {
            m_cardQueue[i].Pointer = m_cardContent.GetChild(m_cardContent.childCount - 1).GetComponent<RectTransform>();
            m_cardQueue[i].Centre = m_cardContent.GetChild(i).GetComponent<RectTransform>();
            m_cardQueue[i].DoFixed();
        }
        //Staff
        m_staff.Pointer = m_cardContent.GetChild(m_cardContent.childCount - 1).GetComponent<RectTransform>();
        m_staff.Centre = m_cardContent.GetChild(m_staffStep).GetComponent<RectTransform>();
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

    public void DoOriginWoodland(ICard Card, Action OnComplete)
    {
        var WoodlandCount = m_cardQueue.Count(t => t.Origin == CardOriginType.Woodland);
        var ManaGainValue = 1.0f * WoodlandCount / 2 + (WoodlandCount % 2 == 0 ? 0 : 0.5f);
        Card.DoManaFill((int)ManaGainValue, () => OnComplete?.Invoke());
    } //Origin Woodland Event

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
        GameManager.instance.CardOriginGhostDoChoice(Card);
    } //Origin Ghost Event

    public void DoOriginGhostStart(ICard CardChoice, Action OnComplete)
    {
        foreach (var Card in m_cardQueue)
            Card.DoChoiceUnReady();
        CardChoice.DoEffectAlpha(() => DoOriginGhostProgess(CardChoice, OnComplete));
    }

    private void DoOriginGhostProgess(ICard CardChoice, Action OnComplete)
    {
        DoStaffNext(() =>
        {
            if (CardStaffCurrent.Equals(CardChoice))
            {
                //Active staff when land on card choosed
                DoStaffActive(() =>
                {
                    //Continue resolve current card before return to main progess
                    CardStaffCurrent.DoEnterActive(() => CardStaffCurrent.DoPassiveActive(OnComplete));
                });
            }
            else
                //Continue move staff if not land on chossed card
                DoOriginGhostProgess(CardChoice, OnComplete);
        });
    }

    public void DoOriginInsect(ICard Card, Action OnComplete) { } //Origin Insect Event

    public void DoOriginSiren(ICard Card, Action OnComplete) { } //Origin Siren Event

    public void DoOriginNeutral(ICard Card, Action OnComplete) { } //Origin Neutral Event


    public void DoStaffNext(Action OnComplete)
    {
        DoBoardReRange();

        //Start Move staff to Point
        var StaffIndexLast = m_staffStep;
        var StaffIndexNext = StaffIndexLast + 1 > m_cardQueue.Count - 1 ? 0 : StaffIndexLast + 1;
        m_staffStep = StaffIndexNext;

        //Update staff Parent to Last
        m_staff.Pointer = m_cardContent.GetChild(m_cardContent.childCount - 1).GetComponent<RectTransform>();
        m_staff.Centre = m_cardContent.GetChild(m_staffStep).GetComponent<RectTransform>();
        m_staff.DoMoveNextJump(OnComplete);
    }

    public void DoStaffActive(Action OnComplete)
    {
        if (CardStaffCurrent == null)
        {
            Debug.LogError("Not found card for staff active");
            OnComplete?.Invoke();
            return;
        }
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
            if (m_cardQueue[i].Equals(this) || m_cardQueue[i].ManaCurrent == 0)
                continue;
            CardGotMana = true;
            m_cardQueue[i].DoChoiceReady();
        }
        Card.DoEffectClass(() =>
        {
            if (CardGotMana)
            {
                //Found monster(s) got mana for this monster cast spell once more time
                m_cardActiveCurrent = Card;
                GameManager.instance.CardClassMagicAddictDoChoice(Card);
            }
            else
                //If no monster got mana for this monster cast spell once more time, skip this
                OnComplete?.Invoke();
        });
    } //Class Magic Addict Event

    public void DoClassMagicAddictStart(ICard CardChoice, Action OnComplete)
    {
        var CharActive = m_cardActiveCurrent;
        m_cardActiveCurrent = null;

        foreach (var Card in m_cardQueue)
            Card.DoChoiceUnReady();

        CardChoice.DoEffectAlpha(() => CardChoice.DoManaFill(-1, () =>
            CharActive.DoSpellActive(() => CharActive.DoSpellActive(() => OnComplete?.Invoke()))));
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
        m_cardActiveCurrent = Card;

        var CardIndex = m_cardQueue.ToList().IndexOf(Card);
        for (int i = 0; i < m_cardQueue.Count; i++)
        {
            if (Mathf.Abs(CardIndex - i) <= 1)
                continue;
            m_cardQueue[i].DoChoiceReady();
        }
        GameManager.instance.CardClassFlyingDoChoice(Card);
    } //Class Flying Event

    public void DoClassFlyingStart(ICard CardChoice, Action OnComplete)
    {
        var CharActive = m_cardActiveCurrent;
        m_cardActiveCurrent = null;

        foreach (var Card in m_cardQueue)
            Card.DoChoiceUnReady();
        CardChoice.DoEffectAlpha(() =>
        {
            var CardFromIndex = m_cardQueue.ToList().IndexOf(CharActive);
            var CardToIndex = m_cardQueue.ToList().IndexOf(CardChoice);
            var MoveDirection = CardFromIndex < CardToIndex ? 1 : -1;
            DoCardSwap(CardFromIndex, CardToIndex + (MoveDirection * -1), () =>
            {
                CharActive.DoRumble(() => CardChoice.DoManaFill(1, () => OnComplete?.Invoke()));
            });
        });
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


    public void DoCardSwap(int IndexFrom, int IndexTo, Action OnComplete)
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
                        m_staff.Pointer = m_cardContent.GetChild(m_cardContent.childCount - 1).GetComponent<RectTransform>();
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
                        m_staff.Pointer = m_cardContent.GetChild(m_cardContent.childCount - 1).GetComponent<RectTransform>();
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
            m_staff.Pointer = m_cardContent.GetChild(m_cardContent.childCount - 1).GetComponent<RectTransform>();
            m_staff.Centre = m_cardContent.GetChild(m_staffStep).GetComponent<RectTransform>();
            m_staff.DoMoveCentreJump(null);
        }
        m_cardQueue[IndexTo] = CardFrom;
    }
}