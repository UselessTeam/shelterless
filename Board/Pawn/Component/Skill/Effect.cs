using Godot;
using System;

public interface IEffect<T> where T : Context
{
    public abstract void AppliedOn(T context);
}

public class Context
{
    public Pawn Launcher;
    public Vector2I TargetCoords;
    public Pawn TargetEntity;
    public VectorUtils.Direction Direction;
}

public class DamageEffect : IEffect<Context>
{
    [Export]
    public int Damage = 10;
    public void AppliedOn(Context context)
    {
        context.TargetEntity.Get<HealthComponent>().ChangeHealth(-Damage);
    }

}
public class MoveEffect : IEffect<Context>
{
    public void AppliedOn(Context context)
    {
        context.Launcher.Get<LocomotionComponent>().RunToCoords(context.TargetCoords);
    }

}