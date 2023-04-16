using Godot;
using System.Collections.Generic;
public partial class MonsterAIComponent : Component
{
    public void TickTurn()
    {
        Context context = new Context();
        context.SourcePawn = Pawn;
        context.Origin = Pawn.Coords;
    }
    public void Decide(Context context)
    {
        int distanceToPlayer = Pawn.Coords.Distance(Pawn.Board.Player.Coords);
        if (distanceToPlayer >= 2)
        {
            List<Vector2I> path = Pawn.Board.Pathfind(Pawn.Coords, Pawn.Board.Player.Coords, nextTo: true);
            if (path is not null && path.Count >= 1)
            {
                context.SourceSkill = SkillList.Move;
                context.CoordsTarget = path[0];
                return;
            }
        }
        else if (distanceToPlayer == 1)
        {
            context.SourceSkill = SkillList.Attack;
            context.PawnTarget = Pawn.Board.Player;
            return;
        }
    }
}