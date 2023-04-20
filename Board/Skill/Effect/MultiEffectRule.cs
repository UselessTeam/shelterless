
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class MultiEffectRule<TContext> : EffectRule<TContext>
{
    protected List<EffectRule<TContext>> SubEffects = new List<EffectRule<TContext>>();
    public MultiEffectRule(params EffectRule<TContext>[] subEffects)
    {
        SubEffects.AddRange(subEffects);
    }
    public MultiEffectRule(MultiEffectRule<TContext> other)
    {
        SubEffects.AddRange(other.SubEffects);
    }

    public override void Execute(TContext context)
    {
        foreach (EffectRule<TContext> subEffect in SubEffects)
        {
            subEffect?.Execute(context);
        }
    }

    public override async Task ExecuteAsync(TContext context)
    {
        foreach (EffectRule<TContext> subEffect in SubEffects)
        {
            await subEffect?.ExecuteAsync(context);
        }
    }

    // Builder

    public MultiEffectRule<TContext> Skip()
    {
        SubEffects.Add(null);
        return this;
    }
    public MultiEffectRule<TContext> Then(EffectRule<TContext> subEffect)
    {
        SubEffects.Add(subEffect);
        return this;
    }
    public MultiEffectRule<TContext> ThenIf(Func<TContext, bool> condition, EffectRule<TContext> subEffect)
    {
        SubEffects.Add(new ConditionalEffectRule<TContext>(condition, subEffect));
        return this;
    }

    public MultiEffectRule<TContext> Then(Action<TContext> subEffect)
    {
        SubEffects.Add(new FunctionEffectRule<TContext>(subEffect));
        return this;
    }

    public MultiEffectRule<TContext> ThenIf(Func<TContext, bool> condition, Action<TContext> subEffect)
    {
        SubEffects.Add(new ConditionalEffectRule<TContext>(condition, new FunctionEffectRule<TContext>(subEffect)));
        return this;
    }

    public MultiEffectRule<TContext> Repeat(int repeat)
    {
        for (int i = 1; i < repeat; i++)
        {
            SubEffects.Add(SubEffects.Last());
        }
        return this;
    }

    public virtual MultiEffectRule<TContext> Copy()
    {
        return new MultiEffectRule<TContext>(this);
    }
}