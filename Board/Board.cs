using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class Board : TileMap
{
    private Dictionary<Vector2I, HashSet<Pawn>> PawnByCoords = new Dictionary<Vector2I, HashSet<Pawn>>();
    private HashSet<Pawn> Pawns = new HashSet<Pawn>();

    internal void Register(Pawn pawn)
    {
        Pawns.Add(pawn);
        if (pawn.Coords.IsValid())
        {
            GD.PrintErr($"Coords for pawn {pawn} weren't null {pawn.Coords}");
            pawn.OverrideCoords(VectorUtils.INVALID_VECTOR2I);
        }
        MovePawn(pawn, LocalToMap(pawn.Position));
    }

    internal void Unregister(Pawn pawn)
    {
        MovePawn(pawn, VectorUtils.INVALID_VECTOR2I);
        Pawns.Remove(pawn);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Action"))
        {
            Vector2I coords = LocalToMap(GetGlobalMousePosition());
            GUI.Main?.MouseOnTile(this, coords, GetGlobalMousePosition(), true);
            GetViewport().SetInputAsHandled();
        }
        if (@event is InputEventMouseMotion)
        {
            Vector2I coords = LocalToMap(GetGlobalMousePosition());
            GUI.Main?.MouseOnTile(this, coords, GetGlobalMousePosition(), false);
            GetViewport().SetInputAsHandled();
        }
    }

    public void MovePawn(Pawn pawn, Vector2I coords)
    {
        if (pawn.Coords == coords)
        {
            return;
        }
        if (pawn.Coords.IsValid())
        {
            if (PawnByCoords[pawn.Coords].Count == 1)
            {
                PawnByCoords.Remove(pawn.Coords);
            }
            else
            {
                PawnByCoords[pawn.Coords].Remove(pawn);
            }
        }
        if (coords.IsValid())
        {
            if (!PawnByCoords.ContainsKey(coords))
            {
                PawnByCoords[coords] = new HashSet<Pawn>();
            }
            PawnByCoords[coords].Add(pawn);
            pawn.OverrideCoords(coords);
        }
    }

    public IEnumerable<Pawn> GetPawnsAt(Vector2I coords)
    {
        if (PawnByCoords.ContainsKey(coords))
        {
            return PawnByCoords[coords];
        }
        else
        {
            return Enumerable.Empty<Pawn>();
        }
    }

    public Pawn GetFirstPawnAt(Vector2I coords)
    {
        if (PawnByCoords.ContainsKey(coords))
        {
            return PawnByCoords[coords].First();
        }
        else
        {
            return null;
        }
    }

    public IEnumerable<Pawn> GetPawnsWith<T>() where T : Component
    {
        foreach (Pawn pawn in Pawns)
        {
            if (pawn.Has<T>())
            {
                yield return pawn;
            }
        }
    }

    public Pawn GetFirstPawnWith<T>() where T : Component
    {
        foreach (Pawn pawn in Pawns)
        {
            if (pawn.Has<T>())
            {
                return pawn;
            }
        }
        return null;
    }

    public Pawn Player => PlayerComponent.Main.Pawn;
}

public static class VectorUtils
{
    public static readonly Vector2I INVALID_VECTOR2I = new Vector2I(int.MaxValue, int.MaxValue);
    public enum Direction : byte
    {
        NONE,
        SE,
        E,
        NE,
        NW,
        W,
        SW,
    }

    public static Vector2I ToVector2I(this Direction direction)
    {
        return direction switch
        {
            Direction.NONE => Vector2I.Zero,
            Direction.SE => Vector2I.Zero,
            Direction.E => Vector2I.Zero,
            Direction.NE => Vector2I.Zero,
            Direction.NW => Vector2I.Zero,
            Direction.W => Vector2I.Zero,
            Direction.SW => Vector2I.Zero,
            _ => INVALID_VECTOR2I,
        };
    }
    public static bool IsValid(this Vector2I vector)
    {
        return vector != INVALID_VECTOR2I;
    }
    public static bool IsInvalid(this Vector2I vector)
    {
        return vector == INVALID_VECTOR2I;
    }

    public static int Magnitude(this Vector2I vector)
    {
        return Mathf.Max(Mathf.Max(Mathf.Abs(vector.X), Mathf.Abs(vector.Y)), Mathf.Abs(vector.X + vector.Y));
    }

    public static int Distance(this Vector2I vector, Vector2I other)
    {
        return (vector - other).Magnitude();
    }
}