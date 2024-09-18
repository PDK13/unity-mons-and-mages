using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    private PlayerData m_data;

    [SerializeField] private GameObject m_cardPointSample;
    [SerializeField] private Transform m_cardContent;

    [Space][SerializeField] private CardMediate[] m_cardMediation = new CardMediate[2];

    [Space]
    [SerializeField] private Transform m_wand;

    private bool m_choice = false;
    private bool m_turn = false;

    private void Start()
    {
        m_cardPointSample.SetActive(false);
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

    public List<ICard> CardQueue => m_data.CardQueue;

    public int WandStep => m_data.WandStep;

    public int[] Mediation => m_data.Mediation;

    public bool MediationEmty => m_data.MediationEmty;

    public PlayerController Controller => this;

    //

    public void Init(PlayerData Data)
    {
        m_data = Data;
        m_data.Player = this;
        //
        for (int i = 0; i < m_cardContent.childCount; i++)
            m_data.CardQueue.Add(m_cardContent.GetChild(i).GetComponentInChildren<ICard>());
        //
        GameManager.instance.PlayerJoin(this);
    }


    public void DoStart(Action OnComplete)
    {
        m_turn = true;
        GameEvent.PlayerStart(this, () => OnComplete?.Invoke());
    }


    public void DoTakeRuneStoneFromSupply(int Value, Action OnComplete)
    {
        if (m_data.Base)
        {
            GameEvent.PlayerTakeRuneStoneFromSupply(this, Value, () =>
            {
                RuneStoneChange(Value, () => OnComplete?.Invoke());
            });
        }
        else
            OnComplete?.Invoke();
    }

    public void DoTakeRuneStoneFromMediation(Action OnComplete)
    {
        var EventInvoke = false;
        var RuneStoneSum = 0;
        for (int i = 0; i < m_data.Mediation.Length; i++)
        {
            if (m_data.Mediation[i] > 0)
            {
                m_data.Mediation[i] -= 2;

                if (!EventInvoke)
                {
                    EventInvoke = true;
                    m_cardMediation[i].EffectAlpha(1f, () =>
                    {
                        if (m_data.Base)
                        {
                            GameEvent.PlayerTakeRuneStoneFromMediation(this, RuneStoneSum, () =>
                            {
                                RuneStoneChange(2, () => OnComplete?.Invoke());
                            });
                        }
                        else
                            OnComplete?.Invoke();
                    });
                }
                else
                    m_cardMediation[i].EffectAlpha(1f, null);

                RuneStoneSum += 2;
            }
        }
        if (!EventInvoke)
            OnComplete?.Invoke();
    }

    public void DoStunnedCheck(Action<bool> OnComplete)
    {
        if (m_data.Base)
            GameEvent.PlayerStunnedCheck(this, () => OnComplete?.Invoke(m_data.Stuned));
        else
            OnComplete?.Invoke(m_data.Stuned);
    }


    public void DoChoice(Action OnComplete)
    {
        GameEvent.PlayerDoChoice(this, () =>
        {
            m_choice = true;
            OnComplete?.Invoke();
        });
    }

    public void DoMediate(int RuneStoneAdd, Action OnComplete)
    {
        m_choice = false;
        if (m_data.Mediation[0] == 0)
        {
            m_data.Mediation[0] = RuneStoneAdd * 2;
            m_cardMediation[0].EffectAlpha(1f, () => OnComplete?.Invoke());
        }
        else
        if (m_data.Mediation[1] == 0)
        {
            m_data.Mediation[1] = RuneStoneAdd * 2;
            m_cardMediation[1].EffectAlpha(1f, () => OnComplete?.Invoke());
        }
        else
            OnComplete?.Invoke();
    }

    public Transform DoCollectReady()
    {
        if (m_data.CardQueue.Count >= 5 && CardQueue[0] == null)
        {
            //Wand still stay at emty position after remove stage card
            //Destroy(m_cardContent.GetChild(0).gameObject);
            //m_data.CardQueue.RemoveAt(0);
        }

        var CardPoint = Instantiate(m_cardPointSample, m_cardContent);
        CardPoint.SetActive(true);
        CardPoint.name = "card-point";

        return CardPoint.transform;
    }

    public void DoCollect(ICard Card, Action OnComplete)
    {
        m_choice = false;
        RuneStoneChange(-Card.RuneStoneCost, () =>
        {
            Card.DoCollectActive(this, () => GameEvent.PlayerDoCollect(this, Card, () => OnComplete?.Invoke()));
        });
        m_data.CardQueue.Add(Card);
    }


    public void DoWandNext(Action OnComplete)
    {
        var WandIndexLast = m_data.WandStep;
        var WandIndexNext = m_data.WandStepNext;

        m_data.WandStep = WandIndexNext;

        var PointLast = m_cardContent.GetChild(WandIndexLast);
        var PointNext = m_cardContent.GetChild(WandIndexNext);

        m_wand.SetParent(PointNext, this.transform);
        m_wand
            .DOLocalJump(Vector3.zero, 45f, 1, 1f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => OnComplete?.Invoke());
    }

    public void DoWandActive(Action OnComplete)
    {
        var Card = m_cardContent.GetChild(WandStep).GetComponentInChildren<ICard>();

        if (Card == null)
        {
            OnComplete?.Invoke();
            return;
        }

        Card.DoWandActive(() => OnComplete?.Invoke());
    }


    public bool DoContinueCheck()
    {
        return m_data.CardQueue.Exists(t => t.EnergyFull);
    }

    public void DoContinue(Action OnComplete)
    {
        m_turn = true;
        OnComplete?.Invoke();
    }


    public void CardEnergyActiveDoChoice(Action OnComplete)
    {
        foreach (var Card in m_data.CardQueue)
        {
            if (Card == null)
                continue;
        }

        GameEvent.PlayerCardEnergyActiveDoChoice(this, () =>
        {
            m_choice = true;
            OnComplete?.Invoke();
        });
    }


    public void DoEnd(Action OnComplete)
    {
        m_turn = false;
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
        m_data.RuneStone = Mathf.Max(0, m_data.RuneStone);
        if (m_data.Base)
            GameEvent.PlayerRuneStoneChange(this, Value, () => OnComplete?.Invoke());
        else
            OnComplete?.Invoke();
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
        GameEvent.PlayerStunnedChange(this, Value, () => OnComplete?.Invoke());
    }

    public void HealthChange(int Value, Action OnComplete)
    {
        if (Value == 0)
        {
            OnComplete?.Invoke();
            return;
        }
        m_data.HealthCurrent += Value;
        m_data.HealthCurrent = Mathf.Clamp(m_data.HealthCurrent, 0, m_data.HealthPoint);
        GameEvent.PlayerHealthChange(this, Value, () => OnComplete?.Invoke());
    }
}