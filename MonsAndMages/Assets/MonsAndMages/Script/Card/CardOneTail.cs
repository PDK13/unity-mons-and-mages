using System;

public class CardOneTail : CardController
{
    public override void DoOriginActive(Action OnComplete)
    {
        EffectAlpha(() => Rumble(() => OnComplete?.Invoke()));
    }

    public override void DoEnterActive() { }

    public override void DoPassiveActive() { }


    public override void DoClassActive(Action OnComplete)
    {
        Rumble(() => OnComplete?.Invoke());
    }

    public override void DoSpellActive(Action OnComplete)
    {
        Rumble(() => OnComplete?.Invoke());
    }
}