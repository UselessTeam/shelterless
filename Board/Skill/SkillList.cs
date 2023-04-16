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
            context.PawnTarget.Get<AnimationComponent>().Play("attack", () =>
            {
                context.PawnTarget.Get<HealthComponent>().ChangeHealth(-25);
            });
        })
    };
}