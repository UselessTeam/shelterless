using System;
using System.Threading.Tasks;

public class SelectEffectRule<TContextIn, TContextOut> : EffectRule<TContextIn>
{
    private Func<TContextIn, TContextOut> Selection;
    private EffectRule<TContextOut> SubEffectRule;
    public SelectEffectRule(Func<TContextIn, TContextOut> selection, EffectRule<TContextOut> effectRule)
    {
        Selection = selection;
        SubEffectRule = effectRule;
    }
    public override void Execute(TContextIn context)
    {
        SubEffectRule.Execute(Selection(context));
    }
    public override Task ExecuteAsync(TContextIn context)
    {
        return SubEffectRule.ExecuteAsync(Selection(context));
    }
}