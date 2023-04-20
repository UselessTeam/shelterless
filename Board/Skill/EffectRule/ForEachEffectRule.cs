using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ForEachEffectRule<TContextIn, TContextOut> : EffectRule<TContextIn>
{
    protected Func<TContextIn, IEnumerable<TContextOut>> Selection;
    protected EffectRule<TContextOut> Effect;

    public ForEachEffectRule(Func<TContextIn, IEnumerable<TContextOut>> selection, EffectRule<TContextOut> effect)
    {
        Selection = selection;
        Effect = effect;
    }

    public override void Execute(TContextIn context)
    {
        foreach (TContextOut subContext in Selection(context))
        {
            Effect.Execute(subContext);
        }
    }

    public override async Task ExecuteAsync(TContextIn context)
    {
        foreach (TContextOut subContext in Selection(context))
        {
            await Effect.ExecuteAsync(subContext);
        }
    }
}