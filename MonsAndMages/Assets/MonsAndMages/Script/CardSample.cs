using UnityEngine;

public class CardSample : MonoBehaviour, ICardImformation, ICardStep
{
    [SerializeField] private CardData Data;

    private int m_energyCurrent;
    private int m_growCurrent;

    //ICardImformation

    public CardNameType CardName => Data.Name;

    public CardOriginType CardOrigin => Data.Origin;

    public CardClassType CardClass => Data.Class;

    public int CardCost => Data.RuneStoneCost;

    public int CardEnergy => Data.Energy;

    public int CardEnergyCurrent => m_energyCurrent;

    public int CardAttack => Data.Attack;

    public int CardAttackCombine => Data.Attack + m_growCurrent;

    public void CardInit(CardData data)
    {
        Data = data;
    }

    //ICardAbility

    public bool QuickAbilityActive() { return false; }

    public bool WandActive() { return false; }

    public bool OriginActive() { return false; }

    public bool AttackActive() { return false; }

    public bool EnergyFill(int Value = 1) { return false; }

    public bool ClassActive() { return false; }

    public bool SkillActive() { return false; }
}