using Godot;
using System;

public interface Effect
{
    public abstract void AppliedOn(Context context);
}

public class MoveEffect : Effect
{
    public void AppliedOn(Context context)
    {
        context.SourcePawn.Get<LocomotionComponent>().RunToCoords(context.CoordsTarget);
        context.SourcePawn.SetCoords(context.CoordsTarget);
    }
}

public class FunctionEffect : Effect
{
    public Action<Context> Apply;

    public void AppliedOn(Context context)
    {
        Apply(context);
    }
    public static implicit operator FunctionEffect(Action<Context> function) => new FunctionEffect { Apply = function };
}