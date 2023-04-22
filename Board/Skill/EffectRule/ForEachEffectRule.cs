using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ForEachEffectRule<TContextIn, TContextOut> : EffectRule<TContextIn>
{
    protected Func<TContextIn, IEnumerable<TContextOut>> Selection;
    protected EffectRule<TContextOut> Effect;
    protected bool Parallel;

    public ForEachEffectRule(Func<TContextIn, IEnumerable<TContextOut>> selection, EffectRule<TContextOut> effect, bool parallel = false)
    {
        Selection = selection;
        Effect = effect;
        Parallel = parallel;
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
        List<Task> awaiting = new List<Task>();
        foreach (TContextOut subContext in Selection(context))
        {
            Task task = Effect.ExecuteAsync(subContext);
            if (task is not null)
            {
                if (Parallel)
                {
                    awaiting.Add(task);
                }
                else
                {
                    await task;
                }
            }
        }
        if (Parallel)
        {
            foreach (Task task in awaiting)
            {
                await task;
            }
        }
    }
}