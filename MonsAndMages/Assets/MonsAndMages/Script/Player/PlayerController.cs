using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPlayer
{
    private PlayerData m_data;

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
    [SerializeField] private RectTransform m_staff;

    private ICard m_cardManaActiveCurrent = null;
    private RectTransform m_staffCentre;

    private RectTransform Pointer => m_cardContent.GetChild(m_cardContent.childCount - 1).GetComponent<RectTransform>();

    //

    private IEnumerator Start()
    {
        m_cardPoint.SetActive(false);
        m_runeStoneIcon.SetActive(false);

        yield return null;

        DoCardPointerReRange();
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
        m_cardMediation[Index].InfoRuneStoneUpdate(m_data.Mediation[Index], OnComplete);
    }

    //IPlayer

    public int Index => m_data.Index;

    public bool Base => m_data.Base;

    public int HealthPoint => m_data.HealthPoint;

    public int HealthCurrent => m_data.HealthCurrent;

    public int RuneStone => m_data.RuneStone;

    public int StunPoint => m_data.StunPoint;

    public int StunCurrent => m_data.StunCurrent;

    public bool Stuned => m_data.Stuned;

    public int[] Mediation => m_data.Mediation;

    public bool MediationEmty => m_data.MediationEmty;

    public ICard[] CardQueue => m_data.CardQueue.ToArray();

    public int StaffStep => m_data.StaffStep;

    public ICard CardStaffCurrent => CardQueue[StaffStep];

    public ICard CardManaActiveCurrent => m_cardManaActiveCurrent;


    public void Init(PlayerData Data)
    {
        m_data = Data;
        m_data.Player = this;

        m_playerName.text = "P" + (Index + 1).ToString();

        InfoRuneStoneUpdate(null);
        InfoStunUpdate(null);
        InfoHealthUpdate(null);
        InfoMediationUpdate(0, null);
        InfoMediationUpdate(1, null);

        for (int i = 0; i < m_cardContent.childCount; i++)
            m_data.CardQueue.Add(m_cardContent.GetChild(i).GetComponentInChildren<ICard>());

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

        m_data.RuneStone += Value;

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
        for (int i = 0; i < m_data.Mediation.Length; i++)
        {
            if (m_data.Mediation[i] > 0)
            {
                RuneStoneTake = true;

                m_data.Mediation[i] -= 2;
                m_data.RuneStone += 2;

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
        InfoStunUpdate(() => OnComplete?.Invoke(m_data.Stuned));
        GameEvent.PlayerStunnedCheck(this, null);
    }

    public void DoMediate(int RuneStoneAdd, Action OnComplete)
    {
        m_data.RuneStone -= RuneStoneAdd;
        InfoRuneStoneUpdate(() =>
        {
            if (m_data.Mediation[0] == 0)
            {
                m_data.Mediation[0] = RuneStoneAdd * 2;
                InfoMediationUpdate(0, OnComplete);
            }
            else
            if (m_data.Mediation[1] == 0)
            {
                m_data.Mediation[1] = RuneStoneAdd * 2;
                InfoMediationUpdate(1, OnComplete);
            }
            else
                OnComplete?.Invoke();
        });
    }


    public (RectTransform Pointer, RectTransform Centre) DoCollectReady()
    {
        if (m_data.CardQueue.Count >= 5 && CardQueue[0] == null)
        {
            //staff still stay at emty position after remove stage card
            //Destroy(m_cardContent.GetChild(0).gameObject);
            //m_data.CardQueue.RemoveAt(0);
        }

        var CardPoint = Instantiate(m_cardPoint, m_cardContent);
        CardPoint.SetActive(true);
        CardPoint.name = "card-point";

        return (CardPoint.GetComponent<RectTransform>(), CardPoint.GetComponent<RectTransform>());
    }

    public void DoCollect(ICard Card, Action OnComplete)
    {
        RuneStoneChange(-Card.RuneStoneCost, () => OnComplete?.Invoke());
        m_data.CardQueue.Add(Card);
    }

    public void DoCardPointerReRange()
    {
        //Card
        for (int i = 0; i < CardQueue.Length; i++)
        {
            var Centre = m_cardContent.GetChild(i).GetComponent<RectTransform>();
            CardQueue[i].Pointer(Pointer, Centre, true, true);
        }
        //Staff
        m_staff.SetParent(Pointer, true);
        m_staff.SetSiblingIndex(Pointer.childCount - 1);
        m_staffCentre = m_cardContent.GetChild(m_data.StaffStep).GetComponent<RectTransform>();
    }


    public void DoStaffNext(Action OnComplete)
    {
        DoCardPointerReRange();

        //Start Move staff to Point
        var StaffIndexLast = m_data.StaffStep;
        var StaffIndexNext = StaffIndexLast + 1 > CardQueue.Length - 1 ? 0 : StaffIndexLast + 1;
        m_data.StaffStep = StaffIndexNext;

        //Update staff Parent to Last
        m_staff.SetParent(m_cardContent.GetChild(m_cardContent.childCount - 1), true);
        m_staffCentre = m_cardContent.GetChild(StaffIndexNext).GetComponent<RectTransform>();

        var CentreInPointer = Pointer.InverseTransformPoint(m_staffCentre.position);

        m_staff
            .DOLocalJump(CentreInPointer, 45f, 1, 1f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => OnComplete?.Invoke());
    }

    private void DoStaffMoveCentreLinear(RectTransform Centre, Action OnComplete)
    {
        var CentreInPointer = Pointer.InverseTransformPoint(Centre.position);
        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        m_staff
            .DOLocalMove(CentreInPointer, MoveDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
    }

    private void DoStaffMoveCentreJump(RectTransform Centre, Action OnComplete)
    {
        var CentreInPointer = Pointer.InverseTransformPoint(Centre.position);
        var MoveDuration = GameManager.instance.TweenConfig.CardAction.MoveDuration;

        m_staff.SetSiblingIndex(Pointer.childCount - 1);

        Sequence CardTween = DOTween.Sequence();
        CardTween.Insert(0f, m_staff.DOScale(Vector3.one * 1.35f, MoveDuration * 0.5f).SetEase(Ease.OutQuad));
        CardTween.Insert(0f, m_staff.DOLocalJump(CentreInPointer, 50, 1, MoveDuration).SetEase(Ease.Linear));
        CardTween.Insert(MoveDuration * 0.5f, m_staff.DOScale(Vector3.one, MoveDuration * 0.5f).SetEase(Ease.InCirc));
        CardTween.OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
        CardTween.Play();
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


    public void DoEnd(Action OnComplete)
    {
        m_data.StunCurrent = 0;
        GameEvent.PlayerEnd(this, OnComplete);
    }


    public void RuneStoneChange(int Value, Action OnComplete)
    {
        if (Value == 0)
        {
            OnComplete?.Invoke();
            return;
        }
        m_data.RuneStone += Value;
        InfoRuneStoneUpdate(OnComplete);
    }

    public void StunChange(int Value, Action OnComplete)
    {
        if (Value == 0)
        {
            OnComplete?.Invoke();
            return;
        }
        m_data.StunCurrent += Value;
        m_data.StunCurrent = Mathf.Clamp(m_data.StunCurrent, 0, m_data.StunPoint);
        InfoStunUpdate(OnComplete);
    }

    public void HealthChange(int Value, Action OnComplete)
    {
        if (Value == 0)
        {
            OnComplete?.Invoke();
            return;
        }
        m_data.HealthCurrent += Value;
        InfoHealthUpdate(OnComplete);
    }


    public void DoCardSpecialActiveCurrent(ICard Card)
    {
        m_cardManaActiveCurrent = Card;
    }


    public void DoCardSwap(int IndexFrom, int IndexTo, Action OnComplete)
    {
        var CardFrom = CardQueue[IndexFrom];
        var MoveDirection = IndexFrom < IndexTo ? 1 : -1;

        switch (MoveDirection)
        {
            case -1:
                for (int i = IndexFrom - 1; i >= IndexTo; i--)
                {
                    var CentreLinear = m_cardContent.transform.GetChild(i + 1).GetComponent<RectTransform>();
                    m_data.CardQueue[i].MoveCentreLinear(CentreLinear, null);
                    m_data.CardQueue[i + 1] = CardQueue[i];
                    if (i == StaffStep)
                        DoStaffMoveCentreLinear(CentreLinear, null);
                }
                break;
            case 1:
                for (int i = IndexFrom + 1; i <= IndexTo; i++)
                {
                    var CentreLinear = m_cardContent.transform.GetChild(i - 1).GetComponent<RectTransform>();
                    m_data.CardQueue[i].MoveCentreLinear(CentreLinear, null);
                    m_data.CardQueue[i - 1] = CardQueue[i];
                    if (i == StaffStep)
                        DoStaffMoveCentreLinear(CentreLinear, null);
                }
                break;
        }

        var CentreJump = m_cardContent.transform.GetChild(IndexTo).GetComponent<RectTransform>();
        CardFrom.MoveCentreJump(CentreJump, () =>
        {
            DoCardPointerReRange();
            OnComplete?.Invoke();
        });
        if (IndexFrom == StaffStep)
            DoStaffMoveCentreJump(CentreJump, null);
        m_data.CardQueue[IndexTo] = CardFrom;
    }
}