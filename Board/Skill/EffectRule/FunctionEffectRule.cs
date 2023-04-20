using System;
using System.Threading.Tasks;

public abstract class SyncEffectRule<TContexT> : EffectRule<TContexT>
{
    public override Task ExecuteAsync(TContexT context)
    {
        Execute(context);
        return null;
    }
}

public class FunctionEffectRule<TContext> : SyncEffectRule<TContext>
{
    private Action<TContext> RunFunction;
    public FunctionEffectRule(Action<TContext> runFunction)
    {
        RunFunction = runFunction;
    }
    public override void Execute(TContext context)
    {
        RunFunction(context);
    }
    public static implicit operator FunctionEffectRule<TContext>(Action<TContext> function)
    {
        return new FunctionEffectRule<TContext>(function);
    }
}

public class AsyncFunctionEffectRule<TContext> : EffectRule<TContext>
{
    private Func<TContext, Task> RunFunction;
    public AsyncFunctionEffectRule(Func<TContext, Task> runFunction)
    {
        RunFunction = runFunction;
    }
    public override void Execute(TContext context) { }
    public override Task ExecuteAsync(TContext context)
    {
        return RunFunction(context);
    }
    public static implicit operator AsyncFunctionEffectRule<TContext>(Func<TContext, Task> function)
    {
        return new Func<TContext, Task>(function);
    }
}