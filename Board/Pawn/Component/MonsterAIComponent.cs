using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MonsterAIComponent : Component
{
    public async Task RunTurn()
    {
        Context context = new Context();
        context.SourcePawn = Pawn;
        context.Origin = Pawn.Coords;
        Decide(context);
        if (context.SourceSkill is not null)
        {
            await context.Run();
        }
    }
    public void Decide(Context context)
    {
        if (Pawn.Board.Player is null)
        {
            return;
        }
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
            context.SourceSkill = (Pawn.Get<SkillComponent>().DoubleAttack) ? SkillList.DoubleAttack : SkillList.SingleAttack;
            context.PawnTarget = Pawn.Board.Player;
            return;
        }
    }
}