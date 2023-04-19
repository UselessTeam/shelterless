using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EffectClass
{
    public class Move : Effect
    {
        public async Task RunOn(Context context)
        {
            await context.SourcePawn.Get<LocomotionComponent>().MoveTo(context.CoordsTarget);
        }
    }

    public class MultiDamage : Effect
    {
        private int Count = 1;
        public MultiDamage(int count = 1)
        {
            Count = count;
        }
        public async Task RunOn(Context context)
        {
            Action damageOnce = () => Damage(context.SourcePawn, context.PawnTarget);
            Action[] damageMulti = Enumerable.Range(0, Count).Select(i => damageOnce).ToArray();
            await context.SourcePawn.Get<AnimationComponent>().Play(
                "attack",
                context.SourcePawn.Coords.SideTowards(context.PawnTarget.Coords),
                damageMulti
            );
        }

        public static void Damage(Pawn source, Pawn target)
        {
            int damage = -source.Get<SkillComponent>()?.Damage ?? 10;
            target.Get<AnimationComponent>().PlayText($"{damage}", Colors.Red);
            target.Get<HealthComponent>().ChangeHealth(damage);
        }
    }

    public class SlideInDirectionEffect : Effect
    {
        private int Slide = 1;
        public SlideInDirectionEffect(int slide = 0)
        {
            Slide = slide;
        }
        public async Task RunOn(Context context)
        {
            Vector2I targetCoord = context.Origin + context.DirectionTarget.ToVector2I();
            Pawn targetPawn = context.SourcePawn.Board.GetFirstPawnAt(targetCoord);
            if (targetPawn is null)
            {
                GD.PrintErr("No pawn to slide!");
                return;
            }
            await context.SourcePawn.Get<AnimationComponent>().Play(
                "attack",
                context.SourcePawn.Coords.SideTowards(targetCoord),
                () => targetPawn.Get<LocomotionComponent>()?.PushTowards(context.DirectionTarget, Slide)
            );
        }
    }
}
public static class Effects
{
    public static Effect Move = new EffectClass.Move();
    public static Effect SingleDamage = new EffectClass.MultiDamage(1);
    public static Effect DoubleDamage = new EffectClass.MultiDamage(2);
    public static Effect PushOne = new EffectClass.SlideInDirectionEffect(1);
}

public static class SkillList
{
    public static Skill Move = new Skill
    {
        Name = "Move",
        Target = Skill.TargetType.TILE,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = Effects.Move,
    };
    public static Skill Push = new Skill
    {
        Name = "Push",
        Target = Skill.TargetType.DIRECTION,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = Effects.PushOne,
    };
    public static Skill SingleAttack = new Skill
    {
        Name = "SingleAttack",
        Target = Skill.TargetType.ENTITY,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = Effects.SingleDamage,
    };
    public static Skill DoubleAttack = new Skill
    {
        Name = "DoubleAttack",
        Target = Skill.TargetType.ENTITY,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = Effects.DoubleDamage,
    };
}