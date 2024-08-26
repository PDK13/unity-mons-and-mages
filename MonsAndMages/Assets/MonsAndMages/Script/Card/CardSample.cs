using UnityEngine;

public class CardSample : MonoBehaviour, ICardImformation, ICardStep
{
    [SerializeField] private string m_name;
    [SerializeField] private CardOriginType m_origin;
    [SerializeField] private CardClassType m_class;
    [SerializeField][Min(0)] private int m_cost = 0;
    [SerializeField][Min(0)] private int m_energy = 0;
    [SerializeField][Min(0)] private int m_attack = 0;

    private int m_energyCurrent;
    private int m_growCurrent;

    //ICardImformation

    public string CardName => m_name;

    public CardOriginType CardOrigin => m_origin;

    public CardClassType CardClass => m_class;

    public int CardCost => m_cost;

    public int CardEnergy => m_energy;

    public int CardEnergyCurrent => m_energyCurrent;

    public int CardAttack => m_attack;

    public int CardAttackCombine => m_attack + m_growCurrent;

    //ICardAbility

    public bool QuickAbilityActive() { return false; }

    public bool WandActive() { return false; }

    public bool OriginActive() { return false; }

    public bool AttackActive() { return false; }

    public bool EnergyFill(int Value = 1) { return false; }

    public bool ClassActive() { return false; }

    public bool SkillActive() { return false; }
}