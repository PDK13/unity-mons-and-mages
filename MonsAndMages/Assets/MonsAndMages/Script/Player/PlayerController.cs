using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

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
    [SerializeField] private RectTransform m_wand;

    private bool m_choice = false;
    private bool m_turn = false;

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
            m_tmpRuneStone.text = GameManager.instance.PlayerCurrent.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
            m_runeStoneBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
        });
    }

    private void InfoStunUpdate(Action OnComplete)
    {
        m_stunBox.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            m_tmpStun.text = GameManager.instance.PlayerCurrent.StunCurrent.ToString();
            m_stunBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
        });
    }

    private void InfoHealthUpdate(Action OnComplete)
    {
        m_healthBox.DOScale(Vector2.one * 1.2f, 0.1f).OnComplete(() =>
        {
            m_tmpHealth.text = GameManager.instance.PlayerCurrent.HealthCurrent.ToString();
            m_healthBox.DOScale(Vector2.one, 0.1f).OnComplete(() => OnComplete?.Invoke());
        });
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

    public List<ICard> CardQueue => m_data.CardQueue;

    public int WandStep => m_data.WandStep;

    public int[] Mediation => m_data.Mediation;

    public bool MediationEmty => m_data.MediationEmty;

    public PlayerController Controller => this;


    public void Init(PlayerData Data)
    {
        m_data = Data;
        m_data.Player = this;
        //
        m_playerName.text = "P" + Index.ToString();
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
        RuneStoneIconTween.Append(RuneStone.DOAnchorPos(m_runeStoneBox.anchoredPosition, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            m_runeStoneBox.GetComponentInChildren<TextMeshProUGUI>().text = this.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
            Destroy(RuneStone.gameObject, 0.2f);
            OnComplete?.Invoke();
        }).SetDelay(0.25f));
        RuneStoneIconTween.Play();
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
                        RuneStoneIconTween.Append(RuneStone.DOAnchorPos(m_runeStoneBox.anchoredPosition, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
                        {
                            m_runeStoneBox.GetComponentInChildren<TextMeshProUGUI>().text = this.RuneStone.ToString() + GameConstant.TMP_ICON_RUNE_STONE;
                            Destroy(RuneStone.gameObject, 0.2f);
                            OnComplete?.Invoke();
                        }).SetDelay(0.25f));
                        RuneStoneIconTween.Play();
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
        InfoStunUpdate(() => OnComplete?.Invoke(m_data.Stuned));
        GameEvent.PlayerStunnedCheck(this, null);
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
    }

    public Transform DoCollectReady()
    {
        if (m_data.CardQueue.Count >= 5 && CardQueue[0] == null)
        {
            //Wand still stay at emty position after remove stage card
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
        m_choice = false;
        RuneStoneChange(-Card.RuneStoneCost, () =>
        {
            Card.DoCollectActive(this, () =>
            {
                GameEvent.PlayerDoCollect(this, Card, () => OnComplete?.Invoke());
            });
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
        InfoRuneStoneUpdate(() => OnComplete?.Invoke());
        GameEvent.PlayerRuneStoneChange(this, Value, null);
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
        GameEvent.PlayerStunnedChange(this, Value, null);
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
        GameEvent.PlayerHealthChange(this, Value, null);
    }
}