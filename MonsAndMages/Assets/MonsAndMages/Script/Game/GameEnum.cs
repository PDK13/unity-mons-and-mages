public enum CardType
{
    None,
    Mons,
    Landmark,
}

public enum CardOriginType
{
    None,
    Dragon,
    Woodland,
    Ghost,
    Insects,
    Siren,
    Neutral,
}

public enum CardClassType
{
    None,
    Fighter,
    MagicAddict,
    Singer,
    Caretaker,
    Diffuser,
    Flying,
}

public enum CardNameType
{
    None = 0,
    //
    Cornibus = 2,
    Duchess = 3,
    DragonEgg = 4,
    Eversor = 5,
    FlowOfTheEssential = 6,
    Forestwing = 7,
    PixieSGrove = 8,
    OneTail = 9,
    Pott = 10,
    Umbella = 11,
    //
    Mediate = int.MaxValue - 1,
    Stage = int.MaxValue,
}

public enum CardTrickType
{

}

public enum CardEffectType
{
    None,
    Alpha,
}

public enum ViewType
{
    None,
    Field,
    Wild,
}

public enum ChoiceType
{
    None,
    MediateOrCollect,
    CardFullMana,
    CardOriginWoodland,
    CardOriginGhost,
    CardClassMagicAddict,
    CardClassFlying,
    CardSpell,
    CardEnter,
}

public enum ProgessCollectType
{
    None,
    Start,
    Origin,
    Enter,
    End,
}

public enum ProgessManaType
{
    None,
    Start,
    Class,
    Spell,
    End,
}