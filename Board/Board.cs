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
            pawn.SetCoords(VectorUtils.INVALID_VECTOR2I);
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
        if (@event is InputEventMouse inputEventMouse)
        {
            Vector2I coords = LocalToMap(GetGlobalMousePosition());
            GUI.Main?.MouseOnTile(this, coords, GetGlobalMousePosition(), inputEventMouse);
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
            pawn.SetCoords(coords);
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

    public Pawn Player => GetFirstPawnWith<PlayerComponent>();
}

public static class VectorUtils
{
    public static readonly Vector2I INVALID_VECTOR2I = new Vector2I(int.MaxValue, int.MaxValue);
    public static bool IsValid(this Vector2I vector)
    {
        return vector != INVALID_VECTOR2I;
    }
    public static bool IsInvalid(this Vector2I vector)
    {
        return vector == INVALID_VECTOR2I;
    }
}