using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class LocomotionComponent : Component
{
    [Export(PropertyHint.Range, "0,1")] float MovementProgress;

    public async Task MoveTo(Vector2I target)
    {
        Vector2 targetPosition = Pawn.Board.MapToLocal(target);
        isRunning = true;
        positionStart = Pawn.Position;
        positionEnd = targetPosition;
        MovementProgress = 0f;
        await animationComponent.Play("move", Pawn.Coords.SideTowards(target), () =>
            {
                Pawn.SetCoords(target);
            }
        );
        isRunning = false;
    }

    private bool isRunning;

    Vector2 positionStart;
    Vector2 positionEnd;

    AnimationComponent animationComponent => Pawn.Get<AnimationComponent>();

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (isRunning)
        {
            Pawn.Position = positionStart * (1 - MovementProgress) + positionEnd * MovementProgress;
        }
    }
    override protected IEnumerable<Type> GetDependencies()
    {
        yield return typeof(AnimationComponent);
    }

}