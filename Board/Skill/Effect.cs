using Godot;
using System;
using System.Threading.Tasks;

public interface Effect
{
    public abstract Task RunOn(Context context);
}

public class FunctionEffect : Effect
{
    public Func<Context, Task> Apply;

    public async Task RunOn(Context context)
    {
        await Apply(context);
    }
    public static implicit operator FunctionEffect(Func<Context, Task> function) => new FunctionEffect { Apply = function };
    public static implicit operator FunctionEffect(Action<Context> function) => new FunctionEffect
    {
        Apply = (Context context) =>
        {
            function(context);
            return Task.FromResult<object>(null);
        }
    };
}