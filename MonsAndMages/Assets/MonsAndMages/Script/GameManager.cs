using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CardConfig CardConfig;
}

public enum CardOriginType
{
    Dragon,
    Woodland,
    Ghost,
    Insects,
    Siren,
    Neutral,
}

public enum CardClassType
{
    Fighter,
    MagicAddict,
    Singer,
    Caretaker,
    Diffuser,
    Flying,
}

public enum CardNameType
{
    MonsterA,
    MonsterB,
    MonsterC,
    MonsterD,
}