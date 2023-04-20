using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public struct AnimationContext
{
    public AnimationComponent Component;
    public string Name;
    public Sign Side;
}

public class AnimationEffectRule<TContext> : MultiEffectRule<TContext>
{
    protected Func<TContext, AnimationContext> Selection;

    public AnimationEffectRule(Func<TContext, AnimationContext> selection = null)
    {
        Selection = selection;
    }
    public AnimationEffectRule(AnimationEffectRule<TContext> other) : base(other)
    {
        Selection = other.Selection;
    }

    public override async Task ExecuteAsync(TContext context)
    {
        AnimationContext animationContext = Selection(context);
        await animationContext.Component.Play(
            animationContext.Name,
            animationContext.Side,
            TaskSequence.Create(context, SubEffects)
        );
    }

    public override AnimationEffectRule<TContext> Copy()
    {
        return new AnimationEffectRule<TContext>(this);
    }
}