using System;
using System.Threading.Tasks;

public class ConditionalEffectRule<TContext> : EffectRule<TContext>
{
    private Func<TContext, bool> Condition;
    private EffectRule<TContext> SubEffectRule;
    public ConditionalEffectRule(Func<TContext, bool> condition, EffectRule<TContext> effectRule)
    {
        Condition = condition;
        SubEffectRule = effectRule;
    }
    public override void Execute(TContext context)
    {
        if (Condition(context))
        {
            SubEffectRule.Execute(context);
        }
    }
    public override Task ExecuteAsync(TContext context)
    {
        if (Condition(context))
        {
            return SubEffectRule.ExecuteAsync(context);
        }
        else
        {
            return null;
        }
    }
}