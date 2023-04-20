using System;
using System.Threading.Tasks;

public abstract class EffectRule<TContext>
{
    public abstract void Execute(TContext context);
    public abstract Task ExecuteAsync(TContext context);

    // Builder

    public EffectRule<TContextIn> WithSelect<TContextIn>(Func<TContextIn, TContext> selection)
    {
        return new SelectEffectRule<TContextIn, TContext>(selection, this);
    }
}