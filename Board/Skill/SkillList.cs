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
        Effect = (FunctionEffect)((Context context) =>
        {
            context.SourcePawn.Get<AnimationComponent>().Play(
                "attack",
                context.SourcePawn.Coords.SideTowards(context.PawnTarget.Coords),
                () =>
                {
                    context.PawnTarget.Get<HealthComponent>().ChangeHealth(-25);
                }
            );
        })
    };
}