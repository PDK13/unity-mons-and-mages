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
    [SerializeField] private RectTransform m_staffMoveTo;

    private void Start()
    {
        m_cardPoint.SetActive(false);
        m_runeStoneIcon.SetActive(false);
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
        m_cardMediation[Index].InfoRuneStoneUpdate(m_data.Mediation[Index], () => OnComplete?.Invoke());
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

    public ICard[] CardQueue => m_data.CardQueue.ToArray();

    public int StaffStep => m_data.StaffStep;

    public ICard CardCurrent => CardQueue[StaffStep];

    public int[] Mediation => m_data.Mediation;

    public bool MediationEmty => m_data.MediationEmty;

    public PlayerController Controller => this;


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
        GameEvent.PlayerStart(this, () => OnComplete?.Invoke());
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


    public void DoChoice(Action OnComplete)
    {
        GameEvent.PlayerDoChoice(this, () =>
        {
            OnComplete?.Invoke();
        });
    }


    public void DoMediate(int RuneStoneAdd, Action OnComplete)
    {
        m_data.RuneStone -= RuneStoneAdd;
        InfoRuneStoneUpdate(() =>
        {
            if (m_data.Mediation[0] == 0)
            {
                m_data.Mediation[0] = RuneStoneAdd * 2;
                InfoMediationUpdate(0, () => OnComplete?.Invoke());
            }
            else
            if (m_data.Mediation[1] == 0)
            {
                m_data.Mediation[1] = RuneStoneAdd * 2;
                InfoMediationUpdate(1, () => OnComplete?.Invoke());
            }
            else
                OnComplete?.Invoke();
        });
    }


    public Transform DoCollectReady()
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

        return CardPoint.transform;
    }

    public void DoCollect(ICard Card, Action OnComplete)
    {
        RuneStoneChange(-Card.RuneStoneCost, () =>
        {
            GameEvent.PlayerDoCollect(this, Card, () =>
            {
                Card.DoCollectActive(this, () =>
                {
                    OnComplete?.Invoke();
                });
            }); //Move card first before active card collect event!
        });
        m_data.CardQueue.Add(Card);
    }


    public void DoStaffNext(Action OnComplete)
    {
        //Update staff Parent to Last
        m_staff.SetParent(m_cardContent.GetChild(m_cardContent.childCount - 1), true);
        m_staffMoveTo.SetParent(m_staff.parent);
        m_staffMoveTo.position = m_staff.position;

        //Start Move staff to Point
        var StaffIndexLast = m_data.StaffStep;
        var StaffIndexNext = StaffIndexLast + 1 > CardQueue.Length - 1 ? 0 : StaffIndexLast + 1;

        m_data.StaffStep = StaffIndexNext;

        var PointLast = m_cardContent.GetChild(StaffIndexLast);
        var PointNext = m_cardContent.GetChild(StaffIndexNext);

        m_staffMoveTo.position = PointNext.position;
        m_staff
            .DOLocalJump(m_staffMoveTo.localPosition, 45f, 1, 1f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => OnComplete?.Invoke());
    }

    public void DoStaffActive(Action OnComplete)
    {
        var Card = m_cardContent.GetChild(StaffStep).GetComponentInChildren<ICard>();

        if (Card == null)
        {
            OnComplete?.Invoke();
            return;
        }

        Card.DostaffActive(() => OnComplete?.Invoke());
    }


    public void CardManaActiveDoChoice(Action OnComplete)
    {
        GameEvent.ShowUiArea(ViewType.Field, true);
        OnComplete?.Invoke();
    }


    public void DoEnd(Action OnComplete)
    {
        m_data.StunCurrent = 0;
        GameEvent.PlayerEnd(this, () => OnComplete?.Invoke());
    }


    public void RuneStoneChange(int Value, Action OnComplete)
    {
        if (Value == 0)
        {
            OnComplete?.Invoke();
            return;
        }
        m_data.RuneStone += Value;
        InfoRuneStoneUpdate(() => OnComplete?.Invoke());
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
        InfoStunUpdate(() => OnComplete?.Invoke());
    }

    public void HealthChange(int Value, Action OnComplete)
    {
        if (Value == 0)
        {
            OnComplete?.Invoke();
            return;
        }
        m_data.HealthCurrent += Value;
        InfoHealthUpdate(() => OnComplete?.Invoke());
    }
}