using Godot;

public static class SkillList
{
    public static Skill Move = new Skill
    {
        Name = "Move",
        Target = Skill.TargetType.TILE,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = new MoveEffect(),
    };
    public static Skill Attack = new Skill
    {
        Name = "Attack",
        Target = Skill.TargetType.ENTITY,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = new FunctionEffect
        {
            Apply = async (Context context) =>
        {
            await context.SourcePawn.Get<AnimationComponent>().Play(
                "attack",
                context.SourcePawn.Coords.SideTowards(context.PawnTarget.Coords),
                () =>
                {
                    int damage = -25;
                    context.PawnTarget.Get<AnimationComponent>().PlayText($"{damage}", Colors.Red);
                    context.PawnTarget.Get<HealthComponent>().ChangeHealth(damage);
                }
            );
        }
        }
    };
}